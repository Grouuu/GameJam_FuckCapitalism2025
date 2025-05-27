using System.Collections.Generic;

public static class ParsingUtils
{
	public static GameVarId MapServerVarId (string serverId)
	{
		return serverId switch
		{
			"Population" => GameVarId.Population,
			"QoL" => GameVarId.QoL,
			"Trust" => GameVarId.Trust,
			"Food" => GameVarId.Food,
			"Scraps" => GameVarId.Scraps,
			"Power" => GameVarId.Power,
			"Science" => GameVarId.Science,
			"Fleet" => GameVarId.Fleet,
			"Weapons" => GameVarId.Weapons,
			"StationHP" => GameVarId.StationHP,
			"Modules" => GameVarId.Modules,
			"Day" => GameVarId.Day,
			"FoodAfterConsumption" => GameVarId.FoodAfterConsumption,
			"NextBattlePreview" => GameVarId.NextBattlePreview,
			"ControlFreeWill" => GameVarId.ControlFreeWill,
			"UtilitarismEmpathy" => GameVarId.UtilitarismEmpathy,
			"TraderCount" => GameVarId.TraderCount,
			"PetLover" => GameVarId.PetLover,
			"TrustGateMaxReached" => GameVarId.TrustGateMaxReached,
			"FoodProduction" => GameVarId.FoodProduction,
			"ScrapsProduction" => GameVarId.ScrapsProduction,
			"ScienceProduction" => GameVarId.ScienceProduction,
			"TrustGate1" => GameVarId.TrustGate1,
			"TrustGate2" => GameVarId.TrustGate2,
			"TrustGate3" => GameVarId.TrustGate3,
			"TrustGate4" => GameVarId.TrustGate4,
			"TrustGate5" => GameVarId.TrustGate5,
			"TrustGate6" => GameVarId.TrustGate6,
			"HPachievments" => GameVarId.HPachievments,
			"FirstBuilding" => GameVarId.FirstBuilding,
			"SixBuildingsTotal" => GameVarId.SixBuildingsTotal,
			"FirstRing" => GameVarId.FirstRing,
			"IsNewcomersActive" => GameVarId.IsNewcomersActive,
			"HasFirstDialogPlayed" => GameVarId.HasFirstDialogPlayed,
			"IsLabBuilt" => GameVarId.IsLabBuilt,
			"BuildingCountLab" => GameVarId.BuildingCountLab,
			"BuildingCountHabitat" => GameVarId.BuildingCountHabitat,
			"BuildingCountHydroponic" => GameVarId.BuildingCountHydroponic,
			"BuildingCountFusionReactor" => GameVarId.BuildingCountFusionReactor,
			"BuildingCountCannons" => GameVarId.BuildingCountCannons,
			"BuildingCountEntertainment" => GameVarId.BuildingCountEntertainment,
			"BuildingCountFactory" => GameVarId.BuildingCountFactory,
			"BuildingCountSensors" => GameVarId.BuildingCountSensors,
			"BuildingCountRepairDrones" => GameVarId.BuildingCountRepairDrones,
			"HasWonGame" => GameVarId.HasWonGame,
			_ => GameVarId.None
		};
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

	public static RequirementData ParseRequirementData (string[] requirement)
	{
		// TODO allow multiple instances

		if (requirement.Length != 3 || requirement[0] == "")
			return null;

		GameVarId varId = MapServerVarId(requirement[0].Trim());

		if (varId == GameVarId.None)
			return null;

		CompareValueType compareType = MapServerCompareType(requirement[1].Trim());
		int compareValue = int.Parse(requirement[2].Trim());

		VarCompareValue compare = new();
		compare.varId = varId;
		compare.checkType = compareType;
		compare.compareValue = compareValue;

		RequirementData require = new();
		require.varsChecks = new VarCompareValue[1] { compare };

		return require;
	}

	public static ResultData ParseResultData (string[] changeVars, string[] changeResources, string[] editDays)
	{
		// TODO allow multiple instances for all parts

		ResultData result = new();

		// vars change

		List<ResultVarChange> varChanges = new();

		if (changeVars != null && changeVars.Length >= 3 && changeVars[0] != "")
		{
			GameVarId varId = MapServerVarId(changeVars[0].Trim());

			if (varId != GameVarId.None)
			{
				ResultVarChange change = new();
				change.varId = varId;
				change.modifierType = MapServerModifierType(changeVars[1].Trim());
				change.modifierValueMin = int.Parse(changeVars[2].Trim());

				if (changeVars.Length == 4)
					change.modifierValueMax = int.Parse(changeVars[3].Trim());
				else
					change.modifierValueMax = change.modifierValueMin;

				change.currentValue = change.modifierValueMin;

				varChanges.Add(change);
			}
		}

		if (changeResources != null && changeResources.Length > 0 && changeResources[0] != "")
		{
			for (int i = 0; i < changeResources.Length; i += 2)
			{
				GameVarId varId = MapServerVarId(changeResources[i].Trim());

				if (varId != GameVarId.None)
				{
					ResultVarChange change = new();
					change.varId = varId;
					change.modifierType = ChangeValueType.Add;
					change.modifierValueMin = int.Parse(changeResources[i + 1].Trim());
					change.modifierValueMax = change.modifierValueMin;
					change.currentValue = change.modifierValueMin;

					varChanges.Add(change);
				}
			}
		}

		result.varChanges = varChanges.ToArray();

		// edit days

		List<EditEventDay> eventDayChanges = new();

		if (editDays != null && editDays.Length >= 3 && editDays[0] != "")
		{
			EditEventDay edit = new();
			edit.eventName = editDays[0].Trim();

			ChangeValueType modifier = MapServerModifierType(editDays[1].Trim());

			if (modifier == ChangeValueType.Set)
			{
				edit.setDay = int.Parse(editDays[2].Trim());
			}
			else
			{
				edit.addMinDays = int.Parse(editDays[2].Trim());

				if (editDays.Length == 4)
					edit.addMaxDays = int.Parse(editDays[3].Trim());
				else
					edit.addMaxDays = edit.addMinDays;
			}

			eventDayChanges.Add(edit);
		}

		result.eventsDay = eventDayChanges.ToArray();

		return result;
	}

}
