using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public GameObject resourceValueContainer;
	public GameObject centralWindow;

	private ResourceValueUI[] _resourceValuesUI;

	private PanelUI panel => centralWindow.GetComponent<PanelUI>();

	public void SetValues (ResourceData[] resources)
	{
		foreach (ResourceData data in resources)
		{
			SetValue(data);
		}
	}

	public void SetValue (ResourceData resourceData)
	{
		if (resourceData.id == ResourceId.None)
			return;

		GetUIComponent(resourceData.id)?.SetValue(resourceData.value);
	}

	public void ShowEvent (PanelUIData panelData, Action onContinue)
	{
		panel.onceClickCallback = () => {
			EventClosed();
			if (onContinue != null)
				onContinue();
		};

		panel.Show(panelData);
	}

	private void Awake ()
	{
		_resourceValuesUI = resourceValueContainer?.GetComponentsInChildren<ResourceValueUI>();
	}

	private void EventClosed ()
	{
		panel.Hide();
	}

	private ResourceValueUI GetUIComponent (ResourceId resourceId)
	{
		return Array.Find(_resourceValuesUI, component => component.id == resourceId);
	}

}
