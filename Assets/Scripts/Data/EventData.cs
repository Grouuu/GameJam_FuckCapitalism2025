
public class EventData
{
    public string id;
    public string name;
    public EventType type;
    public int randomWeight;
    public string title;
    public string description;
    public string headerFileName;
    public int priority;
    public bool isRepeateable;
    public RequirementData requirements;
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

        if (!IsResultsSafe())
            return false;

        return true;
    }

    private bool IsRespectRequirements ()
    {
        if (requirements != null && !requirements.IsOK())
            return false;

        return true;
    }

    public bool IsResultsSafe ()
    {
        return GameManager.Instance.varsManager.IsResultSafe(result);
    }

    public void GenerateResultValue ()
    {
        result.UpdateResult();
    }

}
