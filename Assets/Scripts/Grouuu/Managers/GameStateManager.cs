using Grouuu.Constants;
using Grouuu.Enum;
using Grouuu.PersistentManagers;
using Grouuu.States;
using System;
using System.Linq;
using UnityEngine;

namespace Grouuu.Managers
{
	public class GameStateManager
	{
		public GameState InitialState => _settings.InitialState;

		private readonly GameStateManagerSettings _settings;
		private StateCommand[] _stateCommands;
		private StateCommand _currentState;

		public GameStateManager (GameStateManagerSettings settings)
		{
			_settings = settings;
			Init();
		}

		public void SetState (GameState state)
		{
			StateCommand stateCommand = GetStateCommand(state);

			if (stateCommand != null)
				SetState(stateCommand);
			else
				Debug.LogWarning($"State command not found with {state}");
		}

		public void SetState (StateCommand stateCommand)
		{
			if (stateCommand == null || stateCommand.State == GameState.None)
			{
				Debug.LogError($"Incorrect state set");
				return;
			}

			GameState previousState = GameState.None;

			if (_currentState != null)
			{
				_currentState.OnStateEnd -= NextState;
				previousState = _currentState.State;
			}

			_currentState = stateCommand;

			UpdateSaveData();

			_currentState.OnStateEnd += NextState;

			_currentState.StartCommand(previousState);
		}

		public GameState[] GetStatesName ()
		{
			return _stateCommands.Select(state => state.State).ToArray();
		}

		private void Init ()
		{
			_stateCommands = _settings.StateCommands;
		}

		private void UpdateSaveData ()
		{
			GameController.GameManagers.SaveManager.AddToSaveData(SaveItemKey.State, _currentState.State);
		}

		private void NextState (GameState forceState)
		{
			StateCommand nextState;

			if (forceState != GameState.None)
				nextState = GetStateCommand(forceState);
			else
				nextState = GetNextState();

			if (nextState == null)
			{
				Debug.LogWarning($"No state available");
				return;
			}

			SetState(nextState);
		}

		private StateCommand GetNextState ()
		{
			if (_stateCommands == null || _stateCommands.Length == 0)
				return null;

			int currentIndex = Array.FindIndex(_stateCommands, state => state.State == _currentState.State);
			int targetIndex = currentIndex == _stateCommands.Length - 1 ? 0 : currentIndex + 1;
			return _stateCommands[targetIndex];
		}

		private StateCommand GetStateCommand (GameState state)
		{
			return Array.Find(_stateCommands, command => command.State == state);
		}
	}
}