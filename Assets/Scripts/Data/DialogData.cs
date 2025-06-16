

public class DialogResultData : ResultData
{
    public string dialogName;
    public bool isYes;

    public string response => LocalizationUtils.GetText(dialogName, isYes ? LocCat.DialogsYes : LocCat.DialogsNo);

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
    public int priority;
    public bool isRepeateable;
    public RequirementData requirements;
    public DialogResultData yesResult;
    public DialogResultData noResult;
    public EditAnimations enterAnimations;
    public EditAnimations exitAnimations;
    public EditAnimations yesEnterAnimations;
    public EditAnimations yesExitAnimations;
    public EditAnimations noEnterAnimations;
    public EditAnimations noExitAnimations;

    public string request => LocalizationUtils.GetText(name, LocCat.DialogsRequests);

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
        if (requirements != null && !requirements.IsOK())
            return false;

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

    public void UpdateEnterAnimations ()
	{
        GameManager.Instance.animationsManager.UpdateAnimations(enterAnimations);
	}

    public void UpdateExitAnimations ()
    {
        GameManager.Instance.animationsManager.UpdateAnimations(exitAnimations);
    }

    public void UpdateYesEnterAnimations ()
    {
        GameManager.Instance.animationsManager.UpdateAnimations(yesEnterAnimations);
    }

    public void UpdateYesExitAnimations ()
    {
        GameManager.Instance.animationsManager.UpdateAnimations(yesExitAnimations);
    }

    public void UpdateNoEnterAnimations ()
    {
        GameManager.Instance.animationsManager.UpdateAnimations(noEnterAnimations);
    }

    public void UpdateNoExitAnimations ()
    {
        GameManager.Instance.animationsManager.UpdateAnimations(noExitAnimations);
    }

}
