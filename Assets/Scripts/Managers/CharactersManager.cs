using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharactersManager : MonoBehaviour
{
	public CharacterData[] characters;

	private int _maxPriorityGrade = 10;

	public (CharacterData, DialogData) PickDialog (string[] ignoredCharacters)
	{
		// all characters not played today with available dialogs
		CharacterData[] availableCharacters = characters
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

				bool isResourceLow = GameManager.Instance.resourcesManager.IsResourceLow(character.relatedResource);

				if (isResourceLow)
					lowResourcesDialogs.Add((character, dialog));
			}
		}

		(CharacterData character, DialogData dialog) selected = new();

		// pick priority 0
		if (dialogsByPriority[0] != null)
			selected = dialogsByPriority[0][Random.Range(0, dialogsByPriority[0].Count)];
		// pick low resource
		else if (lowResourcesDialogs.Count > 0)
			selected = lowResourcesDialogs[Random.Range(0, lowResourcesDialogs.Count)];
		// random pick on highest priority
		else
		{
			for (int i = 1; i < dialogsByPriority.Length; i++)
			{
				List<(CharacterData, DialogData)> samePriorityDialogs = dialogsByPriority[i];

				if (samePriorityDialogs == null)
					continue;

				selected = samePriorityDialogs[Random.Range(0, samePriorityDialogs.Count)];
				break;
			}
		}

		return selected;
	}

}
