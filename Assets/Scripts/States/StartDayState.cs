using UnityEngine;

public class StartDayState : StateCommand
{
	public override void StartCommand (GameState previousState)
	{
		// save internally resources value (for the next daily report)
		GameManager.Instance.varsManager.SaveStartDayResourcesValue();

		EndCommand();
	}

	private void OnEnable () => state = GameState.StartDay;

}
