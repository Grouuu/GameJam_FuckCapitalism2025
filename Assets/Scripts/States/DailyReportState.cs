using System.Collections.Generic;
using UnityEngine;

public class DailyReportState : StateCommand
{
	public int minGrowth = -20;
	public int maxGrowth = 30;

	public override void StartCommand ()
	{
		ShowDailyReport();
	}

	private void OnEnable () => state = GameState.DailyReport;

	private void ShowDailyReport ()
	{
		ReportPanelUIData panelData = FormatReportPanelTexts();
		GameManager.Instance.uiManager.ShowReportPanel(panelData, () => OnContinue());
	}

	private void OnContinue ()
	{
		EndCommand();
	}

	private ReportPanelUIData FormatReportPanelTexts ()
	{
		ReportPanelUIData panelData = new();

		int currentDay = GameManager.Instance.varsManager.GetVarValue(GameVarId.Day);

		panelData.dayLabel = "DAY"; // translate
		panelData.dayValue = currentDay;
		panelData.description = "The sensors have gathered the following variations:"; // translate
		panelData.resourcesChange = GetResourceValueDiff();
		panelData.foodChange = GetFoodDiff();
		panelData.populationChange = GetPopulationDiff();

		// TODO update the food/population vars data

		return panelData;
	}

	private (string iconFileName, int valueDiff)[] GetResourceValueDiff ()
	{
		List<(string iconFileName, int valueDiff)> resourceDiff = new();
		Dictionary<GameVarId, int> startDayValues = GameManager.Instance.varsManager.GetStartDayResourcesValue();
		VarData[] endDayValues = GameManager.Instance.varsManager.GetResourcesData();

		foreach (VarData resourceData in endDayValues)
		{
			int diff = 0;

			if (startDayValues.TryGetValue(resourceData.varId, out int oldValue))
				diff = resourceData.currentValue - oldValue;

			if (diff != 0)
				resourceDiff.Add((resourceData.iconFileName, diff));
		}

		return resourceDiff.ToArray();
	}

	private string GetFoodDiff ()
	{
		int population = GameManager.Instance.varsManager.GetVarValue(GameVarId.Population);
		int food = GameManager.Instance.varsManager.GetVarValue(GameVarId.Food);

		if (population > food)
			return $"Not enough food: {population - food} <sprite name=\"Pop\"> left"; // translate
		else
			return $"Everyone is fed: +{food - population} <sprite name=\"Food\">"; // translate
	}

	private string GetPopulationDiff ()
	{
		int populationGrowth = GetPopulationGrowth();

		if (populationGrowth > 0)
			return $"Quality of life is high: +{populationGrowth} <sprite name=\"Pop\">"; // translate
		else if (populationGrowth == 0)
			return $"No newcomers"; // translate
		else
			return $"Quality of life is low: {populationGrowth} <sprite name=\"Pop\">"; // translate
	}

	private int GetFoodConsuption ()
	{
		int population = GameManager.Instance.varsManager.GetVarValue(GameVarId.Population);
		int food = GameManager.Instance.varsManager.GetVarValue(GameVarId.Food);

		return food - population;
	}

	private int GetPopulationGrowth ()
	{
		int qol = GameManager.Instance.varsManager.GetVarValue(GameVarId.QoL);
		return Mathf.RoundToInt(MathUtils.Remap(0, 100, minGrowth, maxGrowth, qol));
	}

}
