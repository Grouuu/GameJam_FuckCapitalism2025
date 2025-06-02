using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
	public Button continueButton;
	public Button newGameButton;
	public Button exitButton;
	public Button muteButton;
	public Slider soundSlider;

	private SoundManager _soundManager => PersistentManager.Instance.soundManager;
	private SaveManager _saveManager => PersistentManager.Instance.saveManager;

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
		_ = _saveManager.DeleteGameSave();
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
		_soundManager.SetMusicMute(!PersistentManager.Instance.soundManager.GetMusicMute());

		_saveManager.AddToSaveData(SaveItemKey.MusicMute, PersistentManager.Instance.soundManager.GetMusicMute());
		_ = _saveManager.SaveData();
	}

	/**
	 * Linked in the editor
	 */
	public void OnVolumeChange ()
	{
		_soundManager.SetMusicVolume(soundSlider.value);
	}

	/**
	 * Linked in the editor
	 */
	public void OnVolumeChangeEnd ()
	{
		_saveManager.AddToSaveData(SaveItemKey.MusicVolume, _soundManager.GetMusicVolume());
		_ = _saveManager.SaveData();
	}

	private void Start ()
	{
		Init();
	}

	private async void Init ()
	{
		await PersistentManager.Instance.InitPersistentData();

		InitSound();
		InitMenus();
	}

	private void InitSound ()
	{
		if (_saveManager.HasKey(SaveItemKey.MusicVolume))
			_soundManager.SetMusicVolume(_saveManager.GetSaveData<float>(SaveItemKey.MusicVolume));

		if (_saveManager.HasKey(SaveItemKey.MusicMute))
			_soundManager.SetMusicMute(_saveManager.GetSaveData<bool>(SaveItemKey.MusicMute));

		soundSlider.value = _soundManager.GetMusicVolume();

		_soundManager.RestartMusic();
	}

	private void InitMenus ()
	{
		// disable exit button on web
#if UNITY_WEBGL && !UNITY_EDITOR
		exitButton.gameObject.SetActive(false);
#endif
		// disable continue button if no save
		if (!_saveManager.HasKey(SaveItemKey.RunStarted))
			continueButton.gameObject.SetActive(false);
	}

}
