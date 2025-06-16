using UnityEngine;

public class EndingData
{
    public string id;
    public string name;
    public bool isWinEnding;
    public string headerFileName;
    public RequirementData requirements;
    public ResultData result;
    public EditAnimations enterAnimations;
    public EditAnimations exitAnimations;

    public string title => LocalizationUtils.GetText(name, LocCat.EndingsTitles);
    public string description => LocalizationUtils.GetText(name, LocCat.EndingsDescriptions);

    // runtime values
    public bool isUsed = false;

    public bool IsRespectRequirements ()
    {
        if (requirements != null && requirements.IsOK())
            return true;

        return false;
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
