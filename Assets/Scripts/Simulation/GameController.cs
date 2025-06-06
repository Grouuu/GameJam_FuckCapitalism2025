using UnityEngine;

public class GameController
{
	public DatabaseController database = new();
	public PickDialog pickDialog = new();
	public PickEvent pickEvent = new();
	public DailyReportController dailyReport = new();
	public GameOverController gameOver = new();

	protected Managers managers;
	protected StateController states = new();

	public async Awaitable Init (Managers managers)
	{
		this.managers = managers;

		await database.LoadDatabase(managers.databaseManager.Parsers, managers.databaseManager.FolderPath);

		states.Init(managers.gameStateManager.GetStatesName(), GameState.StartDay);
		pickDialog.Init(database, managers.gameManager.maxDialogsByDay);
		pickEvent.Init(database);
		dailyReport.Init(database, managers.gameManager.minPopulationGrowth, managers.gameManager.maxDialogsByDay);
		gameOver.Init(database);
	}

	public GameState NextState ()
	{
		GameState state = states.NextState();

		if (gameOver.CheckLose())
			state = GameState.EndGame;
		else if (states.State == GameState.DailyReport)
			state = GameState.StartDay;

		states.SetState(state);

		return state;
	}

}
