using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ReportPanelButtonId
{
	None,
	Continue,
}

public class ReportPanelUIData
{
	public string dayLabel;
	public int dayValue;
	public string description;
	public (string iconFileName, int valueDiff)[] resourcesChange;
	public string foodChange;
	public string populationChange;
}

public class ReportPanelUI : MonoBehaviour
{
	public GameObject parent;
	public TextMeshProUGUI dayLabelUI;
	public TextMeshProUGUI dayCountUI;
	public TextMeshProUGUI descriptionUI;
	public TextMeshProUGUI foodChange;
	public TextMeshProUGUI populationChange;
	public Transform resourcesParent;
	public GameObject prefabResource;
	public Button continueButton;

	[HideInInspector] public Action onceClickCallback;

	public void Show (ReportPanelUIData panelContent)
	{
		dayLabelUI.text = panelContent.dayLabel;
		dayCountUI.text = $"{panelContent.dayValue}";
		descriptionUI.text = panelContent.description;
		foodChange.text = panelContent.foodChange;
		populationChange.text = panelContent.populationChange;

		AddResourceValueDiff();

		parent.SetActive(true);
	}

	public void Hide ()
	{
		parent.SetActive(false);

		dayLabelUI.text = "";
		dayCountUI.text = "";
		descriptionUI.text = "";
		foodChange.text = "";
		populationChange.text = "";

		onceClickCallback = null;

		RemoveResourceValues();
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

	private void Awake ()
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
				AddResourceValue(resourceData, diff);
		}
	}

	private void AddResourceValue (VarData varData, int diff)
	{
		// TODO use pool
		GameObject item = Instantiate(prefabResource, resourcesParent);
		ReportResourceValueUI resource = item.GetComponent<ReportResourceValueUI>();

		resource.SetIcon(varData.iconFileName);
		resource.SetValue(diff);
	}

	private void RemoveResourceValues ()
	{
		ReportResourceValueUI[] resources = resourcesParent.GetComponentsInChildren<ReportResourceValueUI>();

		foreach (ReportResourceValueUI resource in resources)
		{
			Destroy(resource.gameObject);
		}
	}
}
