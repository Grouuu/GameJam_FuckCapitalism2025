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
		await PersistentManager.Instance.InitPersistentData();
	}

	private async Awaitable InitDatabase ()
	{
		await databaseManager.LoadDatabase();
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

	private void InitSounds ()
	{
		uiManager.optionsPanel.volumeControls.UpdateComponent();
		soundManager.RestartMusic();
	}

	private async void StartGame ()
	{
		// play intro
		sceneEffectsManager.ResumeSceneEffects();
		await sceneEffectsManager.PlaySceneEffect(SceneEffectName.IntroResilienceShip);

		// flag run as started
		saveManager.AddToSaveData(SaveItemKey.RunStarted, true);
		await saveManager.SaveData();

		// resume game state
		GameState startState = saveManager.GetSaveData<GameState>(SaveItemKey.State);

		if (startState == GameState.None)
			startState = initialState;

		// start the game
		gameStateManager.SetState(startState);
	}

}
