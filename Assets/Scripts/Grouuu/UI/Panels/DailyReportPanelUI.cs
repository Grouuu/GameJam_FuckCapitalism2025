using Grouuu.Constants;
using Grouuu.Data;
using Grouuu.Enum;
using Grouuu.Utils;
using I2.Loc;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Grouuu.UI
{
	public class DailyReportPanelUIData
	{
		public int dayValue;
		public string foodKey;
		public int foodValue;
		public string qolKey;
		public int qolValue;
		public (GameVarId, int)[] production;
	}

	public class DailyReportPanelUI : MonoBehaviour
	{
		public GameObject parent;
		public TextMeshProUGUI dayCountUI;
		public TextMeshProUGUI foodChange;
		public TextMeshProUGUI populationChange;
		public Transform resourcesParent;
		public Transform productionParent;
		public Button continueButton;
		public Localize foodLocalize;
		public Localize qolLocalize;
		public LocalizationParamsManager foodLocalizeParams;
		public LocalizationParamsManager qolLocalizeParams;

		[HideInInspector] public Action onceClickCallback;

		public void Show (DailyReportPanelUIData panelContent)
		{
			dayCountUI.text = $"{panelContent.dayValue}";

			foodLocalize.SetTerm($"{LocCat.UI}/{panelContent.foodKey}");
			foodLocalizeParams.SetParameterValue(LocParam.DailyFoodValue, $"{panelContent.foodValue}");

			qolLocalize.SetTerm($"{LocCat.UI}/{panelContent.qolKey}");
			qolLocalizeParams.SetParameterValue(LocParam.DailyQoLValue, $"{panelContent.qolValue}");

			AddResourceValueDiff();
			AddProduction(panelContent.production);

			parent.SetActive(true);
		}

		public void Hide ()
		{
			parent.SetActive(false);

			dayCountUI.text = "";
			foodChange.text = "";
			populationChange.text = "";

			onceClickCallback = null;

			if (GameController.GameManagers != null)
			{
				GameController.GameManagers.UiManager.RemoveResourceValues(resourcesParent);
				GameController.GameManagers.UiManager.RemoveResourceValues(productionParent);
			}
		}

		/**
		 * Linked in the editor
		 */
		public void OnContinueClick ()
		{
			OnClick(onceClickCallback);
		}

		private void OnClick (Action callback)
		{
			onceClickCallback = null;

			if (callback != null)
				callback();
		}

		private void OnEnable ()
		{
			Hide();
		}

		private void AddResourceValueDiff ()
		{
			Dictionary<GameVarId, int> startDayValues = GameController.GameManagers.VarsManager.GetStartDayResourcesValue();
			VarData[] endDayValues = GameController.GameManagers.VarsManager.GetResourcesData();
			Dictionary<VarData, int> varDataPositiveDelta = new();
			Dictionary<VarData, int> varDataNegativeDelta = new();

			foreach (VarData resourceData in endDayValues)
			{
				int diff = 0;

				if (startDayValues.TryGetValue(resourceData.varId, out int oldValue))
					diff = resourceData.currentValue - oldValue;

				if (diff < 0)
					varDataNegativeDelta.Add(resourceData, diff);
				else if (diff > 0)
					varDataPositiveDelta.Add(resourceData, diff);
			}

			foreach ((VarData resourceData, int diff) in varDataPositiveDelta)
				GameController.GameManagers.UiManager.AddResourceValue(resourceData, diff, resourcesParent, Color.white, Color.black);

			if (varDataPositiveDelta.Count > 0 && varDataNegativeDelta.Count > 0)
				GameController.GameManagers.UiManager.AddResourceSeparator(resourcesParent);

			foreach ((VarData resourceData, int diff) in varDataNegativeDelta)
				GameController.GameManagers.UiManager.AddResourceValue(resourceData, diff, resourcesParent, Color.white, Color.black);
		}

		private void AddProduction ((GameVarId, int)[] production)
		{
			foreach ((GameVarId varId, int value) in production)
			{
				VarData varData = GameController.GameManagers.VarsManager.GetVarData(varId);
				GameController.GameManagers.UiManager.AddResourceValue(varData, value, productionParent, Color.white, Color.black);
			}
		}
	}
}