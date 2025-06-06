
public class GameOverController
{
	protected DatabaseController database;

	public void Init (DatabaseController database)
	{
		this.database = database;
	}

	public bool CheckWin ()
	{
		foreach (EndingData ending in database.endingsData)
		{
			if (!ending.isWinEnding || ending.isUsed)
				continue;

			if (ending.IsRespectRequirements())
				return true;
		}

		return false;
	}

	public bool CheckLose ()
	{
		foreach (EndingData ending in database.endingsData)
		{
			if (ending.isWinEnding)
				continue;

			if (ending.IsRespectRequirements())
				return true;
		}

		return false;
	}
}
