using Grouuu.Data;
using Grouuu.Enum;
using Grouuu.UI;
using Grouuu.Utils;
using UnityEngine;

namespace Grouuu.States
{
	public class DailyReportState : StateCommand
	{
		public int minGrowth = -20;
		public int maxGrowth = 30;

		public override GameState State => GameState.DailyReport;

		private (GameVarId, int)[] production;

		public override void StartCommand (GameState previousState)
		{
			ShowDailyReport();
		}

		private async void ShowDailyReport ()
		{
			production = GameController.GameManagers.ProductionManager.GetProduction();

			// first generate data with old resources value
			DailyReportPanelUIData panelData = FormatReportPanelTexts();

			// save state
			await GameController.GameManagers.SaveManager.SaveData();

			GameController.GameManagers.UiManager.ShowReportPanel(panelData, () => EndReport());
		}

		private void EndReport ()
		{
			// then updates resources value
			ApplyResult();

			if (GameController.GameManagers.EndingsManager.CheckLose() != null)
			{
				// no need save
				base.EndCommand(GameState.EndGame);
				return;
			}

			CheckWin();
		}

		private async void CheckWin ()
		{
			EndingData endingData = GameController.GameManagers.EndingsManager.CheckWin();

			if (endingData != null)
			{
				await endingData.UpdateEnterSceneEffects();
				GameController.GameManagers.EndingsManager.ShowWin(() => End());
				await endingData.UpdateExitSceneEffects();
			}
			else
				End();
		}

		private void End ()
		{
			// increment day
			GameController.GameManagers.VarsManager.AddValueToVar(GameVarId.Day, 1);

			EndCommand(GameState.StartDay);
		}

		private void ApplyResult ()
		{
			int foodConsuption = GetFoodConsuption();
			int populationGrowth = GetPopulationGrowth() - GetPopulationLossByFood();

			foreach ((GameVarId varId, int value) in production)
				GameController.GameManagers.VarsManager.AddValueToVar(varId, value);

			GameController.GameManagers.VarsManager.AddValueToVar(GameVarId.Food, foodConsuption);
			GameController.GameManagers.VarsManager.AddValueToVar(GameVarId.Population, populationGrowth);
		}

		private DailyReportPanelUIData FormatReportPanelTexts ()
		{
			DailyReportPanelUIData panelData = new();

			int currentDay = GameController.GameManagers.VarsManager.GetVarValue(GameVarId.Day);

			panelData.dayValue = currentDay;
			(panelData.foodKey, panelData.foodValue) = GetFoodDiff();
			(panelData.qolKey, panelData.qolValue) = GetPopulationDiff();
			panelData.production = production;

			return panelData;
		}

		private (string, int) GetFoodDiff ()
		{
			int population = GameController.GameManagers.VarsManager.GetVarValue(GameVarId.Population);
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
			int population = GameController.GameManagers.VarsManager.GetVarValue(GameVarId.Population);
			int food = GetTotalFoodAvailable();

			if (population > food)
				return -food;
			else
				return -population;
		}

		private int GetPopulationGrowth ()
		{
			VarData qolData = GameController.GameManagers.VarsManager.GetVarData(GameVarId.QoL);
			int population = GameController.GameManagers.VarsManager.GetVarValue(GameVarId.Population);
			float percent = MathUtils.Remap(qolData.minValue, qolData.maxValue, minGrowth, maxGrowth, qolData.currentValue) * 0.01f;

			return Mathf.RoundToInt(population * percent);
		}

		private int GetPopulationLossByFood ()
		{
			int population = GameController.GameManagers.VarsManager.GetVarValue(GameVarId.Population);
			int food = GetTotalFoodAvailable();

			if (population > food)
				return population - food;

			return 0;
		}

		private int GetTotalFoodAvailable ()
		{
			int currentFood = GameController.GameManagers.VarsManager.GetVarValue(GameVarId.Food);
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
}