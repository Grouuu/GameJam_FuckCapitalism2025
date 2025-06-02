using UnityEngine.SceneManagement;

public class EndGameState : StateCommand
{
	public override void StartCommand (GameState previousState)
	{
		GameManager.Instance.endingsManager.ShowLose(() => OnRestart());
	}

	private void OnEnable () => state = GameState.EndGame;

	private async void OnRestart ()
	{
		// delete save
		await GameManager.Instance.saveManager.DeleteGameSave();

		SceneManager.LoadScene(SceneList.MAIN);
	}

}
