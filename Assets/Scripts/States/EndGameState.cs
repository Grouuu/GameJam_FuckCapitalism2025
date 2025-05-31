using UnityEngine.SceneManagement;

public class EndGameState : StateCommand
{
	public override void StartCommand (GameState previousState)
	{
		GameManager.Instance.endingsManager.ShowLose(() => OnRestart());
	}

	private void OnEnable () => state = GameState.EndGame;

	private void OnRestart ()
	{
		// delete save
		GameManager.Instance.saveManager.DeleteSave();

		SceneManager.LoadScene(SceneList.MAIN);
	}

}
