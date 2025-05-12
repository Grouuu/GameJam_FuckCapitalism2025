using System;

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
public class ResultData
{
    public DialogData[] resetDialogsUsed;
    public EventData[] resetEventsUsed;
    public ResourceData[] resourceChanges;
    public EditEventDay[] eventsDay;

    public void ApplyResult ()
    {
        UnlockDialogs();
        UnlockEvents();
        UpdateResources();
        UpdateEventsDay();
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

        foreach (ResourceData resourceData in resourceChanges)
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
