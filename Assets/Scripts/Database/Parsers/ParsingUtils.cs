
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

}
