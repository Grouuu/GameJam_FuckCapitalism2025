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

	public void SetBuildingProgress (string buildingName, int progress)
	{
		BuildingData buildingData = GetBuildingByName(buildingName);

		if (buildingData == null)
			return;

		buildingData.progress = progress;

		UpdateBuildingVisual(buildingName);
		UpdateSaveData();
	}

	public void SetBuildingIsBuilt (string buildingName, bool isBuilt)
	{
		BuildingData buildingData = GetBuildingByName(buildingName);

		if (buildingData == null)
			return;

		buildingData.isBuilt = isBuilt;

		UpdateSaveData();
	}

	public void UpdateSaveData ()
	{
		List<(string, bool, int)> buildingsState = new();

		foreach (BuildingData buildingData in _buildings)
			buildingsState.Add((buildingData.name, buildingData.isBuilt, buildingData.progress));

		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.BuildingsState, buildingsState);
	}

	public void ApplySave ()
	{
		List<(string, bool, int)> buildingsState = GameManager.Instance.saveManager.GetSaveData<List<(string, bool, int)>>(SaveItemKey.BuildingsState);

		if (buildingsState != null)
		{
			foreach ((string buildingName, bool isBuilt, int progress) in buildingsState)
			{
				BuildingData buildingData = GetBuildingByName(buildingName);

				if (buildingData != null)
				{
					buildingData.isBuilt = isBuilt;
					buildingData.progress = progress;
				}
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

	private void UpdateBuildingVisual (string buildingName)
	{
		BuildingData buildingData = GetBuildingByName(buildingName);
		// TODO
	}
}
