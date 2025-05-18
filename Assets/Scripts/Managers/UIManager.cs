using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public ResourceValueUI[] resourceValuesUI;
	public GameObject centralPanel;
	public GameObject dialogPanel;

	private CentralPanelUI _centralPanel => GetComponent<CentralPanelUI>();
	private DialogPanelUI _dialogPanel => GetComponent<DialogPanelUI>();

	public void SetResourceValues (ResourceData[] resources)
	{
		foreach (ResourceData data in resources)
		{
			SetResourceValue(data);
		}
	}

	public void SetResourceValue (ResourceData resourceData)
	{
		if (resourceData.id == ResourceId.None)
			return;

		GetUIResourceComponent(resourceData.id)?.SetValue(resourceData.value);
	}

	public void ShowEventPanel (CentralPanelUIData panelData, Action onContinue)
	{
		_centralPanel.onceClickCallback = () => {
			EventClosed();
			if (onContinue != null)
				onContinue();
		};

		_centralPanel.Show(panelData);
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
			_dialogPanel.onceYesCallback = () => {
				DialogClosed();
				if (onYes != null)
					onYes();
			};

			_dialogPanel.onceNoCallback = () => {
				DialogClosed();
				if (onNo != null)
					onNo();
			};
		}
		else
		{
			// response layout
			_dialogPanel.onceContinueCallback = () => {
				DialogClosed();
				if (onYes != null)
					onYes();
			};
		}

		_dialogPanel.Show(contentData, isYesNoContent ? DialogPanelUIButtonsLayout.YesNo : DialogPanelUIButtonsLayout.Continue);
	}

	private void EventClosed ()
	{
		_centralPanel.Hide();
	}

	private void DialogClosed ()
	{
		_dialogPanel.Hide();
	}

	private ResourceValueUI GetUIResourceComponent (ResourceId resourceId)
	{
		return Array.Find(resourceValuesUI, component => component.id == resourceId);
	}

}
