using Grouuu.Constants;
using Grouuu.PersistentManagers;
using UnityEngine;
using UnityEngine.UI;

namespace Grouuu.UI
{
	public class VolumeControlsUI : MonoBehaviour
	{
		public Button muteButton;
		public Sprite soundOnSprite;
		public Sprite soundOffSprite;
		public Slider volumeSlider;

		private SaveManager _saveManager => PersistentController.Instance.SaveManager;
		private SoundManager _soundManager => PersistentController.Instance.SoundManager;

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
			_soundManager.SetMusicMute(!_soundManager.GetMusicMute());

			UpdateMuteButtonSprite();

			_saveManager.AddToSaveData(SaveItemKey.MusicMute, _soundManager.GetMusicMute());
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
}