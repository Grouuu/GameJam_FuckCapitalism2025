using Grouuu.Data;
using Grouuu.Enum;
using Grouuu.Managers;
using Grouuu.PersistentManagers;
using Grouuu.Constants;
using UnityEngine;

namespace Grouuu
{
	public class GameController : MonoBehaviour
	{
		private static GameManagers _managers;
		public static GameManagers GameManagers
		{
			get => _managers;
		}

		public GameSettings GameSettings;

		public GameManagers GetManagers ()
		{
			return new()
			{
				SaveManager =			PersistentController.Instance.SaveManager,
				SoundManager =			PersistentController.Instance.SoundManager,
				LocalizationManager =	PersistentController.Instance.LocalizationManager,
				DebugManager =			GetComponent<DebugManager>(),
				UiManager =				GetComponentInChildren<UIManager>(),
				SceneEffectsManager =	GetComponentInChildren<SceneEffectsManager>(),
				DatabaseManager =		new (GameSettings.DatabaseManagerSettings),
				GameStateManager =		new (GameSettings.GameStateManagerSettings),
				VarsManager =			new (GameSettings.VarsManagerSettings),
				CharactersManager =		new (GameSettings.CharactersManagerSettings),
				EventsManager =			new (GameSettings.EventsManagerSettings),
				EndingsManager =		new (GameSettings.EndingsManagerSettings),
				ProductionManager =		new (GameSettings.ProductionManagerSettings),
				BuildingsManager =		new (GameSettings.BuildingsManagerSettings),
			};
		}

		private void Awake ()
		{
			InitManagers();
		}

		private void OnEnable ()
		{
			InitGame();
		}

		private void InitManagers ()
		{
			_managers = GetManagers();
		}

		private async void InitGame ()
		{
			await InitDatabase();
			InitVars();
			InitCharacters();
			InitEvents();
			InitEndings();
			InitProductions();
			InitBuildings();
			InitSceneEffects();

			await InitPersistentData(); // load save
			SaveGeneratedData();
			ApplySave();

			InitSounds();

			StartGame();
		}

		private async Awaitable InitPersistentData ()
		{
			await PersistentController.Instance.InitPersistentData();
		}

		private async Awaitable InitDatabase ()
		{
			await _managers.DatabaseManager.LoadDatabase();
		}

		private void InitVars ()
		{
			_managers.VarsManager.InitVars(_managers.DatabaseManager.GetData<VarData>());
		}

		private void InitCharacters ()
		{
			_managers.CharactersManager.InitCharacters(_managers.DatabaseManager.GetData<CharacterData>(), _managers.DatabaseManager.GetData<DialogData>());
		}

		private void InitEvents ()
		{
			_managers.EventsManager.InitEvents(_managers.DatabaseManager.GetData<EventData>());
		}

		private void InitEndings ()
		{
			_managers.EndingsManager.InitEndings(_managers.DatabaseManager.GetData<EndingData>());
		}

		private void InitProductions ()
		{
			_managers.ProductionManager.InitProductions(_managers.DatabaseManager.GetData<ProductionData>());
		}

		private void InitBuildings ()
		{
			_managers.BuildingsManager.InitBuildings(_managers.DatabaseManager.GetData<BuildingData>());
		}

		private void InitSceneEffects ()
		{
			_managers.SceneEffectsManager.InitSceneEffects(_managers.DatabaseManager.GetData<SceneEffectData>());
		}

		private void SaveGeneratedData ()
		{
			// save events random generated days at fresh start only
			if (!SaveManager.IsRunStarted)
				_managers.EventsManager.UpdateEventsDaySaveData();
		}

		private void ApplySave ()
		{
			_managers.VarsManager.ApplySave();
			_managers.CharactersManager.ApplySave();
			_managers.EventsManager.ApplySave();
			_managers.EndingsManager.ApplySave();
			_managers.BuildingsManager.ApplySave();
			_managers.SceneEffectsManager.ApplySave();
		}

		private void InitSounds ()
		{
			_managers.UiManager.OptionsPanel.volumeControls.UpdateComponent();
			_managers.SoundManager.RestartMusic();
		}

		private async void StartGame ()
		{
			// play intro
			_managers.SceneEffectsManager.ResumeSceneEffects();
			await _managers.SceneEffectsManager.PlaySceneEffect(SceneEffectName.IntroResilienceShip);

			// flag run as started
			_managers.SaveManager.AddToSaveData(SaveItemKey.RunStarted, true);
			await _managers.SaveManager.SaveData();

			// resume game state
			GameState startState = _managers.SaveManager.GetSaveData<GameState>(SaveItemKey.State);

			if (startState == GameState.None)
				startState = GameManagers.GameStateManager.InitialState;

			// start the game
			_managers.GameStateManager.SetState(startState);
		}

	}
}
