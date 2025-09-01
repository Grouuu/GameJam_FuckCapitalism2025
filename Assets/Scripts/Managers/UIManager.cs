using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static readonly string PATH_SPRITES = "Sprites/";

	public ResourceIconUI prefabResourceIcon;
	public ResourceValueSeparatorUI prefabResourceSeparator;
	public ResourceValueUI[] resourceValuesUI;

	public DialogPanelUI dialogPanel => GetComponent<DialogPanelUI>();
	public EventPanelUI eventPanel => GetComponent<EventPanelUI>();
	public DailyReportPanelUI reportPanel => GetComponent<DailyReportPanelUI>();
	public OptionsPanelUI optionsPanel => GetComponent<OptionsPanelUI>();

	public void SetResourceValue (GameVarId id, int value, int max)
	{
		if (id == GameVarId.None)
			return;

		GetUIResourceComponent(id)?.SetValue(value, max);
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

	public void ShowReportPanel (DailyReportPanelUIData panelData, Action onContinue)
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

	public void AddResourceValue (VarData varData, int diff, Transform parent, Color color, Color secondColor)
	{
		// TODO use pool
		ResourceIconUI resource = Instantiate(prefabResourceIcon, parent);

		resource.SetColor(color, color, secondColor, color);
		resource.SetIcon(varData.iconFileName);
		resource.SetValue(diff);
		resource.SetTooltipName(varData.name);
	}

	public void AddResourceSeparator (Transform parent)
	{
		// TODO use pool
		Instantiate(prefabResourceSeparator, parent);
	}

	public void RemoveResourceValues (Transform parent)
	{
		ResourceIconUI[] resources = parent.GetComponentsInChildren<ResourceIconUI>();

		foreach (ResourceIconUI resource in resources)
		{
			Destroy(resource.gameObject);
		}

		ResourceValueSeparatorUI[] separators = parent.GetComponentsInChildren<ResourceValueSeparatorUI>();

		foreach (ResourceValueSeparatorUI separator in separators)
		{
			Destroy(separator.gameObject);
		}
	}

	public void ShowResourceLowWarning (GameVarId varId, bool isLow)
	{
		GetUIResourceComponent(varId)?.ShowLowWarning(isLow);
	}

	public void ShowResourceMaxWarning (GameVarId varId, bool isMax)
	{
		GetUIResourceComponent(varId)?.ShowMaxWarning(isMax);
	}

	private void OnEnable ()
	{
		InitResourceValueUI();
	}

	private void InitResourceValueUI ()
	{
		foreach (ResourceValueUI valueUI in resourceValuesUI)
			valueUI.Update();
	}

	private ResourceValueUI GetUIResourceComponent (GameVarId resourceId)
	{
		return Array.Find(resourceValuesUI, component => component.id == resourceId);
	}

}
