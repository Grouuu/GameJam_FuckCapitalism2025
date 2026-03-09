using Grouuu.Enum;

namespace Grouuu.States
{
	public class StartDayState : StateCommand
	{
		public override GameState State => GameState.StartDay;

		public override void StartCommand (GameState previousState)
		{
			// save internally resources value (for the next daily report)
			GameController.GameManagers.VarsManager.SaveStartDayResourcesValue();

			UpdateNewDayEffects();
		}

		private async void UpdateNewDayEffects ()
		{
			await GameController.GameManagers.SceneEffectsManager.PlayStartDayEffects();

			EndCommand();
		}
	}
}