using System;
using UnityEngine;

public class Managers
{
	public GameManager gameManager;
	public DatabaseManager databaseManager;
	public UIManager uiManager;
	public GameStateManager gameStateManager;
	public VarsManager varsManager;
	public CharactersManager charactersManager;
	public EventsManager eventsManager;
	public EndingsManager endingsManager;
}

public class GameManager : MonoBehaviour
{
	[NonSerialized] public static GameManager Instance;

	public GameState initialState;
	public int maxDialogsByDay;
	public int minPopulationGrowth;
	public int maxPopulationGrowth;

	// persistent managers
	[HideInInspector] public SaveManager saveManager;
	[HideInInspector] public SoundManager soundManager;

	// scene managers
	[HideInInspector] public DatabaseManager databaseManager;
	[HideInInspector] public UIManager uiManager;
	[HideInInspector] public GameStateManager gameStateManager;
	[HideInInspector] public VarsManager varsManager;
	[HideInInspector] public CharactersManager charactersManager;
	[HideInInspector] public EventsManager eventsManager;
	[HideInInspector] public EndingsManager endingsManager;

	public Managers GetManagers ()
	{
		return new()
		{
			gameManager = this,
			databaseManager = GetComponentInChildren<DatabaseManager>(),
			uiManager = GetComponentInChildren<UIManager>(),
			gameStateManager = GetComponentInChildren<GameStateManager>(),
			varsManager = GetComponentInChildren<VarsManager>(),
			charactersManager = GetComponentInChildren<CharactersManager>(),
			eventsManager = GetComponentInChildren<EventsManager>(),
			endingsManager = GetComponentInChildren<EndingsManager>()
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
		saveManager = PersistentManager.Instance.saveManager;
		soundManager = PersistentManager.Instance.soundManager;

		databaseManager = GetComponentInChildren<DatabaseManager>();
		uiManager = GetComponentInChildren<UIManager>();
		gameStateManager = GetComponentInChildren<GameStateManager>();
		varsManager = GetComponentInChildren<VarsManager>();
		charactersManager = GetComponentInChildren<CharactersManager>();
		eventsManager = GetComponentInChildren<EventsManager>();
		endingsManager = GetComponentInChildren<EndingsManager>();

		InitGame();
	}

	private async void InitGame ()
	{
		InitSounds();

		await InitDatabase();
		InitVars();
		InitCharacters();
		InitEvents();
		InitEndings();

		await InitPersistentData(); // load save
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

	private void ApplySave ()
	{
		varsManager.ApplySave();
		charactersManager.ApplySave();
		eventsManager.ApplySave();
		endingsManager.ApplySave();
	}

	private async void StartGame ()
	{
		saveManager.AddToSaveData(SaveItemKey.RunStarted, true);

		await saveManager.SaveData();

		GameState startState = saveManager.GetSaveData<GameState>(SaveItemKey.State);

		if (startState == GameState.None)
			startState = initialState;

		gameStateManager.SetState(startState);
	}

}
