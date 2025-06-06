
using System.Collections.Generic;

public class CharacterData
{
	public string id;
    public string name;
    public string displayName;
	public GameVarId[] relatedGameVars;
	public string avatarFileName;
	public RequirementData[] requirements;

	public List<DialogData> characterDialogs = new();

	public bool isAvailable ()
	{
		if (characterDialogs == null)
			return false;

		if (requirements != null)
		{
			foreach (RequirementData requirement in requirements)
			{
				if (!requirement.IsOK())
					return false;
			}
		}

		foreach (DialogData dialog in characterDialogs)
		{
			if (dialog.isAvailable())
				return true;
		}

		return false;
	}

}
