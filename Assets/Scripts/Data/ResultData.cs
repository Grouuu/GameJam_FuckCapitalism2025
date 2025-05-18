using System;
using UnityEngine;

[Serializable]
public class EditEventDay
{
    public EventData targetEvent;
    public int setDay = -1;
    public int addDays = -1;
    public int addMinDays = -1;
    public int addMaxDays = -1;

    public void Apply ()
    {
        if (setDay != -1)
            SetDay();
        else if (addDays != -1)
            AddDays();
        else if (addMinDays != -1 && addMaxDays != -1 && addMaxDays > addMinDays)
            AddDaysRange();
	}

    private void SetDay ()
	{
        if (targetEvent != null)
            targetEvent.day = setDay;
    }

    private void AddDays ()
    {
        if (targetEvent != null)
            targetEvent.day = GameManager.Instance.resourcesManager.GetResourceValue(ResourceId.Days) + addDays;
	}

    private void AddDaysRange ()
    {
        if (targetEvent != null)
            targetEvent.day = GameManager.Instance.resourcesManager.GetResourceValue(ResourceId.Days) + UnityEngine.Random.Range(addMinDays, addMaxDays);
    }
}

[Serializable]
public class DialogResultData : ResultData
{
    [TextArea(3, 10)] public string response;
}

[Serializable]
public class ResultData
{
    public DialogData[] resetDialogsUsed;
    public EventData[] resetEventsUsed;
    public EditEventDay[] eventsDay;
    public ResourceDataGenerator[] resourcesChanges;

    // only available after called UpdateResult and cleared after called ApplyResult
    [HideInInspector] public ResourceData[] resourcesChanged => _resourcesChanged;

    [NonSerialized] private ResourceData[] _resourcesChanged;

    public void UpdateResult ()
    {
        _resourcesChanged = new ResourceData[resourcesChanges.Length];

        for (int i = 0; i < resourcesChanges.Length; i++)
        {
            ResourceData data = resourcesChanges[i].GetResourceData();

            if (data.id != ResourceId.None)
                _resourcesChanged[i] = data;
        }
    }

    public void ApplyResult ()
    {
        UnlockDialogs();
        UnlockEvents();
        UpdateResources();
        UpdateEventsDay();

        _resourcesChanged = null;
    }

    private void UnlockDialogs ()
    {
        foreach (DialogData dialogData in resetDialogsUsed)
            dialogData.isUsed = false;
    }

    private void UnlockEvents ()
    {
        foreach (EventData eventData in resetEventsUsed)
            eventData.isUsed = false;
    }

    private void UpdateResources ()
    {
        ResourcesManager rm = GameManager.Instance.resourcesManager;

        foreach (ResourceData resourceData in _resourcesChanged)
        {
            rm.AddResourceValue(resourceData);
        }
    }

    private void UpdateEventsDay ()
    {
        foreach (EditEventDay editDay in eventsDay)
        {
            editDay.Apply();
        }
    }

}
