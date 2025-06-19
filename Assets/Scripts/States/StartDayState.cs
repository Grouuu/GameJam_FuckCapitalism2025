using UnityEngine;

public class StartDayState : StateCommand
{
	public override GameState state => GameState.StartDay;

	public override void StartCommand (GameState previousState)
	{
		// save internally resources value (for the next daily report)
		GameManager.Instance.varsManager.SaveStartDayResourcesValue();

		UpdateNewDayEffects();
	}

	private async void UpdateNewDayEffects ()
	{
		await GameManager.Instance.sceneEffectsManager.PlayStartDayEffects();

		EndCommand();
	}

}
