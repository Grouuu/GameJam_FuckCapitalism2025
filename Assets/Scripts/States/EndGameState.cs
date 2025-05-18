using UnityEngine;

public class EndGameState : StateCommand
{
	public override void StartCommand ()
	{
		// TODO
		Debug.Log("GAME OVER");
	}

	private void OnEnable () => state = GameState.EndGame;
}
