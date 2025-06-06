using UnityEngine;

public class DailyReportController
{
	protected DatabaseController database;
	public int minGrowth;
	public int maxGrowth;

	public void Init (DatabaseController database, int minGrowth, int maxGrowth)
	{
		this.database = database;
		this.minGrowth = minGrowth;
		this.maxGrowth = maxGrowth;
	}

	public bool IsEnoughFood ()
	{
		int population = database.GetVarData(GameVarId.Population).currentValue;
		int food = database.GetVarData(GameVarId.Food).currentValue;

		return food >= population;
	}

	public int GetFoodConsuption ()
	{
		int population = database.GetVarData(GameVarId.Population).currentValue;
		int food = database.GetVarData(GameVarId.Food).currentValue;

		return population > food ? -food : -population;
	}

	public int GetPopulationGrowth ()
	{
		VarData qolData = database.GetVarData(GameVarId.QoL);
		int population = database.GetVarData(GameVarId.Population).currentValue;
		float percent = MathUtils.Remap(qolData.minValue, qolData.maxValue, minGrowth, maxGrowth, qolData.currentValue) * 0.01f;

		return Mathf.RoundToInt(population * percent);
	}

	public int GetPopulationLossByFood ()
	{
		int population = database.GetVarData(GameVarId.Population).currentValue;
		int food = database.GetVarData(GameVarId.Food).currentValue;

		return population > food ? population - food : 0;
	}

}
