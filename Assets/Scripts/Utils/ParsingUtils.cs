using System.Collections.Generic;
using System.Linq;

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
	};

	public static GameVarId MapServerVarId (string serverId)
	{
		return MapVars.FirstOrDefault(entrey => entrey.Value == serverId).Key;
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

	public static EventType MapServerEventType (string serverType)
	{
		return serverType switch
		{
			"FixedDay" => EventType.FixedDay,
			"RequireTrue" => EventType.RequireTrue,
			"Random" => EventType.Random,
			_ => EventType.None
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
			for (int i = 0; i < changeResources.Length; i++)
			{
				GameVarId varId = MapServerVarId(changeResources[i].Trim());

				if (varId != GameVarId.None)
				{
					ResultVarChange change = new();
					change.varId = varId;
					change.modifierType = ChangeValueType.Add;
					change.modifierValueMin = int.Parse(changeResources[i + 1].Trim());
					i++;

					// if a another int value is provided, there's a range result
					if (i < changeResources.Length - 1 && int.TryParse(changeResources[i + 1].Trim(), out int value))
					{
						change.modifierValueMax = value;
						i++;
					}
					else
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
