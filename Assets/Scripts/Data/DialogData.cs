

public class DialogResultData : ResultData
{
    public string response;
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

    public void GenerateResultValue ()
	{
        yesResult.UpdateResult();
        noResult.UpdateResult();
    }

}
