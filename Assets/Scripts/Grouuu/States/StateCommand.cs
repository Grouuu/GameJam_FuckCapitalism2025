using Grouuu.Enum;
using System;

namespace Grouuu.States
{
	public delegate void OnStateEnd (GameState state = GameState.None);

	public class StateCommand
	{
		public event OnStateEnd OnStateEnd;

		public virtual GameState State => GameState.None;

		public virtual void StartCommand (GameState previousState = GameState.None)
		{
			throw new NotImplementedException();
		}

		public virtual void EndCommand (GameState nextState = GameState.None)
		{
			OnStateEnd?.Invoke(nextState);
		}
	}
}