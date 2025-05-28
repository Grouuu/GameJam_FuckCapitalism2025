using System;
using UnityEngine;

public enum GameState
{
	None,
	StartDay,
	PlayDialog,
	PlayEvent,
	DailyReport,
	EndGame,
}

public class GameStateManager : MonoBehaviour
{
	public StateCommand currentState { get; private set; }

	private StateCommand[] _stateCommands;

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
		if (stateCommand == null || stateCommand.state == GameState.None)
		{
			Debug.LogError($"Incorrect state set");
			return;
		}

		GameState previousState = GameState.None;

		if (currentState != null)
		{
			currentState.OnStateEnd -= NextState;
			previousState = currentState.state;
		}

		currentState = stateCommand;

		UpdateSaveData();

		currentState.OnStateEnd += NextState;

		currentState.StartCommand(previousState);
	}

	private void UpdateSaveData ()
	{
		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.State, currentState.state);
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

	private void OnEnable ()
	{
		_stateCommands = GetComponents<StateCommand>();
	}

	private StateCommand GetNextState ()
	{
		if (_stateCommands == null || _stateCommands.Length == 0)
			return null;

		int currentIndex = Array.FindIndex(_stateCommands, state => state.state == currentState.state);
		int targetIndex = currentIndex == _stateCommands.Length - 1 ? 0 : currentIndex + 1;
		return _stateCommands[targetIndex];
	}

	private StateCommand GetStateCommand (GameState state)
	{
		return Array.Find(_stateCommands, command => command.state == state);
	}

}
