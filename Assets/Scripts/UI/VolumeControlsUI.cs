using UnityEngine;
using UnityEngine.UI;

public class VolumeControlsUI : MonoBehaviour
{
	public Button muteButton;
	public Sprite soundOnSprite;
	public Sprite soundOffSprite;
	public Slider volumeSlider;

	private SaveManager _saveManager => PersistentManager.Instance.saveManager;
	private SoundManager _soundManager => PersistentManager.Instance.soundManager;

	public void UpdateComponent ()
	{
		volumeSlider.value = _soundManager.GetMusicVolume();
		UpdateMuteButtonSprite();
	}

	/**
	 * Linked in the editor
	 */
	public void OnMuteClick ()
	{
		_soundManager.SetMusicMute(!PersistentManager.Instance.soundManager.GetMusicMute());

		UpdateMuteButtonSprite();

		_saveManager.AddToSaveData(SaveItemKey.MusicMute, PersistentManager.Instance.soundManager.GetMusicMute());
		_ = _saveManager.SaveData();
	}

	/**
	 * Linked in the editor
	 */
	public void OnVolumeChange ()
	{
		_soundManager.SetMusicVolume(volumeSlider.value);
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
		// NOTE : do not resume saved config when the project starts on the gameplay scene
		UpdateComponent();
	}

	private void UpdateMuteButtonSprite ()
	{
		muteButton.GetComponent<Image>().sprite = _soundManager.GetMusicMute() ? soundOffSprite : soundOnSprite;
	}

}
