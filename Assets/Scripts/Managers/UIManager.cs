using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public GameObject resourceValueContainer;

	private ResourceValueUI[] _resourceValues;

	public void SetValues (ResourceData[] resources)
	{
		foreach (ResourceData data in resources)
		{
			GetUIComponent(data.id)?.SetValue(data.value);
		}
	}

	private void Awake ()
	{
		_resourceValues = resourceValueContainer?.GetComponentsInChildren<ResourceValueUI>();
	}

	private ResourceValueUI GetUIComponent (ResourceId resourceId)
	{
		return Array.Find(_resourceValues, component => component.id == resourceId);
	}

}
