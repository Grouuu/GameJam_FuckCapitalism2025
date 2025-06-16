
public class EventData
{
    public string id;
    public string name;
    public EventDataType type;
    public int randomWeight;
    public string headerFileName;
    public int priority;
    public bool isRepeateable;
    public RequirementData requirements;
    public ResultData result;
    public EditAnimations enterAnimations;
    public EditAnimations exitAnimations;

    public string title => LocalizationUtils.GetText(name, LocCat.EventsTitles);
    public string description => LocalizationUtils.GetText(name, LocCat.EventsDescriptions);

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

    public void UpdateEnterAnimations ()
    {
        GameManager.Instance.animationsManager.UpdateAnimations(enterAnimations);
    }

    public void UpdateExitAnimations ()
    {
        GameManager.Instance.animationsManager.UpdateAnimations(exitAnimations);
    }

}
