using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
	TraderStayLength,
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
	public GameVarId varId;
	public GameVarType type;
	public string displayName;
	public string iconFileName;
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

	public VarData Clone ()
	{
		var serialized = JsonConvert.SerializeObject(this);
		return JsonConvert.DeserializeObject<VarData>(serialized);
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
	private VarData[] _gameVars = new VarData[0];
	private Dictionary<GameVarId, int> _startDayResources = new();

	public void InitVars (VarData[] vars)
	{
		if (vars == null)
		{
			Debug.LogError($"No vars to init");
			return;
		}

		_gameVars = vars;

		foreach (VarData varData in _gameVars)
		{
			UpdateUIValue(varData);
			UpdateCheckVars(varData);
		}
	}

	public void SaveStartDayResourcesValue ()
	{
		foreach (VarData data in _gameVars)
		{
			if (data.type != GameVarType.UIVar)
				continue;

			GameVarId id = data.varId;
			int value = GetVarValue(data.varId);

			if (_startDayResources.ContainsKey(id))
				_startDayResources[id] = value;
			else
				_startDayResources.Add(id, value);
		}
	}

	public Dictionary<GameVarId, int> GetStartDayResourcesValue ()
	{
		return _startDayResources;
	}

	public int GetStartDayResourceValue (GameVarId id)
	{
		if (_startDayResources.TryGetValue(id, out int value))
			return value;

		return 0;
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

		SetValueToVar(varData.varId, varData.currentValue + value);
	}

	public void SetValueToVar (GameVarId id, int value)
	{
		if (id == GameVarId.None)
			return;

		VarData varData = GetVarData(id);

		if (varData == null)
			return;

		Debug.Log($"{id} is now {value} (old: {varData.currentValue})");

		varData.currentValue = value;

		UpdateUIValue(varData);
		UpdateCheckVars(varData);
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

	public VarData[] GetResourcesData ()
	{
		return _gameVars
			.Where(entry => entry.type == GameVarType.UIVar)
			.ToArray()
		;
	}

	public VarData GetVarData (GameVarId id)
	{
		return _gameVars.First(data => data.varId == id);
	}

	private void UpdateUIValue (VarData varData)
	{
		if (varData.type == GameVarType.UIVar)
			GameManager.Instance.uiManager.SetResourceValue(varData.varId, varData.currentValue);
	}

	private void UpdateCheckVars(VarData varData)
	{
		if (varData.type == GameVarType.CheckVar)
			return;

		if (varData.varId == GameVarId.Food || varData.varId == GameVarId.Population)
		{
			int food = GetVarValue(GameVarId.Food);
			int population = GetVarValue(GameVarId.Population);
			SetValueToVar(GameVarId.FoodAfterConsumption, food - population);
		}
	}

}
