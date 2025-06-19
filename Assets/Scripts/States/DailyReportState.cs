using UnityEngine;

public class DailyReportState : StateCommand
{
	public int minGrowth = -20;
	public int maxGrowth = 30;

	public override GameState state => GameState.DailyReport;

	private (GameVarId, int)[] production;

	public override void StartCommand (GameState previousState)
	{
		ShowDailyReport();
	}

	private async void ShowDailyReport ()
	{
		production = GameManager.Instance.productionManager.GetProduction();

		// first generate data with old resources value
		DailyReportPanelUIData panelData = FormatReportPanelTexts();

		// save state
		await GameManager.Instance.saveManager.SaveData();

		GameManager.Instance.uiManager.ShowReportPanel(panelData, () => EndReport());
	}

	private void EndReport ()
	{
		// then updates resources value
		ApplyResult();

		if (GameManager.Instance.endingsManager.CheckLose() != null)
		{
			// no need save
			EndCommand(GameState.EndGame);
			return;
		}

		CheckWin();
	}

	private async void CheckWin ()
	{
		EndingData endingData = GameManager.Instance.endingsManager.CheckWin();

		if (endingData != null)
		{
			await endingData.UpdateEnterSceneEffects();
			GameManager.Instance.endingsManager.ShowWin(() => End());
			await endingData.UpdateExitSceneEffects();
		}
		else
			End();
	}

	private void End ()
	{
		// increment day
		GameManager.Instance.varsManager.AddValueToVar(GameVarId.Day, 1);

		EndCommand(GameState.StartDay);
	}

	private void ApplyResult ()
	{
		int foodConsuption = GetFoodConsuption();
		int populationGrowth = GetPopulationGrowth() - GetPopulationLossByFood();

		foreach ((GameVarId varId, int value) in production)
			GameManager.Instance.varsManager.AddValueToVar(varId, value);

		GameManager.Instance.varsManager.AddValueToVar(GameVarId.Food, foodConsuption);
		GameManager.Instance.varsManager.AddValueToVar(GameVarId.Population, populationGrowth);
	}

	private DailyReportPanelUIData FormatReportPanelTexts ()
	{
		DailyReportPanelUIData panelData = new();

		int currentDay = GameManager.Instance.varsManager.GetVarValue(GameVarId.Day);

		panelData.dayValue = currentDay;
		(panelData.foodKey, panelData.foodValue) = GetFoodDiff();
		(panelData.qolKey, panelData.qolValue) = GetPopulationDiff();
		panelData.production = production;

		return panelData;
	}

	private (string, int) GetFoodDiff ()
	{
		int population = GameManager.Instance.varsManager.GetVarValue(GameVarId.Population);
		int food = GetTotalFoodAvailable();
		int populationLoss = GetPopulationLossByFood();

		if (population > food)
			return ("UI_REPORT_FOOD_NOT_ENOUGH", populationLoss);
		else
			return ("UI_REPORT_FOOD_ENOUGH", population);
	}

	private (string, int) GetPopulationDiff ()
	{
		int populationGrowth = GetPopulationGrowth();


		if (populationGrowth > 0)
			return ("UI_REPORT_QOL_ENOUGH", populationGrowth);
		else if (populationGrowth == 0)
			return ("UI_REPORT_QOL_MEDIAN", 0);
		else
			return ("UI_REPORT_QOL_NOT_ENOUGH", populationGrowth);
	}

	private int GetFoodConsuption ()
	{
		int population = GameManager.Instance.varsManager.GetVarValue(GameVarId.Population);
		int food = GetTotalFoodAvailable();

		if (population > food)
			return -food;
		else
			return -population;
	}

	private int GetPopulationGrowth ()
	{
		VarData qolData = GameManager.Instance.varsManager.GetVarData(GameVarId.QoL);
		int population = GameManager.Instance.varsManager.GetVarValue(GameVarId.Population);
		float percent = MathUtils.Remap(qolData.minValue, qolData.maxValue, minGrowth, maxGrowth, qolData.currentValue) * 0.01f;

		return Mathf.RoundToInt(population * percent);
	}

	private int GetPopulationLossByFood ()
	{
		int population = GameManager.Instance.varsManager.GetVarValue(GameVarId.Population);
		int food = GetTotalFoodAvailable();

		if (population > food)
			return population - food;

		return 0;
	}

	private int GetTotalFoodAvailable ()
	{
		int currentFood = GameManager.Instance.varsManager.GetVarValue(GameVarId.Food);
		int productedFood = 0;

		foreach ((GameVarId varId, int value) in production)
		{
			if (varId == GameVarId.Food)
			{
				productedFood = value;
				break;
			}
		}

		return currentFood + productedFood;
	}

}
