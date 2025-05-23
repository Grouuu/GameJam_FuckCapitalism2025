using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public ResourceValueUI[] resourceValuesUI;

	private DialogPanelUI _dialogPanel => GetComponent<DialogPanelUI>();
	private EventPanelUI _eventPanel => GetComponent<EventPanelUI>();
	private ReportPanelUI _reportPanel => GetComponent<ReportPanelUI>();

	public void SetResourceValue (GameVarId id, int value)
	{
		if (id == GameVarId.None)
			return;

		GetUIResourceComponent(id)?.SetValue(value);
	}

	public void ShowEventPanel (EventPanelUIData panelData, Action onContinue)
	{
		_eventPanel.onceClickCallback = () => {
			EventClosed();
			if (onContinue != null)
				onContinue();
		};

		_eventPanel.Show(panelData);
	}

	public void ShowReportPanel (ReportPanelUIData panelData, Action onContinue)
	{
		_reportPanel.onceClickCallback = () => {
			ReportClosed();
			if (onContinue != null)
				onContinue();
		};

		_reportPanel.Show(panelData);
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
		_eventPanel.Hide();
	}

	private void ReportClosed ()
	{
		_reportPanel.Hide();
	}

	private void DialogClosed ()
	{
		_dialogPanel.Hide();
	}

	private ResourceValueUI GetUIResourceComponent (GameVarId resourceId)
	{
		return Array.Find(resourceValuesUI, component => component.id == resourceId);
	}

}
