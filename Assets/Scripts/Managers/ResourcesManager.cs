using System;
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

	[HideInInspector] public bool isShow = true;
}

public class ResourcesManager : MonoBehaviour
{
	[HideInInspector] public ResourceData[] resources;

	public void Log ()
	{
# if UNITY_EDITOR
		foreach (ResourceData data in resources)
			Debug.Log($"{data.id}: {data.value}");
# endif
	}

}
