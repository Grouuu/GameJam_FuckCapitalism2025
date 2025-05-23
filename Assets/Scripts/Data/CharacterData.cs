
public class CharacterData
{
	public string id;
    public string name;
    public string displayName;
	public GameVarId[] relatedGameVars;
	public string avatarFileName;

	public DialogData[] characterDialogs;

	public bool isAvailable ()
	{
		if (characterDialogs == null)
			return false;

		foreach (DialogData dialog in characterDialogs)
		{
			if (dialog.isAvailable())
				return true;
		}

		return false;
	}

}
