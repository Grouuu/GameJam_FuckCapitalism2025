using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DatabaseController : MonoBehaviour
{
	public VarData[] varsData = { };
	public CharacterData[] charactersData = { };
	public DialogData[] dialogsData = { };
	public EventData[] eventsData = { };
	public EndingData[] endingsData = { };

	protected DatabaseHandler handler = new();

	public async Awaitable LoadDatabase (DatabaseManager manager)
	{
		await handler.LoadDatabase(manager.Parsers, manager.FolderPath);

		varsData = handler.GetData<VarData>();
		charactersData = handler.GetData<CharacterData>();
		dialogsData = handler.GetData<DialogData>();
		eventsData = handler.GetData<EventData>();
		endingsData = handler.GetData<EndingData>();

		foreach (DialogData dialogData in dialogsData)
		{
			CharacterData character = charactersData.FirstOrDefault(entry => entry.name == dialogData.characterName);

			if (character == null)
			{
				Debug.LogWarning($"No character found from dialog (name: {dialogData.characterName})");
				continue;
			}

			character.characterDialogs.Add(dialogData);
		}
	}

}
