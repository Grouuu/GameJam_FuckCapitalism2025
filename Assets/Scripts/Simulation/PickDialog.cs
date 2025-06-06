using System.Collections.Generic;
using System.Linq;

public class PickDialog
{
	protected DatabaseController database;

	protected List<string> ignoredCharacters = new();
	protected int dialogsPlayed = 0;
	protected int maxDialogs = 0;

	public void Init (DatabaseController database, int maxDialogs)
	{
		this.database = database;
		this.maxDialogs = maxDialogs;
	}

	public void Reset ()
	{
		ignoredCharacters = new();
		dialogsPlayed = 0;
	}

	public (CharacterData, DialogData) GetNextDialog (string forceDialogName = null)
	{
		CharacterData characterData = null;
		DialogData dialogData = null;

		if (dialogsPlayed < maxDialogs)
		{
			if (!string.IsNullOrEmpty(forceDialogName))
			{
				// force a specific dialog
				dialogData = database.GetDialogData(forceDialogName);
				characterData = dialogData != null ? database.GetCharacterData(dialogData.characterName) : null;
			}
			else
				// pick a random dialog
				(characterData, dialogData) = Next();
		}

		if (characterData != null && dialogData != null)
			UpdatePick(characterData, dialogData);
		else
			Reset();

		return (characterData, dialogData);
	}

	protected void UpdatePick (CharacterData characterData, DialogData dialogData)
	{
		ignoredCharacters.Add(characterData.name);
		dialogsPlayed++;

		dialogData.isUsed = true;
		dialogData.GenerateResultValue();
	}

	protected (CharacterData, DialogData) Next ()
	{
		// all characters not played today with available dialogs
		CharacterData[] availableCharacters = database.charactersData
			.Where(characterData => characterData.isAvailable())
			.Where(characterData => !ignoredCharacters.Any(id => id == characterData.name))
			.ToArray()
		;

		if (availableCharacters.Length == 0)
			return (null, null);


		Dictionary<int, DialogData[]> dialogsByPriority = database.dialogsData
			.GroupBy(dialogData => dialogData.priority)
			.ToDictionary(group => group.Key, group => group.ToArray())
		;

		DialogData[] dialogsLowResources = database.dialogsData
			.Where(dialogData => database.GetCharacterData(dialogData.characterName).relatedGameVars.Any(varId => database.IsVarLow(varId)))
			.ToArray()
		;

		CharacterData characterData = null;
		DialogData dialogData = null;

		// pick priority 0
		if (dialogsByPriority.TryGetValue(0, out DialogData[] dialogsPriorityZero))
			dialogData = dialogsPriorityZero[UnityEngine.Random.Range(0, dialogsPriorityZero.Length)];
		// pick low resource
		else if (dialogsLowResources.Length > 0)
			dialogData = dialogsLowResources[UnityEngine.Random.Range(0, dialogsLowResources.Length)];
		// pick on highest priority
		else
		{
			DialogData[] dialogsPriorityHigher = dialogsByPriority
				.OrderBy(entry => entry.Key)
				.Select(entry => entry.Value)
				.FirstOrDefault(entry => entry != null && entry.Length > 0)
			;

			if (dialogsPriorityHigher != null)
				dialogData = dialogsPriorityHigher[UnityEngine.Random.Range(0, dialogsPriorityHigher.Length)];
		}

		if (dialogData != null)
			characterData = database.GetCharacterData(dialogData.characterName);

		return (characterData, dialogData);
	}
}
