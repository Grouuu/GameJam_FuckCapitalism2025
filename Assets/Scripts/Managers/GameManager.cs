using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public GameState startState;
	public ResourceData[] startResources;

	[HideInInspector] public DatabaseManager databaseManager;
	[HideInInspector] public UIManager uiManager;
	[HideInInspector] public GameStateManager gameStateManager;
	[HideInInspector] public ResourcesManager resourcesManager;
	[HideInInspector] public CharactersManager charactersManager;
	[HideInInspector] public EventsManager eventsManager;

	private void Awake ()
	{
		if (Instance == null)
			Instance = this;

		databaseManager = GetComponentInChildren<DatabaseManager>();
		uiManager = GetComponentInChildren<UIManager>();
		gameStateManager = GetComponentInChildren<GameStateManager>();
		resourcesManager = GetComponentInChildren<ResourcesManager>();
		charactersManager = GetComponentInChildren<CharactersManager>();
		eventsManager = GetComponentInChildren<EventsManager>();
	}

	private void Start ()
	{
		InitGame();
	}

	private async void InitGame ()
	{
		await InitDatabase();
		await InitSave();

		InitResources();
		InitCharacters();

		// Start game loop
		InitState();
	}

	private async Awaitable InitDatabase ()
	{
		await databaseManager.LoadDatabase();
	}

	private async Awaitable InitSave ()
	{
		// TODO
		await Awaitable.WaitForSecondsAsync(0);
	}

	private void InitResources ()
	{
		// TODO get data from save if available

		resourcesManager.SetResourcesValue(startResources);
	}

	private void InitCharacters ()
	{
		// TODO get data from save if available
	}

	private void InitState ()
	{
		// TODO get data from save if available

		gameStateManager.SetState(startState);
	}

}
