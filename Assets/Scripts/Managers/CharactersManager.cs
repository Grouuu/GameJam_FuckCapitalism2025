using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharactersManager : MonoBehaviour
{
	[NonSerialized] private CharacterData[] _characters = new CharacterData[0];

	public bool debug = false;

	private int _numberOfDialogPriorities = 6;

	public void InitCharacters (CharacterData[] characters, DialogData[] dialogs)
	{
		if (characters == null)
		{
			Debug.LogError($"No characters to init");
			return;
		}

		Debug.Log($"Characters loaded (total: {characters.Length})");

		_characters = characters;

		if (dialogs == null)
		{
			Debug.LogError($"No dialogs to init");
			return;
		}

		Debug.Log($"Dialogs loaded (total: {dialogs.Length})");

		foreach (DialogData dialogData in dialogs)
		{
			string characterName = dialogData.characterName;
			CharacterData character = _characters.FirstOrDefault(entry => entry.name == characterName);

			if (character == null)
			{
				Debug.LogWarning($"No character found from dialog (name: {characterName})");
				continue;
			}

			character.characterDialogs.Add(dialogData);
		}
	}

	public (CharacterData, DialogData) PickDialog (string[] ignoredCharacters)
	{
		// all characters not played today with available dialogs
		CharacterData[] availableCharacters = _characters
			.Where(characterData => characterData.isAvailable())
			.Where(characterData => !ignoredCharacters.Any(id => id == characterData.name))
			.ToArray()
		;

		if (debug)
		{
			Debug.Log($"---- CHARS ----------");
			Debug.Log($"Ignored characters:");
			foreach (var charName in ignoredCharacters)
				Debug.Log($"<color=#FF0000>{charName}</color>");
			Debug.Log($"Available characters:");
			foreach (var charData in availableCharacters)
				Debug.Log($"<color=#7FFF00>{charData.name}</color>");
		}

		if (availableCharacters.Length == 0)
			return (null, null);

		List<(CharacterData, DialogData)>[] dialogsByPriority = new List<(CharacterData, DialogData)>[_numberOfDialogPriorities];
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

					if (isResourceLow && dialog.priority == 3)
					{
						lowResourcesDialogs.Add((character, dialog));
						break;
					}
				}
			}
		}

		if (debug)
		{
			Debug.Log($"---- DIALOGS ----------");
			Debug.Log($"Dialogs by priority:");
			for (var i = 0; i < dialogsByPriority.Length; i++)
			{
				var entries = dialogsByPriority[i];
				if (entries != null)
					foreach(var (c, d) in entries)
						Debug.Log($"{i} <color=#FFFFFF>{d.name}</color>");
			}
			Debug.Log("----");
			Debug.Log($"LowResources dialogs:");
			foreach (var (c, d) in lowResourcesDialogs)
				Debug.Log($"{d.priority} <color=#FFFF00>{d.name}</color>");
		}

		(CharacterData character, DialogData dialog) selected = new();

		// pick priority 0
		if (dialogsByPriority[0] != null)
			selected = dialogsByPriority[0][UnityEngine.Random.Range(0, dialogsByPriority[0].Count)];
		// pick priority 3 with low resource
		else if (lowResourcesDialogs.Count > 0)
			selected = lowResourcesDialogs[UnityEngine.Random.Range(0, lowResourcesDialogs.Count)];
		// random pick
		else
		{
			// pick priority 1
			if (dialogsByPriority[1] != null)
				selected = dialogsByPriority[1][UnityEngine.Random.Range(0, dialogsByPriority[1].Count)];
			// pick priority 2
			else if (dialogsByPriority[2] != null)
				selected = dialogsByPriority[2][UnityEngine.Random.Range(0, dialogsByPriority[2].Count)];
			// pick priority 4 over priority 5 75% of the time
			else
			{
				var priorityMax = dialogsByPriority[4] ?? dialogsByPriority[5] ?? null;
				var priorityMin = dialogsByPriority[5] ?? dialogsByPriority[4] ?? null;
				var prioritySelected = UnityEngine.Random.value > 0.25 ? priorityMax : priorityMin;

				if (prioritySelected != null)
					selected = prioritySelected[UnityEngine.Random.Range(0, prioritySelected.Count)];
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
			if (character.characterDialogs == null)
				continue;

			DialogData matchDialog = character.characterDialogs.FirstOrDefault(dialog => dialog.name == dialogName);

			if (matchDialog != null)
				return matchDialog;
		}

		return null;
	}

	public CharacterData GetCharacterByDialogName (string dialogName)
	{
		foreach (CharacterData characterData in _characters)
		{
			if (characterData.characterDialogs == null)
				continue;

			foreach (DialogData dialogData in characterData.characterDialogs)
			{
				if (dialogData.name == dialogName)
					return characterData;
			}
		}

		return null;
	}

	public void UpdateDialogsUsedSaveData ()
	{
		List<string> dialogsUsed = new();

		foreach (CharacterData charData in _characters)
		{
			if (charData.characterDialogs == null)
				continue;

			foreach (DialogData dialogData in charData.characterDialogs)
			{
				if (dialogData.isUsed)
					dialogsUsed.Add(dialogData.name);
			}
		}

		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.DialogsUsed, dialogsUsed);
	}

	public void UpdateCharactersPlayedTodaySaveData (List<string> charactersName)
	{
		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.CharactersPlayedToday, charactersName);
	}

	public void UpdateDialogStartedSaveData (string dialogName)
	{
		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.DialogStarted, dialogName);
	}

	public void UpdateTotalDialogPlayedTodaySaveData (int total)
	{
		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.DialogsPlayedToday, total);
	}

	public void ApplySave ()
	{
		List<string> dialogsUsed = GameManager.Instance.saveManager.GetSaveData<List<string>>(SaveItemKey.DialogsUsed);

		if (dialogsUsed != null)
		{
			foreach (string dialogName in dialogsUsed)
			{
				DialogData dialogData = GetDialogByName(dialogName);

				if (dialogData == null)
					continue;

				dialogData.isUsed = true;
			}
		}
	}

}
