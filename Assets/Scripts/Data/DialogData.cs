using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogData", menuName = "Scriptable Objects/DialogData")]
public class DialogData : ScriptableObject
{
    [TextArea(3, 10)]
    public string request;
    public bool isRepetable;
    public int priority;
    public RequirementData[] requirements;
    public ResultData yesResult;
    public ResultData noResult;

    [HideInInspector] public string id = Guid.NewGuid().ToString();
    [HideInInspector] public bool isUsed;

    public bool isAvailable ()
	{
        if (!IsRespectRequirements())
            return false;

        if (isUsed && !isRepetable)
            return false;

        return true;
	}

    public bool IsRespectRequirements ()
	{
        if (requirements == null)
            return true;

        foreach(RequirementData requirement in requirements)
		{
            if (!requirement.IsOK())
                return false;
		}

        return true;
	}

}
