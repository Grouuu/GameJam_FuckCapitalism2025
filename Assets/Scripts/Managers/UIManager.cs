using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public GameObject prefabResourceIcon;
	public ResourceValueUI[] resourceValuesUI;

	public DialogPanelUI dialogPanel => GetComponent<DialogPanelUI>();
	public EventPanelUI eventPanel => GetComponent<EventPanelUI>();
	public ReportPanelUI reportPanel => GetComponent<ReportPanelUI>();
	public OptionsPanelUI optionsPanel => GetComponent<OptionsPanelUI>();

	public void SetResourceValue (GameVarId id, int value)
	{
		if (id == GameVarId.None)
			return;

		GetUIResourceComponent(id)?.SetValue(value);
	}

	public void ShowEventPanel (EventPanelUIData panelData, Action onContinue)
	{
		eventPanel.onceClickCallback = () => {
			HideEventPanel();
			if (onContinue != null)
				onContinue();
		};

		eventPanel.Show(panelData);
	}

	public void ShowReportPanel (ReportPanelUIData panelData, Action onContinue)
	{
		reportPanel.onceClickCallback = () => {
			HideReportPanel();
			if (onContinue != null)
				onContinue();
		};

		reportPanel.Show(panelData);
	}

	public void ShowDialogPanel (DialogPanelUIData contentData, Action onContinue)
	{
		ShowDialogPanel(contentData, onContinue, null);
	}

	public void ShowDialogPanel (DialogPanelUIData contentData, Action onYes, Action onNo)
	{
		bool isYesNoContent = onNo != null;

		if (isYesNoContent)
		{
			// request layout
			dialogPanel.onceYesCallback = () => {
				HideDialogPanel();
				if (onYes != null)
					onYes();
			};

			dialogPanel.onceNoCallback = () => {
				HideDialogPanel();
				if (onNo != null)
					onNo();
			};
		}
		else
		{
			// response layout
			dialogPanel.onceContinueCallback = () => {
				HideDialogPanel();
				if (onYes != null)
					onYes();
			};
		}

		dialogPanel.Show(contentData, isYesNoContent ? DialogPanelUIButtonsLayout.YesNo : DialogPanelUIButtonsLayout.Continue);
	}

	public void HideEventPanel ()
	{
		eventPanel.Hide();
	}

	public void HideReportPanel ()
	{
		reportPanel.Hide();
	}

	public void HideDialogPanel ()
	{
		dialogPanel.Hide();
	}

	public void AddResourceValue (VarData varData, int diff, Transform parent, Color color)
	{
		// TODO use pool
		GameObject item = Instantiate(prefabResourceIcon, parent);
		ReportResourceValueUI resource = item.GetComponent<ReportResourceValueUI>();

		resource.SetColor(color);
		resource.SetIcon(varData.iconFileName);
		resource.SetValue(diff);
	}

	public void RemoveResourceValues (Transform parent)
	{
		ReportResourceValueUI[] resources = parent.GetComponentsInChildren<ReportResourceValueUI>();

		foreach (ReportResourceValueUI resource in resources)
		{
			Destroy(resource.gameObject);
		}
	}

	private ResourceValueUI GetUIResourceComponent (GameVarId resourceId)
	{
		return Array.Find(resourceValuesUI, component => component.id == resourceId);
	}

}
