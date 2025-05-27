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
		// TODO go to main menu instead

		string currentSceneName = SceneManager.GetActiveScene().name;
		SceneManager.LoadScene(currentSceneName);
	}

}
