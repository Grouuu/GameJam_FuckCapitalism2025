using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public GameState startState;
	public ResourceData[] startResources;

	[HideInInspector] public CharactersManager charactersManager;
	[HideInInspector] public ResourcesManager resourcesManager;
	[HideInInspector] public UIManager uiManager;
	[HideInInspector] public GameStateManager gameStateManager;

	private void Awake ()
	{
		if (Instance == null)
			Instance = this;

		charactersManager = GetComponentInChildren<CharactersManager>();
		resourcesManager = GetComponentInChildren<ResourcesManager>();
		uiManager = GetComponentInChildren<UIManager>();
		gameStateManager = GetComponentInChildren<GameStateManager>();
	}

	private void Start ()
	{
		InitResources();
		InitCharacters();
		InitState();
	}

	private void InitResources ()
	{
		// TODO get data from save if available

		resourcesManager.resources = startResources;
		uiManager.SetValues(resourcesManager.resources);
	}

	private void InitCharacters ()
	{
		// TODO get data from save if available

		charactersManager.UpdateAvailableCharacters();
	}

	private void InitState ()
	{
		// TODO get data from save if available

		gameStateManager.SetState(startState);
	}

}
