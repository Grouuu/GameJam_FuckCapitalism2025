using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
	public SaveManager saveManager;
	public Button continueButton;
	public Button newGameButton;
	public Button exitButton;
	public Button muteButton;
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
	public void OnMuteClick ()
	{
		PersistentManager.Instance.SetMusicMute(!PersistentManager.Instance.GetMusicMute());

		saveManager.AddToSaveData(SaveItemKey.MusicMute, PersistentManager.Instance.GetMusicMute());
		_ = saveManager.SaveData();
	}

	/**
	 * Linked in the editor
	 */
	public void OnVolumeChange ()
	{
		PersistentManager.Instance.SetMusicVolume(soundSlider.value);
	}

	/**
	 * Linked in the editor
	 */
	public void OnVolumeChangeEnd ()
	{
		saveManager.AddToSaveData(SaveItemKey.MusicVolume, PersistentManager.Instance.GetMusicVolume());
		_ = saveManager.SaveData();
	}

	private void OnEnable ()
	{
		Init();
	}

	private async void Init ()
	{
		saveManager.Init();

		await saveManager.LoadData();

		InitSound();

		PersistentManager.Instance.ChangeScene();

		// disable exit button on web
#if UNITY_WEBGL && !UNITY_EDITOR
		exitButton.gameObject.SetActive(false);
#endif
		// disable continue button if no save
		if (!saveManager.HasSave())
			continueButton.gameObject.SetActive(false);
	}

	private void InitSound ()
	{
		if (saveManager.HasKey(SaveItemKey.MusicVolume))
			PersistentManager.Instance.SetMusicVolume(saveManager.GetSaveData<float>(SaveItemKey.MusicVolume));

		if (saveManager.HasKey(SaveItemKey.MusicMute))
			PersistentManager.Instance.SetMusicMute(saveManager.GetSaveData<bool>(SaveItemKey.MusicMute));

		soundSlider.value = PersistentManager.Instance.GetMusicVolume();
	}

}
