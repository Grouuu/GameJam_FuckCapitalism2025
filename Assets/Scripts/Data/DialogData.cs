using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogData", menuName = "Scriptable Objects/DialogData")]
public class DialogData : ScriptableObject
{
    public string dialogId;
    [TextArea(3, 10)]
    public string request;
    public int priority;
    public bool isRepeateable;
    public RequirementData[] requirements;
    public DialogResultData yesResult;
    public DialogResultData noResult;

    [HideInInspector] public string id = Guid.NewGuid().ToString();

    // runtime values
    [HideInInspector] [NonSerialized] public bool isUsed = false;

    public bool isAvailable ()
	{
        if (!IsRespectRequirements())
            return false;

        if (isUsed && !isRepeateable)
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
