using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RequirementData", menuName = "Scriptable Objects/RequirementData")]
public class RequirementData : ScriptableObject
{
    [HideInInspector] public string id = Guid.NewGuid().ToString();

    public bool IsOK ()
	{
		GameResources resources = GameManager.Instance.resourcesManager.resources;

		return true; // TODO
	}

}
