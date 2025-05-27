using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public GameState startState;

	[HideInInspector] public DatabaseManager databaseManager;
	[HideInInspector] public UIManager uiManager;
	[HideInInspector] public GameStateManager gameStateManager;
	[HideInInspector] public VarsManager varsManager;
	[HideInInspector] public CharactersManager charactersManager;
	[HideInInspector] public EventsManager eventsManager;
	[HideInInspector] public EndingsManager endingsManager;

	private void Awake ()
	{
		if (Instance == null)
			Instance = this;

		databaseManager = GetComponentInChildren<DatabaseManager>();
		uiManager = GetComponentInChildren<UIManager>();
		gameStateManager = GetComponentInChildren<GameStateManager>();
		varsManager = GetComponentInChildren<VarsManager>();
		charactersManager = GetComponentInChildren<CharactersManager>();
		eventsManager = GetComponentInChildren<EventsManager>();
		endingsManager = GetComponentInChildren<EndingsManager>();
	}

	private void Start ()
	{
		InitGame();
	}

	private async void InitGame ()
	{
		await InitDatabase();

		InitVars();
		InitCharacters();
		InitEvents();
		InitEndings();

		await LoadSave();

		// Start game loop
		InitState();
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

	private async Awaitable LoadSave ()
	{
		// TODO
		await Awaitable.WaitForSecondsAsync(0);
	}

	private void InitState ()
	{
		// TODO get data from save if available

		gameStateManager.SetState(startState);
	}

}
