
public class EventData
{
    public string id;
    public string name;
    public string title;
    public string description;
    public int priority;
    public bool isRepeateable;
    public int initialDay = -1;
    public RequirementData[] requirements;
    public ResultData result;

    // runtime values
    public int day = -1;
    public bool isUsed = false;

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

    public void GenerateResultValue ()
    {
        result.UpdateResult();
    }

}
