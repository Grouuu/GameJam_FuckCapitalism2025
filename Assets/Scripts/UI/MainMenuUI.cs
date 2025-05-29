using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
	public SaveManager saveManager;
	public Button continueButton;
	public Button newGameButton;
	public Button exitButton;
	public Slider soundSlider;

	/**
	 * Linked in the editor
	 */
	public void OnContinueClick ()
	{
		SceneManager.LoadScene(SceneList.GAMEPLAY);
	}

	/**
	 * Linked in the editor
	 */
	public void OnNewGameClick ()
	{
		saveManager.DeleteSave();
		SceneManager.LoadScene(SceneList.GAMEPLAY);
	}

	/**
	 * Linked in the editor
	 */
	public void OnExitClick ()
	{
		Application.Quit();
	}

	/**
	 * Linked in the editor
	 */
	public void OnVolumeChange ()
	{
		// TODO
	}

	private void Awake ()
	{
		// disable exit button on web
#if UNITY_WEBGL && !UNITY_EDITOR
		exitButton.gameObject.SetActive(false);
#endif
		// disable continue button if no save
		if (!saveManager.HasSave())
			continueButton.gameObject.SetActive(false);
	}

}
