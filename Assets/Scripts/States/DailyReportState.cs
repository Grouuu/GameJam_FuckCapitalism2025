using UnityEngine;

public class DailyReportState : StateCommand
{
	public int minGrowth = -20;
	public int maxGrowth = 30;

	private (GameVarId, int)[] production;

	public override void StartCommand (GameState previousState)
	{
		ShowDailyReport();
	}

	private void OnEnable () => state = GameState.DailyReport;

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

		if (GameManager.Instance.endingsManager.CheckLose())
		{
			// no need save
			EndCommand(GameState.EndGame);
			return;
		}

		CheckWin();
	}

	private void CheckWin ()
	{
		if (GameManager.Instance.endingsManager.CheckWin())
			GameManager.Instance.endingsManager.ShowWin(() => AfterWin());
		else
			End();
	}

	private void AfterWin ()
	{
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
		panelData.foodChange = GetFoodDiff();
		panelData.populationChange = GetPopulationDiff();
		panelData.production = production;

		return panelData;
	}

	private string GetFoodDiff ()
	{
		int population = GameManager.Instance.varsManager.GetVarValue(GameVarId.Population);
		int food = GetTotalFoodAvailable();
		int populationLoss = GetPopulationLossByFood();

		if (population > food)
		{
			return LocalizationUtils.GetText("UI_REPORT_FOOD_NOT_ENOUGH", LocCat.UI).ReplaceValue("VALUE", $"{populationLoss}");
		}
		else
		{
			return LocalizationUtils.GetText("UI_REPORT_FOOD_ENOUGH", LocCat.UI).ReplaceValue("VALUE", $"{population}");
		}
	}

	private string GetPopulationDiff ()
	{
		int populationGrowth = GetPopulationGrowth();


		if (populationGrowth > 0)
		{
			return LocalizationUtils.GetText("UI_REPORT_QOL_ENOUGH", LocCat.UI).ReplaceValue("VALUE", $"{populationGrowth}");
		}
		else if (populationGrowth == 0)
		{
			return LocalizationUtils.GetText("UI_REPORT_QOL_MEDIAN", LocCat.UI);
		}
		else
		{
			return LocalizationUtils.GetText("UI_REPORT_QOL_NOT_ENOUGH", LocCat.UI).ReplaceValue("VALUE", $"{populationGrowth}");
		}
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
