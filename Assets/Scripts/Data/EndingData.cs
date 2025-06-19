using System.Threading.Tasks;

public class EndingData
{
    public string id;
    public string name;
    public bool isWinEnding;
    public string headerFileName;
    public RequirementData requirements;
    public ResultData result;
    public EditSceneEffect enterSceneEffets;
    public EditSceneEffect exitSceneEffets;

    // runtime values
    public bool isUsed = false;

    public bool IsRespectRequirements ()
    {
        if (requirements != null && requirements.IsOK())
            return true;

        return false;
    }

    public Task UpdateEnterSceneEffects ()
    {
        return GameManager.Instance.sceneEffectsManager.UpdateSceneEffects(enterSceneEffets);
    }

    public Task UpdateExitSceneEffects ()
    {
        return GameManager.Instance.sceneEffectsManager.UpdateSceneEffects(exitSceneEffets);
    }
}
