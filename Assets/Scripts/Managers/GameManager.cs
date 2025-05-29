using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public GameState initialState;

	[HideInInspector] public DatabaseManager databaseManager;
	[HideInInspector] public SaveManager saveManager;
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
		saveManager = GetComponentInChildren<SaveManager>();
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

		saveManager.Init();

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
		await saveManager.LoadData();

		varsManager.ApplySave();
		charactersManager.ApplySave();
		eventsManager.ApplySave();
		endingsManager.ApplySave();
	}

	private void InitState ()
	{
		GameState startState = saveManager.GetSaveData<GameState>(SaveItemKey.State);

		if (startState == GameState.None)
			startState = initialState;

		gameStateManager.SetState(startState);
	}

}
