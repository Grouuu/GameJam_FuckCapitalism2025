using System;

public class StateController
{
	public GameState State => currentState;

	protected GameState[] states;
	protected GameState currentState;

	public void Init (GameState[] states, GameState startState)
	{
		this.states = states;
		currentState = startState;
	}

	public void SetState (GameState state)
	{
		currentState = state;
	}

	public GameState NextState ()
	{
		int index = (Array.FindIndex(states, state => state == currentState) + 1) % states.Length;
		return states[index];
	}

}
