using UnityEngine;

public class EndingData
{
    public string id;
    public string name;
    public string title;
    public string description;
    public bool isWinEnding;
    public string headerFileName;
    public RequirementData requirements;
    public ResultData result;

    // runtime values
    public bool isUsed = false;

    public bool IsRespectRequirements ()
    {
        if (requirements != null && requirements.IsOK())
            return true;

        return false;
    }
}
