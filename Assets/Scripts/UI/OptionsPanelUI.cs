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
		if (PersistentManager.Instance != null)
		{
			PersistentManager.Instance.SetMusicMute(!PersistentManager.Instance.GetMusicMute());

			GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.MusicMute, PersistentManager.Instance.GetMusicMute());
			_ = GameManager.Instance.saveManager.SaveData();
		}
	}

	/**
	 * Linked in the editor
	 */
	public void OnVolumeChange ()
	{
		if (PersistentManager.Instance != null)
			PersistentManager.Instance.SetMusicVolume(soundSlider.value);
	}

	/**
	 * Linked in the editor
	 */
	public void OnVolumeChangeEnd ()
	{
		if (PersistentManager.Instance != null)
		{
			GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.MusicVolume, PersistentManager.Instance.GetMusicVolume());
			_ = GameManager.Instance.saveManager.SaveData();
		}
	}

	private void Awake ()
	{
		parent.SetActive(false);
	}

}
