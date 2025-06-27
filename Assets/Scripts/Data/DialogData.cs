using Cysharp.Threading.Tasks;

public class DialogResultData : ResultData
{
    public string dialogName;
    public bool isYes;

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
    public EditSceneEffect enterSceneEffets;
    public EditSceneEffect exitSceneEffets;
    public EditSceneEffect yesEnterSceneEffets;
    public EditSceneEffect yesExitSceneEffets;
    public EditSceneEffect noEnterSceneEffets;
    public EditSceneEffect noExitSceneEffets;

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

    public UniTask UpdateEnterSceneEffects ()
	{
        return GameManager.Instance.sceneEffectsManager.UpdateSceneEffects(enterSceneEffets);
	}

    public UniTask UpdateExitSceneEffects ()
    {
        return GameManager.Instance.sceneEffectsManager.UpdateSceneEffects(exitSceneEffets);
    }

    public UniTask UpdateYesEnterSceneEffects ()
    {
        return GameManager.Instance.sceneEffectsManager.UpdateSceneEffects(yesEnterSceneEffets);
    }

    public UniTask UpdateYesExitSceneEffects ()
    {
        return GameManager.Instance.sceneEffectsManager.UpdateSceneEffects(yesExitSceneEffets);
    }

    public UniTask UpdateNoEnterSceneEffects ()
    {
        return GameManager.Instance.sceneEffectsManager.UpdateSceneEffects(noEnterSceneEffets);
    }

    public UniTask UpdateNoExitSceneEffects ()
    {
        return GameManager.Instance.sceneEffectsManager.UpdateSceneEffects(noExitSceneEffets);
    }

}
