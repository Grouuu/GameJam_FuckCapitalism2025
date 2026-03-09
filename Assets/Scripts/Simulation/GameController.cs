//using UnityEngine;

//public class GameController
//{
//	public DatabaseController database = new();
//	public PickDialog pickDialog = new();
//	public PickEvent pickEvent = new();
//	public DailyReportController dailyReport = new();
//	public GameOverController gameOver = new();

//	protected Managers managers;
//	protected StateController states = new();

//	public async Awaitable Init (Managers managers)
//	{
//		this.managers = managers;

//		await database.LoadDatabase(managers.DatabaseManager.Parsers, managers.DatabaseManager.FolderPath);

//		states.Init(managers.GameStateManager.GetStatesName(), GameState.StartDay);
//		pickDialog.Init(database, managers.GameManager.maxDialogsByDay);
//		pickEvent.Init(database);
//		dailyReport.Init(database, managers.GameManager.minPopulationGrowth, managers.GameManager.maxDialogsByDay);
//		gameOver.Init(database);
//	}

//	public GameState NextState ()
//	{
//		GameState state = states.NextState();

//		if (gameOver.CheckLose())
//			state = GameState.EndGame;
//		else if (states.State == GameState.DailyReport)
//			state = GameState.StartDay;

//		states.SetState(state);

//		return state;
//	}

//}
