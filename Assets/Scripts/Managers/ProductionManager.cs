using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProductionManager : MonoBehaviour
{
	[NonSerialized] private ProductionData[] _production;

	public void InitProductions(ProductionData[] production)
	{
		if (production == null)
		{
			Debug.LogError($"No productions to init");
			return;
		}

		Debug.Log($"Produtions loaded (total: {production.Length})");

		_production = production;
	}

	public (GameVarId, int)[] GetProduction ()
	{
		int population = GameManager.Instance.varsManager.GetVarValue(GameVarId.Population);

		Dictionary<GameVarId, int> productionByResource = new();

		for (int i = 0; i < population; i++)
		{
			int randomIndex = UnityEngine.Random.Range(0, _production.Length);
			ProductionData productionData = _production[randomIndex];

			if (productionData.CheckRandom())
			{
				int productionValue = 1;

				foreach (ProductionMultiplierData multiplierData in GetProductionMultiplier(productionData.varId))
				{
					// additif
					productionValue += multiplierData.multiplier;
				}

				if (productionByResource.ContainsKey(productionData.varId))
					productionByResource[productionData.varId] += productionValue;
				else
					productionByResource.Add(productionData.varId, productionValue);
			}
		}

		return productionByResource
			.Select(entry => (entry.Key, entry.Value))
			.ToArray()
		;
	}

	private ProductionMultiplierData[] GetProductionMultiplier (GameVarId varId)
	{
		return GameManager.Instance.buildingsManager.GetMultipliers(varId);
	}

}
