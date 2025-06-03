using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsPanelUI : MonoBehaviour
{
	public GameObject parent;
	public Button optionsButton;
	public Button closeButton;
	public Button mainMenuButton;
	public Button muteButton;
	public Slider soundSlider;

	/**
	 * Linked in the editor
	 */
	public void OnOptionsClick ()
	{
		parent.SetActive(true);
	}

	/**
	 * Linked in the editor
	 */
	public void OnCloseClick ()
	{
		parent.SetActive(false);
	}

	/**
	 * Linked in the editor
	 */
	public void OnMainMenuClick ()
	{
		SceneManager.LoadScene(SceneList.MAIN);
	}

	/**
	 * Linked in the editor
	 */
	public void OnMuteClick ()
	{
		PersistentManager.Instance.soundManager.SetMusicMute(!GameManager.Instance.soundManager.GetMusicMute());

		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.MusicMute, GameManager.Instance.soundManager.GetMusicMute());
		_ = GameManager.Instance.saveManager.SaveData();
	}

	/**
	 * Linked in the editor
	 */
	public void OnVolumeChange ()
	{
		GameManager.Instance.soundManager.SetMusicVolume(soundSlider.value);
	}

	/**
	 * Linked in the editor
	 */
	public void OnVolumeChangeEnd ()
	{
		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.MusicVolume, GameManager.Instance.soundManager.GetMusicVolume());
		_ = GameManager.Instance.saveManager.SaveData();
	}

	/**
	 * Linked in the editor
	 */
	public void OnVolumeRelease ()
	{
		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.MusicVolume, GameManager.Instance.soundManager.GetMusicVolume());
		_ = GameManager.Instance.saveManager.SaveData();
	}

	private void Awake ()
	{
		parent.SetActive(false);
	}

}
