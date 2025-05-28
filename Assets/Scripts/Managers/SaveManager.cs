using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum SaveItemKey
{
	Date,					// string
	Version,                // string
	State,                  // GameState
	VarsValue,              // List<KeyValuePair<GameVarId, int>>
	StartDayVarsValues,     // List<KeyValuePair<GameVarId, int>>
	DialogsUsed,            // List<string>
	EventsDay,              // List<KeyValuePair<string, int>>
	EventsUsed,				// List<string>
	EndingsUsed,            // List<string>
}

public class SaveManager : MonoBehaviour
{
	private static string VERSION = "0.1";

	public bool debug = false;

	[SerializeField] private string _saveFileName = "hopeIsHere.json";

	[HideInInspector] public List<SaveItem> saveData { get; private set; }

	private string _savePath;

	public void AddToSaveData (SaveItemKey key, object data)
	{
		SaveItem item = saveData.Find(entry => entry.key == key);

		if (item == null)
		{
			item = new();
			item.key = key;
			saveData.Add(item);
		}

		item.json = JsonConvert.SerializeObject(data);
	}

	public T GetSaveData<T> (SaveItemKey key)
	{
		SaveItem item = saveData.Find(entry => entry.key == key);

		if (item == null)
			return default;

		try
		{
			return JsonConvert.DeserializeObject<T>(item.json);
		}
		catch (Exception e)
		{
			Debug.LogError($"Incorrect save data: {e.Message}");
		}

		return default;
	}

	public bool HasSave ()
	{
		return File.Exists(_savePath);
	}

	public async Awaitable LoadData ()
	{
		try
		{
			if (File.Exists(_savePath))
			{
				string jsonData = await File.ReadAllTextAsync(_savePath);
				saveData = JsonConvert.DeserializeObject<List<SaveItem>>(jsonData);

				if (debug)
					Debug.Log($"Game loaded successfully from {_savePath}");
			}
			else
				Debug.Log($"No save file found.");
		}
		catch (Exception e)
		{
			Debug.LogError($"Failed to load game: {e.Message}");
		}
	}

	public async Awaitable SaveData ()
	{
		try
		{
			// update save date
			AddToSaveData(SaveItemKey.Date, DateTime.Now);

			string jsonData = JsonConvert.SerializeObject(saveData);
			await File.WriteAllTextAsync(_savePath, jsonData);

			if (debug)
				Debug.Log($"Game saved successfully at {_savePath}");
		}
		catch (Exception e)
		{
			Debug.LogError($"Failed to save game: {e.Message}");
		}
	}

	public void DeleteSave ()
	{
		try
		{
			if (File.Exists(_savePath))
			{
				File.Delete(_savePath);

				if (debug)
					Debug.Log($"Save file deleted successfully");
			}
		}
		catch (Exception e)
		{
			Debug.LogError($"Failed to delete save: {e.Message}");
		}
	}

	private void Awake ()
	{
		_savePath = Path.Combine(Application.persistentDataPath, _saveFileName);
		saveData = new();
		AddToSaveData(SaveItemKey.Version, VERSION);
	}

}
