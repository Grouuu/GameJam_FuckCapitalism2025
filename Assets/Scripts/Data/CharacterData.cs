using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string characterName;
	[TextArea(3, 10)]
	public string characterBackground;
	public string characterVoice;
	public string characterAge;
	public string characterRoles;
	public string characterNeeds;
	public string characterDesires;
	public string characterQuirks;
	public string characterFears;
	public Sprite characterAvatar;
    public DialogData[] characterDialogs;

    [HideInInspector] public string id = Guid.NewGuid().ToString();

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
