using I2.Loc;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DailyReportPanelButtonId
{
	None,
	Continue,
}

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
		foodLocalizeParams.SetParameterValue(LocValue.DailyFoodValue, $"{panelContent.foodValue}");

		qolLocalize.SetTerm($"{LocCat.UI}/{panelContent.qolKey}");
		qolLocalizeParams.SetParameterValue(LocValue.DailyQoLValue, $"{panelContent.qolValue}");

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

		if (GameManager.Instance != null)
		{
			GameManager.Instance.uiManager.RemoveResourceValues(resourcesParent);
			GameManager.Instance.uiManager.RemoveResourceValues(productionParent);
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

	private void AddResourceValueDiff()
	{
		Dictionary<GameVarId, int> startDayValues = GameManager.Instance.varsManager.GetStartDayResourcesValue();
		VarData[] endDayValues = GameManager.Instance.varsManager.GetResourcesData();

		foreach (VarData resourceData in endDayValues)
		{
			int diff = 0;

			if (startDayValues.TryGetValue(resourceData.varId, out int oldValue))
				diff = resourceData.currentValue - oldValue;

			if (diff != 0)
				GameManager.Instance.uiManager.AddResourceValue(resourceData, diff, resourcesParent, Color.white);
		}
	}

	private void AddProduction ((GameVarId, int)[] production)
	{
		foreach ((GameVarId varId, int value) in production)
		{
			VarData varData = GameManager.Instance.varsManager.GetVarData(varId);
			GameManager.Instance.uiManager.AddResourceValue(varData, value, productionParent, Color.white);
		}
	}

}
