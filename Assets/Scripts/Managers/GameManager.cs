using System;
using UnityEngine;

public class Managers
{
	public GameManager gameManager;
	public SaveManager saveManager;
	public SoundManager soundManager;
	public I2Manager localizationManager;
	public DatabaseManager databaseManager;
	public UIManager uiManager;
	public GameStateManager gameStateManager;
	public VarsManager varsManager;
	public CharactersManager charactersManager;
	public EventsManager eventsManager;
	public EndingsManager endingsManager;
	public ProductionManager productionManager;
	public BuildingsManager buildingsManager;
	public SceneEffectsManager sceneEffectsManager;
}

public class GameManager : MonoBehaviour
{
	[NonSerialized] public static GameManager Instance;

	public GameState initialState;
	public int maxDialogsByDay;
	public int minPopulationGrowth;
	public int maxPopulationGrowth;

	public SaveManager saveManager => _managers.saveManager;
	public SoundManager soundManager => _managers.soundManager;
	public I2Manager localizationManager => _managers.localizationManager;
	public DatabaseManager databaseManager => _managers.databaseManager;
	public UIManager uiManager => _managers.uiManager;
	public GameStateManager gameStateManager => _managers.gameStateManager;
	public VarsManager varsManager => _managers.varsManager;
	public CharactersManager charactersManager => _managers.charactersManager;
	public EventsManager eventsManager => _managers.eventsManager;
	public EndingsManager endingsManager => _managers.endingsManager;
	public ProductionManager productionManager => _managers.productionManager;
	public BuildingsManager buildingsManager => _managers.buildingsManager;
	public SceneEffectsManager sceneEffectsManager => _managers.sceneEffectsManager;

	private Managers _managers;

	public Managers GetManagers ()
	{
		return new()
		{
			gameManager = this,
			saveManager = PersistentManager.Instance.saveManager,
			soundManager = PersistentManager.Instance.soundManager,
			localizationManager = PersistentManager.Instance.localizationManager,
			databaseManager = GetComponentInChildren<DatabaseManager>(),
			uiManager = GetComponentInChildren<UIManager>(),
			gameStateManager = GetComponentInChildren<GameStateManager>(),
			varsManager = GetComponentInChildren<VarsManager>(),
			charactersManager = GetComponentInChildren<CharactersManager>(),
			eventsManager = GetComponentInChildren<EventsManager>(),
			endingsManager = GetComponentInChildren<EndingsManager>(),
			productionManager = GetComponentInChildren<ProductionManager>(),
			buildingsManager = GetComponentInChildren<BuildingsManager>(),
			sceneEffectsManager = GetComponentInChildren<SceneEffectsManager>()
		};
	}

	private void OnEnable ()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}

	private void Start ()
	{
		InitManagers();
		InitGame();
	}

	private void InitManagers ()
	{
		_managers = GetManagers();
	}

	private async void InitGame ()
	{
		InitSounds();

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

		StartGame();
	}

	private async Awaitable InitPersistentData ()
	{
		await PersistentManager.Instance.InitPersistentData();
	}

	private async Awaitable InitDatabase ()
	{
		await databaseManager.LoadDatabase();
	}

	private void InitSounds ()
	{
		soundManager.RestartMusic();
		uiManager.optionsPanel.soundSlider.value = soundManager.GetMusicVolume();
	}

	private void InitVars ()
	{
		varsManager.InitVars(databaseManager.GetData<VarData>());
	}

	private void InitCharacters ()
	{
		charactersManager.InitCharacters(databaseManager.GetData<CharacterData>(), databaseManager.GetData<DialogData>());
	}

	private void InitEvents ()
	{
		eventsManager.InitEvents(databaseManager.GetData<EventData>());
	}

	private void InitEndings ()
	{
		endingsManager.InitEndings(databaseManager.GetData<EndingData>());
	}

	private void InitProductions ()
	{
		productionManager.InitProductions(databaseManager.GetData<ProductionData>());
	}

	private void InitBuildings ()
	{
		buildingsManager.InitBuildings(databaseManager.GetData<BuildingData>());
	}

	private void InitSceneEffects ()
	{
		sceneEffectsManager.InitSceneEffects(databaseManager.GetData<SceneEffectData>());
	}

	private void SaveGeneratedData ()
	{
		// save events random generated days at fresh start only
		if (!SaveManager.IsRunStarted)
			eventsManager.UpdateEventsDaySaveData();
	}

	private void ApplySave ()
	{
		varsManager.ApplySave();
		charactersManager.ApplySave();
		eventsManager.ApplySave();
		endingsManager.ApplySave();
		buildingsManager.ApplySave();
		sceneEffectsManager.ApplySave();
	}

	private async void StartGame ()
	{
		sceneEffectsManager.ResumeSceneEffects();

		await sceneEffectsManager.PlaySceneEffect(SceneEffectName.IntroResilienceShip);

		saveManager.AddToSaveData(SaveItemKey.RunStarted, true);

		await saveManager.SaveData();

		GameState startState = saveManager.GetSaveData<GameState>(SaveItemKey.State);

		if (startState == GameState.None)
			startState = initialState;

		gameStateManager.SetState(startState);
	}

}
