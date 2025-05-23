using System;
using System.Linq;
using UnityEngine;

public enum GameVarType
{
	None,
	UIVar,
	CheckVar,
	TrackingVar,
}

public enum GameVarId
{
	None,

	// UI vars
	Population,
	QoL,
	Trust,
	Food,
	Scraps,
	Power,
	Science,
	Fleet,
	Weapons,
	StationHP,
	Modules,
	Day, 

	// CheckType
	FoodAfterConsumption,
	NextBattlePreview,

	// Tracking type
	ControlFreeWill,
	UtilitarismEmpathy,
	TraderCount,
	PetLover,
	TrustGateMaxReached,
	FoodProduction,
	ScrapsProduction,
	ScienceProduction,
	TrustGate1,
	TrustGate2,
	TrustGate3,
	TrustGate4,
	TrustGate5,
	TrustGate6,
	HPachievments,
	FirstBuilding,
	SixBuildingsTotal,
	FirstRing,
	IsNewcomersActive,
	HasFirstDialogPlayed,
	IsLabBuilt,
	BuildingCountLab,
	BuildingCountHabitat,
	BuildingCountHydroponic,
	BuildingCountFusionReactor,
	BuildingCountCannons,
	BuildingCountEntertainment,
	BuildingCountFactory,
	BuildingCountSensors,
	BuildingCountRepairDrones,
	HasWonGame,
}

public enum CompareValueType
{
	None,
	Equal,
	Less,
	More,
	LessEqual,
	MoreEqual
}

public enum ChangeValueType
{
	None,
	Set,
	Add,
}

public class VarData
{
	public string id;
	public GameVarId name;
	public GameVarType type;
	public string displayName;
	public int startValue;
	public int minValue;
	public int maxValue;
	public int prodPerCycleMin;
	public int prodPerCycleMax;
	public VarCompareValue lowThreshold;

	public int currentValue;

	public int GetRandomProductionYield ()
	{
		if (prodPerCycleMax == 0 || prodPerCycleMin == prodPerCycleMax)
			return prodPerCycleMin;

		return UnityEngine.Random.Range(prodPerCycleMin, prodPerCycleMax + 1);
	}

	public bool isLow ()
	{
		if (lowThreshold == null)
			return false;

		int checkedValue = GameManager.Instance.varsManager.GetVarValue(lowThreshold.varId);

		return lowThreshold.IsValueOK(checkedValue);
	}
}

public class VarCompareValue
{
	public GameVarId varId = GameVarId.None;
	public CompareValueType checkType = CompareValueType.None;
	public int compareValue;

	public bool IsValueOK (int value)
	{
		return GetCompareFunc()(value);
	}

	private Func<int, bool> GetCompareFunc ()
	{
		return checkType switch
		{
			CompareValueType.Equal =>		valueToCheck => valueToCheck == compareValue,
			CompareValueType.Less =>		valueToCheck => valueToCheck < compareValue,
			CompareValueType.More =>		valueToCheck => valueToCheck > compareValue,
			CompareValueType.LessEqual =>	valueToCheck => valueToCheck <= compareValue,
			CompareValueType.MoreEqual =>	valueToCheck => valueToCheck >= compareValue,
			_ =>							valueToCheck => true,
		};
	}
}

public class VarsManager : MonoBehaviour
{
	[NonSerialized] private VarData[] _gameVars = new VarData[0];

	public void InitVars (VarData[] vars)
	{
		if (vars == null)
		{
			Debug.LogError($"No vars to init");
			return;
		}

		_gameVars = vars;

		foreach (VarData varData in _gameVars)
			UpdateUIValue(varData);
	}

	public int GetVarValue (GameVarId id)
	{
		VarData varData = GetVarData(id);

		if (varData == null)
			return 0;

		return varData.currentValue;
	}

	public void AddValueToVar (GameVarId id, int value)
	{
		VarData varData = GetVarData(id);

		if (varData == null)
			return;

		SetValueToVar(varData.name, varData.currentValue + value);
	}

	public void SetValueToVar (GameVarId id, int value)
	{
		if (id == GameVarId.None)
			return;

		VarData varData = GetVarData(id);

		if (varData == null)
			return;

		varData.currentValue = value;

		UpdateUIValue(varData);
	}

	public bool IsVarLow (GameVarId id)
	{
		if (id == GameVarId.None)
			return false;

		VarData varData = GetVarData(id);

		if (varData == null)
			return false;

		return varData.isLow();
	}

	public string GetVarDisplayName (GameVarId id)
	{
		VarData varData = GetVarData(id);

		if (varData != null)
			return varData.displayName;

		return "";
	}

	private VarData GetVarData (GameVarId id)
	{
		return _gameVars.First(data => data.name == id);
	}

	private void UpdateUIValue (VarData varData)
	{
		if (varData.type == GameVarType.UIVar)
			GameManager.Instance.uiManager.SetResourceValue(varData.name, varData.currentValue);
	}

}
