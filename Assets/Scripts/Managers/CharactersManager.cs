using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharactersManager : MonoBehaviour
{
	[NonSerialized] private CharacterData[] _characters = new CharacterData[0];

	private int _maxPriorityGrade = 20;

	public void InitCharacters (CharacterData[] characters, DialogData[] dialogs)
	{
		if (characters == null)
		{
			Debug.LogError($"No characters to init");
			return;
		}

		_characters = characters;

		Dictionary<string, List<DialogData>> mapDialogs = new();

		foreach (DialogData dialogData in dialogs)
		{
			string characterName = dialogData.characterName;
			CharacterData character = _characters.First(entry => entry.name == characterName);

			if (!mapDialogs.ContainsKey(characterName))
				mapDialogs.Add(characterName, new());

			if (mapDialogs.TryGetValue(characterName, out List<DialogData> listDialogs))
				listDialogs.Add(dialogData);
		}

		foreach (CharacterData character in _characters)
		{
			if (mapDialogs.TryGetValue(character.name, out List<DialogData> listDialogs))
				character.characterDialogs = listDialogs.ToArray();
		}
	}

	public (CharacterData, DialogData) PickDialog (string[] ignoredCharacters)
	{
		// all characters not played today with available dialogs
		CharacterData[] availableCharacters = _characters
			.Where(characterData => characterData.isAvailable())
			.Where(characterData => !ignoredCharacters.Any(id => id == characterData.id))
			.ToArray()
		;

		if (availableCharacters.Length == 0)
			return (null, null);

		List<(CharacterData, DialogData)>[] dialogsByPriority = new List<(CharacterData, DialogData)>[_maxPriorityGrade];
		List<(CharacterData, DialogData)> lowResourcesDialogs = new();

		foreach (CharacterData character in availableCharacters)
		{
			foreach (DialogData dialog in character.characterDialogs)
			{
				if (!dialog.isAvailable())
					continue;

				if (dialogsByPriority[dialog.priority] == null)
					dialogsByPriority[dialog.priority] = new();

				dialogsByPriority[dialog.priority].Add((character, dialog));

				foreach (GameVarId varId in character.relatedGameVars)
				{
					bool isResourceLow = GameManager.Instance.varsManager.IsVarLow(varId);

					if (isResourceLow)
					{
						lowResourcesDialogs.Add((character, dialog));
						break;
					}
				}
			}
		}

		(CharacterData character, DialogData dialog) selected = new();

		// pick priority 0
		if (dialogsByPriority[0] != null)
			selected = dialogsByPriority[0][UnityEngine.Random.Range(0, dialogsByPriority[0].Count)];
		// pick low resource
		else if (lowResourcesDialogs.Count > 0)
			selected = lowResourcesDialogs[UnityEngine.Random.Range(0, lowResourcesDialogs.Count)];
		// random pick on highest priority
		else
		{
			for (int i = 1; i < dialogsByPriority.Length; i++)
			{
				List<(CharacterData, DialogData)> samePriorityDialogs = dialogsByPriority[i];

				if (samePriorityDialogs == null)
					continue;

				selected = samePriorityDialogs[UnityEngine.Random.Range(0, samePriorityDialogs.Count)];
				break;
			}
		}

		return selected;
	}

	public void SetDialogUsed (string dialogName, bool isUsed)
	{
		DialogData dialogData = GetDialogByName(dialogName);

		if (dialogData != null)
			dialogData.isUsed = isUsed;
	}

	public DialogData GetDialogByName (string dialogName)
	{
		foreach (CharacterData character in _characters)
		{
			DialogData matchDialog = character.characterDialogs.First(dialog => dialog.name == dialogName);

			if (matchDialog == null)
				return matchDialog;
		}

		return null;
	}

	public DialogData GetDialogById (string dialogId)
	{
		foreach (CharacterData character in _characters)
		{
			DialogData matchDialog = character.characterDialogs.First(dialog => dialog.id == dialogId);

			if (matchDialog == null)
				return matchDialog;
		}

		return null;
	}

}
