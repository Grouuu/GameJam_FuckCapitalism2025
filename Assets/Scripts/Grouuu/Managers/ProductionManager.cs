using Grouuu.Data;
using Grouuu.Enum;
using Grouuu.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grouuu.Managers
{
	public class ProductionManager : IManager
	{
		private readonly ProductionManagerSettings _settings;
		private ProductionData[] _production;

		public ProductionManager (ProductionManagerSettings settings)
		{
			_settings = settings;
		}

		public void InitProductions (ProductionData[] production)
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
			int population = GameController.GameManagers.VarsManager.GetVarValue(GameVarId.Population);

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
			return GameController.GameManagers.BuildingsManager.GetMultipliers(varId);
		}
	}
}