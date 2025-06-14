using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ParsingUtils
{
	public static Dictionary<GameVarId, string> MapVars = new()
	{
		{ GameVarId.Population, "Population" },
		{ GameVarId.QoL, "QoL" },
		{ GameVarId.Trust, "Trust" },
		{ GameVarId.Food, "Food" },
		{ GameVarId.Scraps, "Scraps" },
		{ GameVarId.Power, "Power" },
		{ GameVarId.Science, "Science" },
		{ GameVarId.Fleet, "Fleet" },
		{ GameVarId.Weapons, "Weapons" },
		{ GameVarId.StationHP, "StationHP" },

		{ GameVarId.Modules, "Modules" },
		{ GameVarId.Day, "Day" },
		{ GameVarId.FoodAfterConsumption, "FoodAfterConsumption" },
		{ GameVarId.NextBattlePreview, "NextBattlePreview" },
		{ GameVarId.ControlFreeWill, "ControlFreeWill" },
		{ GameVarId.UtilitarismEmpathy, "UtilitarismEmpathy" },
		{ GameVarId.TradeCount, "TradeCount" },
		{ GameVarId.PetLover, "PetLover" },
		{ GameVarId.TrustGateMaxReached, "TrustGateMaxReached" },
		{ GameVarId.FoodProduction, "FoodProduction" },

		{ GameVarId.ScrapsProduction, "ScrapsProduction" },
		{ GameVarId.ScienceProduction, "ScienceProduction" },
		{ GameVarId.TrustGate1, "TrustGate1" },
		{ GameVarId.TrustGate2, "TrustGate2" },
		{ GameVarId.TrustGate3, "TrustGate3" },
		{ GameVarId.TrustGate4, "TrustGate4" },
		{ GameVarId.TrustGate5, "TrustGate5" },
		{ GameVarId.TrustGate6, "TrustGate6" },
		{ GameVarId.HPachievments, "HPachievments" },
		{ GameVarId.FirstBuilding, "FirstBuilding" },

		{ GameVarId.SixBuildingsTotal, "SixBuildingsTotal" },
		{ GameVarId.FirstRing, "FirstRing" },
		{ GameVarId.IsNewcomersActive, "IsNewcomersActive" },
		{ GameVarId.HasFirstDialogPlayed, "HasFirstDialogPlayed" },
		{ GameVarId.IsLabBuilt, "IsLabBuilt" },
		{ GameVarId.BuildingCountLab, "BuildingCountLab" },
		{ GameVarId.BuildingCountHabitat, "BuildingCountHabitat" },
		{ GameVarId.BuildingCountHydroponic, "BuildingCountHydroponic" },
		{ GameVarId.BuildingCountFusionReactor, "BuildingCountFusionReactor" },
		{ GameVarId.BuildingCountCannons, "BuildingCountCannons" },

		{ GameVarId.BuildingCountEntertainment, "BuildingCountEntertainment" },
		{ GameVarId.BuildingCountFactory, "BuildingCountFactory" },
		{ GameVarId.BuildingCountSensors, "BuildingCountSensors" },
		{ GameVarId.BuildingCountRepairDrones, "BuildingCountRepairDrones" },
		{ GameVarId.HasWonGame, "HasWonGame" },
		{ GameVarId.TraderStayLength, "TraderStayLength" },
		{ GameVarId.Bugz_Progress, "Bugz_Progress" },
		{ GameVarId.AttackStatus, "AttackStatus" },
		{ GameVarId.Voices_Progress, "Voices_Progress" },
		{ GameVarId.Tweak_Progress, "Tweak_Progress" },

		{ GameVarId.Mara_Progress, "Mara_Progress" },
		{ GameVarId.RandomEventGeneratorMax, "RandomEventGeneratorMax" },
		{ GameVarId.Celestis_Awareness, "Celestis_Awareness" },
		{ GameVarId.Chantara_Mission_Progress, "Chantara_Mission_Progress" },
		{ GameVarId.Chantara_Progress, "Chantara_Progress" },
		{ GameVarId.Cook_Progress, "Cook_Progress" },
		{ GameVarId.SpaceDebrisLength, "SpaceDebrisLength" },
		{ GameVarId.Habitat_Building_Gate, "Habitat_Building_Gate" },
		{ GameVarId.Habitat_Building_Progress, "Habitat_Building_Progress" },
		{ GameVarId.Station_Repair_Gate, "Station_Repair_Gate" },

		{ GameVarId.RemainingPopulationRoom, "RemainingPopulationRoom" },
		{ GameVarId.Chantara_Ship_State, "Chantara_Ship_State" },
		{ GameVarId.Trader_Ship_State, "Trader_Ship_State" },
	};

	public static GameVarId MapServerVarId (string serverId)
	{
		GameVarId varId = MapVars.FirstOrDefault(entrey => entrey.Value == serverId).Key;

		if (varId == GameVarId.None)
			Debug.LogWarning($"Missing GameVarId for serverId {serverId}");

		return varId;
	}

	public static GameVarType MapServerVarType (string serverType)
	{
		return serverType switch
		{
			"UI_VAR" => GameVarType.UIVar,
			"CHECK_VARS" => GameVarType.CheckVar,
			"TRACKING_VARS" => GameVarType.TrackingVar,
			_ => GameVarType.None
		};
	}

	public static CompareValueType MapServerCompareType (string serverType)
	{
		return serverType switch
		{
			"Equal" => CompareValueType.Equal,
			"Less" => CompareValueType.Less,
			"More" => CompareValueType.More,
			"LessEqual" => CompareValueType.LessEqual,
			"MoreEqual" => CompareValueType.MoreEqual,
			_ => CompareValueType.None
		};
	}

	public static ChangeValueType MapServerModifierType (string serverType)
	{
		return serverType switch
		{
			"Add" => ChangeValueType.Add,
			"Set" => ChangeValueType.Set,
			_ => ChangeValueType.None
		};
	}

	public static EventDataType MapServerEventType (string serverType)
	{
		return serverType switch
		{
			"FixedDay" => EventDataType.FixedDay,
			"RequireTrue" => EventDataType.RequireTrue,
			"Random" => EventDataType.Random,
			_ => EventDataType.None
		};
	}

	public static int ParseAndSolveMinMaxInt (string minData, string maxData)
	{
		int min = -1;
		int max = -1;

		if (!string.IsNullOrEmpty(minData))
			min = int.Parse(minData);

		if (!string.IsNullOrEmpty(maxData) && maxData.Trim() != "-1")
			max = int.Parse(maxData);
		else
			max = min;

		return UnityEngine.Random.Range(min, max + 1);
	}

	public static RequirementData ParseRequirementData (string[] requirements)
	{
		RequirementData require = new();
		require.varsChecks = ParseVarCompareValues (requirements);
		return require;
	}

	public static ResultData ParseResultData (string[] changeVars, string[] changeResources, string[] editEventsDay, string[] editVarsMax, string[] editBuildingsProgress)
	{
		string[] varsChanges = { };

		if (changeVars.Length > 0 && !string.IsNullOrEmpty(changeVars[0]))
			varsChanges = varsChanges.Concat(changeVars).ToArray();

		if (changeResources.Length > 0 && !string.IsNullOrEmpty(changeResources[0]))
			varsChanges = varsChanges.Concat(changeResources).ToArray();

		ResultData result = new();
		result.varChanges = ParseResultVarChanges(varsChanges);
		result.eventsDay = ParseEditEventDays(editEventsDay);
		result.varsMax = ParseEditVarMaxs(editVarsMax);
		result.buildingsProgress = ParseEditBuildingsProgress(editBuildingsProgress);
		return result;
	}

	public static VarCompareValue[] ParseVarCompareValues (string[] jsonData)
	{
		return ParseModuleData(jsonData, (string[] jsonData, int index) =>
		{
			// pattern : "GameVarId,CompareValueType,intValue"
			int endOffset = 3;
			GameVarId varId = MapServerVarId(jsonData[index].Trim());

			if (varId == GameVarId.None)
			{
				Debug.LogWarning($"Incorrect varId: {jsonData[index]}");
				return (jsonData.Length, null);
			}

			CompareValueType compareType = MapServerCompareType(jsonData[index + 1].Trim());
			int compareValue = int.Parse(jsonData[index + 2].Trim());

			VarCompareValue compare = new();
			compare.varId = varId;
			compare.checkType = compareType;
			compare.compareValue = compareValue;

			return (index + endOffset, compare);
		});
	}

	public static ResultVarChange[] ParseResultVarChanges (string[] jsonData)
	{
		return ParseModuleData(jsonData, (string[] jsonData, int index) =>
		{
			// pattern : "GameVarId,[ChangeValueType],intValueMin,[intValueMax]"
			int endOffset = 2;
			GameVarId varId = MapServerVarId(jsonData[index].Trim());

			if (varId == GameVarId.None)
			{
				Debug.LogWarning($"Incorrect varId: {jsonData[index]}");
				return (jsonData.Length, null);
			}

			ResultVarChange varChange = new();
			varChange.varId = varId;

			if (int.TryParse(jsonData[index + 1].Trim(), out int minValue))
			{
				varChange.modifierType = ChangeValueType.Add;
				varChange.modifierValueMin = minValue;
			}
			else
			{
				varChange.modifierType = MapServerModifierType(jsonData[index + 1].Trim());
				varChange.modifierValueMin = int.Parse(jsonData[index + 2].Trim());
				endOffset++;
			}

			if (index + endOffset < jsonData.Length && int.TryParse(jsonData[index + endOffset].Trim(), out int maxValue))
			{
				varChange.modifierValueMax = maxValue;
				endOffset++;
			}
			else
				varChange.modifierValueMax = varChange.modifierValueMin;

			// default initial value
			varChange.currentValue = varChange.modifierValueMin;

			return (index + endOffset, varChange);
		});
	}

	public static EditEventDay[] ParseEditEventDays (string[] jsonData)
	{
		return ParseModuleData(jsonData, (string[] jsonData, int index) =>
		{
			// pattern:	"eventName,ChangeValueType,intValueMin,[intValueMax]"
			int endOffset = 3;

			EditEventDay editDay = new();
			editDay.eventName = jsonData[index].Trim();

			ChangeValueType modifier = MapServerModifierType(jsonData[index + 1].Trim());
			int firstValue = int.Parse(jsonData[index + 2].Trim());

			if (modifier == ChangeValueType.Set)
				editDay.setDay = firstValue;
			else
				editDay.addMinDays = firstValue;

			if (index + endOffset < jsonData.Length && int.TryParse(jsonData[index + 3].Trim(), out int secondValue))
			{
				if (modifier == ChangeValueType.Add)
					editDay.addMaxDays = secondValue;

				endOffset++;
			}

			return (index + endOffset, editDay);
		});
	}

	public static EditVarMax[] ParseEditVarMaxs (string[] jsonData)
	{
		return ParseModuleData(jsonData, (string[] jsonData, int index) =>
		{
			// pattern:	"GameVarId,intValue"
			int endOffset = 2;

			EditVarMax editVarMax = new();
			editVarMax.varId = MapServerVarId(jsonData[index].Trim());
			editVarMax.setMax = int.Parse(jsonData[index + 1].Trim());

			return (index + endOffset, editVarMax);
		});
	}

	public static EditBuildingProgress[] ParseEditBuildingsProgress (string[] jsonData)
	{
		return ParseModuleData(jsonData, (string[] jsonData, int index) =>
		{
			// pattern: "buildingName,intProgress,boolIsBuilt"
			int endOffset = 3;

			EditBuildingProgress editBuildingProgress = new();
			editBuildingProgress.buildingName = jsonData[index].Trim();
			editBuildingProgress.progress = int.Parse(jsonData[index + 1].Trim());
			editBuildingProgress.isBuilt = bool.Parse(jsonData[index + 2].Trim());

			return (index + endOffset, editBuildingProgress);
		});
	}

	public static (EditAnimations, EditAnimations) ParseEditAnimations (string[] jsonData)
	{
		EditAnimations enterAnimations = new();
		EditAnimations exitAnimations = new();

		List<string> enterStartAnimation = new();
		List<string> enterStopAnimation = new();
		List<string> exitStartAnimation = new();
		List<string> exitStopAnimation = new();

		ParseModuleData<string>(jsonData, (string[] jsonData, int index) =>
		{
			// pattern: "animationName,boolIsStart,boolIsEnter"
			int endOffset = 3;

			string name = jsonData[index].Trim();
			bool isStart = bool.Parse(jsonData[index].Trim());
			bool isEnter = bool.Parse(jsonData[index].Trim());

			List<string> animations = null;

			if (isStart && isEnter)
				animations = enterStartAnimation;
			else if (isStart && !isEnter)
				animations = exitStartAnimation;
			else if (!isStart && isEnter)
				animations = enterStopAnimation;
			else if (!isStart && !isEnter)
				animations = exitStopAnimation;

			animations.Add(name);

			return (index + endOffset, null);
		});

		enterAnimations.startAnimations = enterStartAnimation.ToArray();
		enterAnimations.stopAnimations = enterStopAnimation.ToArray();
		exitAnimations.stopAnimations = exitStartAnimation.ToArray();
		exitAnimations.stopAnimations = exitStopAnimation.ToArray();

		return (enterAnimations, exitAnimations);
	}

	public static ProductionMultiplierData[] ParseProductionMultipliers (string[] jsonData)
	{
		return ParseModuleData(jsonData, (string[] jsonData, int index) =>
		{
			// pattern:	"GameVarId,intValue"
			int endOffset = 2;

			ProductionMultiplierData productionMultiplier = new();
			productionMultiplier.varId = MapServerVarId(jsonData[index].Trim());
			productionMultiplier.multiplier = int.Parse(jsonData[index + 1].Trim());

			return (index + endOffset, productionMultiplier);
		});
	}

	private static T[] ParseModuleData<T> (string[] jsonData, Func<string[], int, (int, T)> parser)
	{
		if (jsonData == null || jsonData.Length == 0 || string.IsNullOrEmpty(jsonData[0].Trim()))
			return new T[0];

		List<T> entries = new();

		for (int i = 1; i < jsonData.Length; i++)
		{
			(int newIndex, T entry) = parser(jsonData, i - 1);

			if (entry != null)
				entries.Add(entry);

			i = newIndex;
		}

		return entries.ToArray();
	}

}
