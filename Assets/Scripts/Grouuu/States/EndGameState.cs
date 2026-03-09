using Grouuu.Constants;
using Grouuu.Data;
using Grouuu.Enum;
using UnityEngine.SceneManagement;

namespace Grouuu.States
{
	public class EndGameState : StateCommand
	{
		public override GameState State => GameState.EndGame;

		public override void StartCommand (GameState previousState)
		{
			ShowLose();
		}

		private async void ShowLose ()
		{
			// delete save
			await GameController.GameManagers.SaveManager.DeleteGameSave();

			EndingData endingData = GameController.GameManagers.EndingsManager.CheckLose();

			await endingData.UpdateEnterSceneEffects();

			GameController.GameManagers.EndingsManager.ShowLose(() => OnLose(endingData));
		}

		private async void OnLose (EndingData endingData)
		{
			await endingData.UpdateExitSceneEffects();

			Restart();
		}

		private void Restart ()
		{
			SceneManager.LoadScene(SceneList.MAIN);
		}
	}
}