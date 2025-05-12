using System;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceId
{
	None,
	Days,
	Population,
	Morale,
	Trust,
	Food,
	Scraps,
	Power,
	Science,
	Fleet,
	Weapons,
	StationHP,
	Modules,
}

[Serializable]
public class ResourceData
{
	public ResourceId id = ResourceId.None;
	public int value = -1;

	public ResourceData Clone () => new () { id = id, value = value };
}

public class ResourcesManager : MonoBehaviour
{
	public List<ResourceData> resources = new();

	public int GetResourceValue (ResourceId id)
	{
		return GetLocalResourceData(id).value;
	}

	public void AddResourceValue (ResourceData resourceData)
	{
		SetResourceValue(resourceData.id, GetResourceValue(resourceData.id) + resourceData.value);
	}

	public void SetResourceValue (ResourceData resourceData)
	{
		SetResourceValue(resourceData.id, resourceData.value);
	}

	public void SetResourceValue (ResourceId id, int value)
	{
		if (id == ResourceId.None)
			return;

		ResourceData resourceData = GetLocalResourceData(id);
		resourceData.value = value;

		GameManager.Instance.uiManager.SetValue(resourceData.Clone());
	}

	public void SetResourcesValue (ResourceData[] resources)
	{
		foreach (ResourceData resourceData in resources)
		{
			SetResourceValue(resourceData);
		}
	}

	public void Log ()
	{
# if UNITY_EDITOR
		foreach (ResourceData data in resources)
			Debug.Log($"{data.id}: {data.value}");
# endif
	}

	/**
	 * Get the associated data of a resource, create a default one if missing
	 */
	private ResourceData GetLocalResourceData (ResourceId id)
	{
		ResourceData resourceData = resources.Find(data => data.id == id);

		if (resourceData == null)
		{
			resourceData = new ResourceData() { id = id };
			resources.Add(resourceData);
		}

		return resourceData;
	}

}
