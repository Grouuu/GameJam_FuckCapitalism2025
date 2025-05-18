using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public GameState startState;
	public ResourceData[] startResources;

	[HideInInspector] public CharactersManager charactersManager;
	[HideInInspector] public ResourcesManager resourcesManager;
	[HideInInspector] public EventsManager eventsManager;
	[HideInInspector] public UIManager uiManager;
	[HideInInspector] public GameStateManager gameStateManager;

	private void Awake ()
	{
		if (Instance == null)
			Instance = this;

		charactersManager = GetComponentInChildren<CharactersManager>();
		resourcesManager = GetComponentInChildren<ResourcesManager>();
		eventsManager = GetComponentInChildren<EventsManager>();
		uiManager = GetComponentInChildren<UIManager>();
		gameStateManager = GetComponentInChildren<GameStateManager>();
	}

	private void Start ()
	{
		InitResources();
		InitCharacters();

		// Start game loop
		InitState();
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
