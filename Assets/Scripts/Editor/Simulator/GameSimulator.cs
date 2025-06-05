using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GameSimulator : EditorWindow
{
	[Serializable]
	public class VarRow
	{
		public string name;
		public int value;
	}

	[Serializable]
	public class EventRow
	{
		public string name;
		public EventType type;
		public int priority;
		public int randomWeight;
		public int day = -1;
		public bool isUsed = false;
	}

	private GameManager _gameManager;
	private DatabaseManager _database;
	private VarsManager _vars;
	private CharactersManager _characters;
	private EventsManager _events;
	private EndingsManager _endings;
	private PlayDialogState _dialogsState;
	private DailyReportState _dailyState;

	private VisualElement _varsContainer;
	private VisualElement _contentContainer;

	private List<VarRow> _varRows;
	private ListView _varsTable;
	private List<EventRow> _eventRows;
	private ListView _eventsTable;

	private GameState _currentState = GameState.PlayEvent;

	//[MenuItem("Debug/Game simulation")]
	//private static void ShowWindow () => GetWindow<GameSimulator>("Game Simulator");

	private void OnGUI ()
	{
		if (_gameManager == null)
		{
			EditorGUILayout.TextField("No game manager found.");
			return;
		}

		if (_currentState == GameState.PlayEvent)
			ShowEvents();
	}

	private void ShowEvents ()
	{
		if (GUILayout.Button("Pick event"))
		{

		}
	}

	private void OnEnable ()
	{
		//_gameManager = FindAnyObjectByType<GameManager>();

		//if (_gameManager != null)
		//	InitSimulator();
	}

	private async void InitSimulator ()
	{
		await InitData();
		InitUI();
	}

	private async Awaitable InitData ()
	{
		// TODO
		// need:
		// . _database
		//		. chars setup with dialogs
		// . pickEvent
		// . pickDialog
		// . daily calculation
		// . params dialogs state
		// . params daily state
		// . checkWin/Lose

		// NEED
		// . database loading
		// . build char data with dialogData
		// . set used
		// . set days
		// . generate result random
		// . pickEvent
		// . pickDialog
		// . daily results
		// . check win/lose

		// . Database
		//		. load
		//		. set Vars/Chars+Dialogs/Events/Endings data
		// . GameLogic
		//		. flag dialogs/events/endings as used
		//		. edit events day
		//		. update random result values
		//		. pick dialogs (local charsUsed, dialogs played, max dialogs)
		//		. pick events (local eventsUsed, randomEvent played)
		//		. set daily results
		//		. check win/lose

		_database = _gameManager.gameObject.GetComponentInChildren<DatabaseManager>();
		_vars = _gameManager.gameObject.GetComponentInChildren<VarsManager>();
		_characters = _gameManager.gameObject.GetComponentInChildren<CharactersManager>();
		_events = _gameManager.gameObject.GetComponentInChildren<EventsManager>();
		_endings = _gameManager.gameObject.GetComponentInChildren<EndingsManager>();
		_dialogsState = _gameManager.gameObject.GetComponentInChildren<PlayDialogState>();
		_dailyState = _gameManager.gameObject.GetComponentInChildren<DailyReportState>();

		_database.Init();
		await _database.LoadDatabase();

		_vars.InitVars(_database.GetData<VarData>());
		_characters.InitCharacters(_database.GetData<CharacterData>(), _database.GetData<DialogData>());
		_events.InitEvents(_database.GetData<EventData>());
		_endings.InitEndings(_database.GetData<EndingData>());
	}

	private void InitUI ()
	{
		_varsContainer = new VisualElement();
		rootVisualElement.Add(_varsContainer);

		_contentContainer = new VisualElement();
		rootVisualElement.Add(_contentContainer);

		CreateVarsTable();
	}

	private void CreateVarsTable ()
	{
		_varRows = new();

		foreach (VarData varData in _vars.GetAllVars())
		{
			VarRow row = new()
			{
				name = ParsingUtils.MapVars[varData.varId],
				value = varData.currentValue,
			};

			_varRows.Add(row);
		}

		_varsTable = new();
		_varsTable.itemsSource = _varRows;
		_varsTable.fixedItemHeight = 30;
		_varsTable.style.flexGrow = 1;

		_varsTable.makeItem = () =>
		{
			var container = new VisualElement();
			container.style.flexDirection = FlexDirection.Row;
			container.style.alignItems = Align.Center;
			container.style.paddingLeft = 5;
			container.style.paddingRight = 5;
			container.style.paddingTop = 5;
			container.style.paddingBottom = 5;

			var nameLabel = new Label();
			nameLabel.style.fontSize = 14;
			nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
			nameLabel.style.width = 100;

			var valueLabel = new Label();
			nameLabel.style.fontSize = 14;
			nameLabel.style.width = 100;

			container.Add(nameLabel);
			container.Add(valueLabel);

			return container;
		};

		_varsTable.bindItem = (element, index) =>
		{
			var varData = _varRows[index];
			var labels = element.Children().Cast<Label>().ToArray();

			labels[0].text = varData.name;
			labels[1].text = $"{varData.value}";
		};

		_varsContainer.Add(_varsTable);
	}

	private void CreateEventsTable ()
	{
		_eventRows = new();

		foreach (EventData eventData in _events.GetEvents())
		{
			EventRow row = new()
			{
				name = eventData.name,
				type = eventData.type,
				priority = eventData.priority,
				randomWeight = eventData.randomWeight,
				day = eventData.day,
				isUsed = eventData.isUsed
			};

			_eventRows.Add(row);
		}
	}

	private void ShowView (GameState state)
	{
		_currentState = state;

		_contentContainer.Clear();

		ListView view = null;

		switch (state)
		{
			case GameState.PlayEvent: view = _eventsTable; break;
		}

		if (view != null)
			_contentContainer.Add(view);
	}

}
