using Grouuu.Constants;
using Grouuu.PersistentManagers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Grouuu.UI
{
	public class MainMenuUI : MonoBehaviour
	{
		public Button continueButton;
		public Button newGameButton;
		public Button exitButton;
		public VolumeControlsUI volumeControls;

		private SoundManager _soundManager => PersistentController.Instance.SoundManager;
		private SaveManager _saveManager => PersistentController.Instance.SaveManager;

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

		private void Start ()
		{
			Init();
		}

		private async void Init ()
		{
			await PersistentController.Instance.InitPersistentData();

			InitSound();
			InitMenus();
		}

		private void InitSound ()
		{
			volumeControls.UpdateComponent();
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
}