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

	public ResourceData (ResourceId id, int value)
	{
		this.id = id;
		this.value = value;
	}

	public ResourceData Clone () => new(id, value);
}

[Serializable]
public class ResourceDataGenerator
{
	public ResourceId resourceId;
	public int fixedValue;
	public int randomMin;
	public int randomMax;
	public AnimationCurve distribution;

	public ResourceData GetResourceData ()
	{
		if (randomMin == 0 && randomMax == 0)
			return new ResourceData(resourceId, fixedValue);

		if (distribution.length == 0)
		{
			Debug.LogWarning($"ResourceDataGenerator is missing a valid distribution ({resourceId}), fallback to fixed value");
			return new ResourceData(resourceId, fixedValue);
		}

		float rng = UnityEngine.Random.Range(0f, 1f);
		float evaluate = distribution.Evaluate(rng);
		float value = Mathf.Lerp(randomMin, randomMax, evaluate);
		int roundedValue = Mathf.RoundToInt(value);

		return new ResourceData(resourceId, roundedValue);
	}
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
			resourceData = new ResourceData(id, -1);
			resources.Add(resourceData);
		}

		return resourceData;
	}

}
