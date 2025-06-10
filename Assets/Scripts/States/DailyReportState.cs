using System.Collections.Generic;
using UnityEngine;

public class DailyReportState : StateCommand
{
	public int minGrowth = -20;
	public int maxGrowth = 30;

	public override void StartCommand (GameState previousState)
	{
		ShowDailyReport();
	}

	private void OnEnable () => state = GameState.DailyReport;

	private async void ShowDailyReport ()
	{
		// first generate data with old resources value
		ReportPanelUIData panelData = FormatReportPanelTexts();

		await GameManager.Instance.saveManager.SaveData();

		GameManager.Instance.uiManager.ShowReportPanel(panelData, () => EndReport());
	}

	private void EndReport ()
	{
		// then updates resources value
		ApplyResult();

		if (GameManager.Instance.endingsManager.CheckLose())
		{
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

		// TODO add prod

		GameManager.Instance.varsManager.AddValueToVar(GameVarId.Food, foodConsuption);
		GameManager.Instance.varsManager.AddValueToVar(GameVarId.Population, populationGrowth);
	}

	private ReportPanelUIData FormatReportPanelTexts ()
	{
		ReportPanelUIData panelData = new();

		int currentDay = GameManager.Instance.varsManager.GetVarValue(GameVarId.Day);

		panelData.dayLabel = "DAY"; // translate
		panelData.dayValue = currentDay;
		panelData.description = "The sensors have gathered the following variations:"; // translate
		panelData.foodChange = GetFoodDiff();
		panelData.populationChange = GetPopulationDiff();

		return panelData;
	}

	private string GetFoodDiff ()
	{
		int population = GameManager.Instance.varsManager.GetVarValue(GameVarId.Population);
		int food = GameManager.Instance.varsManager.GetVarValue(GameVarId.Food);
		int populationLoss = GetPopulationLossByFood();

		if (population > food)
			return $"Not enough food: -{populationLoss} <sprite name=\"Population\"> left"; // translate
		else
			return $"Everyone is fed: -{population} <sprite name=\"Food\">"; // translate
	}

	private string GetPopulationDiff ()
	{
		int populationGrowth = GetPopulationGrowth();

		if (populationGrowth > 0)
			return $"Quality of life is high: +{populationGrowth} <sprite name=\"Population\">"; // translate
		else if (populationGrowth == 0)
			return $"No newcomers"; // translate
		else
			return $"Quality of life is low: {populationGrowth} <sprite name=\"Population\">"; // translate
	}

	private int GetFoodConsuption ()
	{
		int population = GameManager.Instance.varsManager.GetVarValue(GameVarId.Population);
		int food = GameManager.Instance.varsManager.GetVarValue(GameVarId.Food);

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
		int food = GameManager.Instance.varsManager.GetVarValue(GameVarId.Food);

		if (population > food)
			return population - food;

		return 0;
	}

	private List<(GameVarId, int)> GetProduction ()
	{
		List<(GameVarId, int)> production = new();

		int population = GameManager.Instance.varsManager.GetVarValue(GameVarId.Population);



		return production;
	}

}
