using System;
using UnityEngine;

// TODO move enums to better place
public enum EventArt
{
    None,
}

public enum EventPostFX
{
    None,
}

public enum EventSoundFX
{
    None,
}

[CreateAssetMenu(fileName = "EventData", menuName = "Scriptable Objects/EventData")]
public class EventData : ScriptableObject
{
    public string eventId;
    public string title;
    [TextArea(3, 10)] public string description;
    public int priority;
    public bool isRepeateable;
    public int initialDay = -1;
    public RequirementData[] requirements;
    public ResultData result;

    [Space(10)]
    public EventArt art;
    public EventPostFX postFX;
    public EventSoundFX soundFX;

    [HideInInspector] public string id = Guid.NewGuid().ToString();

    // runtime values
    [HideInInspector] [NonSerialized] public int day = -1;
    [HideInInspector] [NonSerialized] public bool isUsed = false;

    public bool isAvailable ()
    {
        if (!IsRespectRequirements())
            return false;

        if (isUsed && !isRepeateable)
            return false;

        return true;
    }

    private bool IsRespectRequirements ()
    {
        foreach (RequirementData requirement in requirements)
        {
            if (!requirement.IsOK())
                return false;
        }

        return true;
    }

	private void OnEnable ()
    {
        day = initialDay;
    }

}
