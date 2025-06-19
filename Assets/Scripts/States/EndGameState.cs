using UnityEngine.SceneManagement;

public class EndGameState : StateCommand
{
	public override GameState state => GameState.EndGame;

	public override void StartCommand (GameState previousState)
	{
		ShowLose();
	}

	private async void ShowLose ()
	{
		// delete save
		await GameManager.Instance.saveManager.DeleteGameSave();

		EndingData endingData = GameManager.Instance.endingsManager.CheckLose();

		await endingData.UpdateEnterSceneEffects();

		GameManager.Instance.endingsManager.ShowLose(() => OnLose(endingData));
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
