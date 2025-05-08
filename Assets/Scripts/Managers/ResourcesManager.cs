using System;
using UnityEngine;


[Serializable]
public struct GameResources
{
	public int scraps;
	public int energy;
	public int trust;
	public int population;
	public int science;
}

public class ResourcesManager : MonoBehaviour
{
	[HideInInspector] public GameResources resources;

	public void Log ()
	{
		Debug.Log($"scraps: {resources.scraps}");
		Debug.Log($"energy: {resources.energy}");
		Debug.Log($"trust: {resources.trust}");
		Debug.Log($"population: {resources.population}");
		Debug.Log($"science: {resources.science}");
	}

}
