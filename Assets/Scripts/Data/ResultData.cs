
public class EditEventDay
{
	public string eventName;
	public int setDay = -1;
	public int addMinDays = -1;
	public int addMaxDays = -1;

	public int GetNewDay (int currentDay)
	{
		if (setDay != -1)
			return setDay;
		else if (addMinDays != -1 && addMaxDays != -1 && addMaxDays > addMinDays)
			return currentDay + UnityEngine.Random.Range(addMinDays, addMaxDays);

		return currentDay;
	}
}

public class EditVarMax
{
	public GameVarId varId;
	public int setMax = -1;
}

public class EditBuildingProgress
{
	public string buildingName;
	public int progress;
	public bool isBuilt;
}

public class ResultVarChange
{
    public GameVarId varId;
    public ChangeValueType modifierType;
    public int modifierValueMin;
    public int modifierValueMax;

    public int currentValue;

    public void GenerateRandomValue ()
	{
        currentValue = UnityEngine.Random.Range(modifierValueMin, modifierValueMax + 1);
    }
}

public class ResultData
{
    public EditEventDay[] eventsDay = { };
    public EditVarMax[] varsMax = { };
    public EditBuildingProgress[] buildingsProgress = { };
	public ResultVarChange[] varChanges = { };

    public void UpdateResult ()
    {
        foreach (ResultVarChange modifier in varChanges)
            modifier.GenerateRandomValue();
    }

    public void ApplyResult ()
    {
		UpdateResources();
		UpdateEventsDay();
		UpdateVarsMax();
		UpdateBuildingsProgress();
	}

    private void UpdateResources ()
	{
		foreach (ResultVarChange modifier in varChanges)
		{
            if (modifier.modifierType == ChangeValueType.Add)
                GameManager.Instance.varsManager.AddValueToVar(modifier.varId, modifier.currentValue);
            else if (modifier.modifierType == ChangeValueType.Set)
                GameManager.Instance.varsManager.SetValueToVar(modifier.varId, modifier.currentValue);
        }
    }

    private void UpdateEventsDay ()
	{
		foreach (EditEventDay editDay in eventsDay)
		{
			EventData eventData = GameManager.Instance.eventsManager.GetEventByName(editDay.eventName);

			if (eventData != null)
				eventData.day = editDay.GetNewDay(GameManager.Instance.varsManager.GetVarValue(GameVarId.Day));
		}
	}

	private void UpdateVarsMax ()
	{
		foreach (EditVarMax varMax in varsMax)
			GameManager.Instance.varsManager.SetValueMax(varMax.varId, varMax.setMax);
	}

	private void UpdateBuildingsProgress ()
	{
		foreach (EditBuildingProgress buildingProgress in buildingsProgress)
		{
			GameManager.Instance.buildingsManager.SetBuildingProgress(buildingProgress.buildingName, buildingProgress.progress);
			GameManager.Instance.buildingsManager.SetBuildingIsBuilt(buildingProgress.buildingName, buildingProgress.isBuilt);
		}
	}

}
