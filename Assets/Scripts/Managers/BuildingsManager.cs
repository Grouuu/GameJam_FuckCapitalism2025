using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingsManager : MonoBehaviour
{
	[NonSerialized] private BuildingData[] _buildings;

	public void InitBuildings (BuildingData[] buildings)
	{
		if (buildings == null)
		{
			Debug.LogError($"No buildings to init");
			return;
		}

		Debug.Log($"Buildings loaded (total: {buildings.Length})");

		_buildings = buildings;
	}

	public void UpdateSaveData ()
	{
		// TODO use it

		List<string> buildingsBuilt = new();

		foreach (BuildingData buildingData in _buildings)
		{
			if (buildingData.isBuilt)
				buildingsBuilt.Add(buildingData.name);
		}

		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.BuildingsBuilt, buildingsBuilt);
	}

	public void ApplySave ()
	{
		List<string> buildingsBuilt = GameManager.Instance.saveManager.GetSaveData<List<string>>(SaveItemKey.BuildingsBuilt);

		if (buildingsBuilt != null)
		{
			foreach (string buildingName in buildingsBuilt)
			{
				BuildingData buildingData = GetBuildingByName(buildingName);

				if (buildingData == null)
					continue;

				buildingData.isBuilt = true;
			}
		}
	}

	public BuildingData GetBuildingByName (string buildingName)
	{
		return _buildings.FirstOrDefault(entry => entry.name == buildingName);
	}

	public ProductionMultiplierData[] GetMultipliers (GameVarId varId)
	{
		List<ProductionMultiplierData> multipliers = new();

		foreach (BuildingData buildingData in _buildings)
		{
			if (!buildingData.isBuilt)
				continue;

			foreach(ProductionMultiplierData multiplierData in buildingData.productionMultipliers)
			{
				if (multiplierData.varId == varId)
					multipliers.Add(multiplierData);
			}
		}

		return multipliers.ToArray();
	}
}
