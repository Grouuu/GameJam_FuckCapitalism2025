

public class DialogResultData : ResultData
{
    public string response;

    public static DialogResultData CreateFrom (ResultData resultData)
    {
        return new()
        {
            eventsDay = resultData.eventsDay,
            varChanges = resultData.varChanges,
        };
    }
}

public class DialogData
{
    public string id;
    public string name;
    public string characterName;
    public string request;
    public int priority;
    public bool isRepeateable;
    public RequirementData[] requirements;
    public DialogResultData yesResult;
    public DialogResultData noResult;

    // runtime values
    public bool isUsed = false;

    public bool isAvailable ()
	{
        if (!IsRespectRequirements())
            return false;

        if (isUsed && !isRepeateable)
            return false;

        if (!IsResultsSafe())
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

    public bool IsResultsSafe ()
	{
        return GameManager.Instance.varsManager.IsResultSafe(yesResult) && GameManager.Instance.varsManager.IsResultSafe(noResult);
    }

    public void GenerateResultValue ()
	{
        yesResult.UpdateResult();
        noResult.UpdateResult();
    }

}
