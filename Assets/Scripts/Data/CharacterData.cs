
using System.Collections.Generic;

public class CharacterData
{
	public string id;
    public string name;
	public GameVarId[] relatedGameVars;
	public string avatarFileName;
	public RequirementData requirements;

	public List<DialogData> characterDialogs = new();

	public bool isAvailable ()
	{
		if (characterDialogs == null)
			return false;

		if (requirements != null && !requirements.IsOK())
			return false;

		foreach (DialogData dialog in characterDialogs)
		{
			if (dialog.isAvailable())
				return true;
		}

		return false;
	}

}
