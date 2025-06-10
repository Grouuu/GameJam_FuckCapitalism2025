using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProductionManager : MonoBehaviour
{
	[NonSerialized] private ProductionData[] _production;

	public void InitProduction(ProductionData[] production)
	{
		if (production == null)
		{
			Debug.LogError($"No production to init");
			return;
		}

		Debug.Log($"Prodution loaded (total: {production.Length})");

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
				if (productionByResource.ContainsKey(productionData.varId))
					productionByResource[productionData.varId]++;
				else
					productionByResource.Add(productionData.varId, 1);
			}
		}

		return productionByResource
			.Select(entry => (entry.Key, entry.Value))
			.ToArray()
		;
	}

}
