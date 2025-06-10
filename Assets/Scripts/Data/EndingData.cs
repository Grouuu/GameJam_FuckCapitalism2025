using UnityEngine;

public class EndingData
{
    public string id;
    public string name;
    public string title;
    public string description;
    public bool isWinEnding;
    public string headerFileName;
    public RequirementData[] requirements;
    public ResultData result;

    // runtime values
    public bool isUsed = false;

    public bool IsRespectRequirements ()
    {
        if (requirements == null)
            return false;

        foreach (RequirementData requirement in requirements)
        {
            if (requirement.IsOK())
                return true;
        }

        return false;
    }
}
