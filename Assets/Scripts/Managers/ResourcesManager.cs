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

[Serializable]
public class ResourceLimit
{
	public ResourceId id;
	public int max;
	public int min;
	public int low;
}

public class ResourcesManager : MonoBehaviour
{
	public ResourceLimit[] resourceLimits;

	private List<ResourceData> _resources = new();

	public int GetResourceValue (ResourceId id)
	{
		return GetResourceData(id).value;
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

		ResourceData resourceData = GetResourceData(id);
		resourceData.value = value;

		GameManager.Instance.uiManager.SetResourceValue(resourceData.Clone());
	}

	public void SetResourcesValue (ResourceData[] resources)
	{
		foreach (ResourceData resourceData in resources)
		{
			SetResourceValue(resourceData);
		}
	}

	public bool IsResourceLow (ResourceId id)
	{
		ResourceLimit limits = GetResourceLimit(id);

		if (limits == null)
			return false;

		return GetResourceValue(id) <= limits.low;
	}

	public void Log ()
	{
# if UNITY_EDITOR
		foreach (ResourceData data in _resources)
			Debug.Log($"{data.id}: {data.value}");
# endif
	}

	/**
	 * Get the associated data of a resource, create a default one if missing
	 */
	private ResourceData GetResourceData (ResourceId id)
	{
		ResourceData resourceData = _resources.Find(data => data.id == id);

		if (resourceData == null)
		{
			resourceData = new ResourceData(id, -1);
			_resources.Add(resourceData);
		}

		return resourceData;
	}

	private ResourceLimit GetResourceLimit (ResourceId id)
	{
		return Array.Find(resourceLimits, entry => entry.id == id);
	}

}
