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

/**
 * When update this, update also ParsingUtils.MapServerVarId
 * DO NOT CHANGE THE ORDER, otherwise it will break saves
 */
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
	RemainingPopulationRoom,
	Chantara_Ship_State,
	Trader_Ship_State,

	// Tracking type
	ControlFreeWill,
	UtilitarismEmpathy,
	TradeCount,
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
	Bugz_Progress,
	AttackStatus,
	Voices_Progress,
	Tweak_Progress,
	Mara_Progress,
	RandomEventGeneratorMax,
	Celestis_Awareness,
	Chantara_Mission_Progress,
	Chantara_Progress,
	Cook_Progress,
	SpaceDebrisLength,
	Habitat_Building_Gate,
	Habitat_Building_Progress,
	Station_Repair_Gate,
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

public class VarsManager : MonoBehaviour
{
	public bool debug = false;

	[NonSerialized] private VarData[] _gameVars = new VarData[0];
	[NonSerialized] private Dictionary<GameVarId, int> _startDayResources = new();

	public void InitVars (VarData[] vars)
	{
		if (vars == null)
		{
			Debug.LogError($"No vars to init");
			return;
		}

		_gameVars = vars
			.Where(entry => entry.varId != GameVarId.None)
			.ToArray()
		;

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

			UpdateStartDayResource(id, value);
		}

		UpdateSaveData();
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

	public void SetValueToVar (GameVarId id, int value, bool ignoreSave = false, bool ignoreCheckVars = false)
	{
		if (id == GameVarId.None)
			return;

		VarData varData = GetVarData(id);

		if (varData == null)
			return;

		// only UIVars have legit max values
		if (varData.type == GameVarType.UIVar)
		{
			// clamp
			value = Mathf.Max(value, varData.minValue);
			value = Mathf.Min(value, varData.maxValue);
		}

		if (debug)
			Debug.Log($"{id} is now {value} (old: {varData.currentValue})");

		varData.currentValue = value;

		UpdateUIValue(varData);

		if (!ignoreCheckVars)
			UpdateCheckVars(varData);

		if (!ignoreSave)
			UpdateSaveData();
	}

	public void SetOldValueToVar (GameVarId id, int oldValue)
	{
		if (id == GameVarId.None)
			return;

		VarData varData = GetVarData(id);

		if (varData == null)
			return;

		varData.previousValue = oldValue;
	}

	public void SetValueMax (GameVarId id, int maxValue)
	{
		if (id == GameVarId.None)
			return;

		VarData varData = GetVarData(id);

		if (varData == null)
			return;

		varData.maxValue = maxValue;

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

		return varData.IsLow();
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

	public VarData[] GetAllVars ()
	{
		return _gameVars;
	}

	public VarData GetVarData (GameVarId id)
	{
		return _gameVars.FirstOrDefault(data => data.varId == id);
	}

	public bool IsResultSafe (ResultData result)
	{
		ResultVarChange[] varChanges = result.varChanges;

		if (varChanges == null)
			return true;

		foreach (ResultVarChange varChange in varChanges)
		{
			VarData varData = GetVarData(varChange.varId);

			if (varData == null)
			{
				Debug.LogWarning($"Var data null for varId: {varChange.varId} (from IsResultSafe)");
				continue;
			}

			if (varData.type != GameVarType.UIVar)
				continue;

			int currentValue = varData.currentValue;
			int minValue = varData.minValue;
			int maxValue = varData.maxValue;

			int lowestModifier = Mathf.Min(varChange.modifierValueMin, varChange.modifierValueMax);
			int higherModifier = Mathf.Max(varChange.modifierValueMin, varChange.modifierValueMax);

			bool endsTooLow = currentValue + lowestModifier < minValue;
			bool endsTooHigh = currentValue + higherModifier > maxValue;

			if (endsTooLow || endsTooHigh)
				return false;
		}

		return true;
	}

	public void UpdateSaveData ()
	{
		List<(GameVarId, int, int, int)> varsValue = new();
		List<KeyValuePair<GameVarId, int>> startDayVarsValue = new();

		foreach (VarData varData in _gameVars)
		{
			// do not save CheckVars
			if (varData.type != GameVarType.CheckVar)
				varsValue.Add((varData.varId, varData.currentValue, varData.previousValue, varData.maxValue));
		}

		foreach (KeyValuePair<GameVarId, int> varData in _startDayResources)
			startDayVarsValue.Add(varData);

		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.VarsValue, varsValue);
		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.StartDayVarsValues, startDayVarsValue);
	}

	public void ApplySave ()
	{
		List < (GameVarId, int, int, int)> varsValue = GameManager.Instance.saveManager.GetSaveData<List<(GameVarId, int, int, int)>>(SaveItemKey.VarsValue);
		List<KeyValuePair<GameVarId, int>> startDayVarsValue = GameManager.Instance.saveManager.GetSaveData<List<KeyValuePair<GameVarId, int>>>(SaveItemKey.StartDayVarsValues);

		if (varsValue != null)
		{
			foreach ((GameVarId varId, int value, int oldValue, int max) in varsValue)
			{
				SetValueToVar(varId, value, true);
				SetOldValueToVar(varId, oldValue);
				SetValueMax(varId, max);
			}
		}

		if (startDayVarsValue != null)
		{
			foreach ((GameVarId varId, int value) in startDayVarsValue)
				UpdateStartDayResource(varId, value);
		}
	}

	private void UpdateStartDayResource(GameVarId id, int value)
	{
		if (_startDayResources.ContainsKey(id))
			_startDayResources[id] = value;
		else
			_startDayResources.Add(id, value);
	}

	private void UpdateUIValue (VarData varData)
	{
		if (varData.type == GameVarType.UIVar)
		{
			GameManager.Instance.uiManager.SetResourceValue(varData.varId, varData.currentValue, varData.maxValue);

			if (varData.lowThreshold != null)
			{
				bool isLow = varData.lowThreshold.IsValueOK(varData.currentValue);
				GameManager.Instance.uiManager.ShowResourceLowWarning(varData.varId, isLow);
			}
		}
	}

	private void UpdateCheckVars(VarData varData)
	{
		if (varData.type == GameVarType.CheckVar)
			return;

		// do not trigger any save from here, the saveManager is not ready at start

		if (varData.varId == GameVarId.Food || varData.varId == GameVarId.Population)
		{
			int food = GetVarValue(GameVarId.Food);
			int population = GetVarValue(GameVarId.Population);
			SetValueToVar(GameVarId.FoodAfterConsumption, food - population, true, true);
		}
		
		if (varData.varId == GameVarId.Population)
		{
			VarData populationData = GetVarData(GameVarId.Population);
			SetValueToVar(GameVarId.RemainingPopulationRoom, populationData.maxValue - populationData.currentValue, true, true);
		}
	}

}
