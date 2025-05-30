using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SaveItemKey
{
	public static string Date = "Date";										// string
	public static string Version = "Version";								// string
	public static string State = "State";									// GameState
	public static string VarsValue = "VarsValue";							// List<KeyValuePair<GameVarId, int>>
	public static string StartDayVarsValues = "StartDayVarsValues";         // List<KeyValuePair<GameVarId, int>>
	public static string CharactersPlayedToday = "CharactersPlayedToday";   // List<string>
	public static string DialogsUsed = "DialogsUsed";                       // List<string>
	public static string DialogsPlayedToday = "DialogsPlayedToday";         // int
	public static string DialogStarted = "DialogStarted";                   // string
	public static string EventsDay = "EventsDay";							// List<KeyValuePair<string, int>>
	public static string EventsUsed = "EventsUsed";                         // List<string
	public static string EventsPlayedToday = "EventsPlayedToday";           // List<string>
	public static string EndingsUsed = "EndingsUsed";						// List<string>
	public static string MusicVolume = "MusicVolume";						// float
	public static string MusicMute = "MusicMute";							// bool
}

public class SaveManager : MonoBehaviour
{
	private static string VERSION = "0.1";
#pragma warning disable CS0414
	private static string WEB_SAVE_KEY = "HOPE_save";
#pragma warning restore CS0414

	public bool debug = false;

	[HideInInspector] public List<SaveItem> saveData { get; private set; }

	private string _saveFileName = "hopeIsHere.json";
	private string _savePath;

	public void Init ()
	{
		_savePath = Path.Combine(Application.persistentDataPath, _saveFileName);
		saveData = new();
		AddToSaveData(SaveItemKey.Version, VERSION);
	}

	public void AddToSaveData (string key, object data)
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

	public T GetSaveData<T> (string key)
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

	public async Awaitable LoadData ()
	{
		try
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			string jsonData = PlayerPrefs.GetString(WEB_SAVE_KEY);

			if (!string.IsNullOrEmpty(jsonData))
			{
				saveData = JsonConvert.DeserializeObject<List<SaveItem>>(jsonData);

				if (debug)
					Debug.Log($"Game loaded successfully");
			}
			else
				Debug.Log($"No save file found.");
#else
			if (File.Exists(_savePath))
			{
				string jsonData = await File.ReadAllTextAsync(_savePath);
				saveData = JsonConvert.DeserializeObject<List<SaveItem>>(jsonData);

				if (debug)
					Debug.Log($"Game loaded successfully from {_savePath}");
			}
			else
				Debug.Log($"No save file found.");
#endif
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

#if UNITY_WEBGL && !UNITY_EDITOR
			PlayerPrefs.SetString(WEB_SAVE_KEY, jsonData);
			PlayerPrefs.Save();

			if (debug)
				Debug.Log($"Game saved successfully");
#else
			await File.WriteAllTextAsync(_savePath, jsonData);

			if (debug)
				Debug.Log($"Game saved successfully at {_savePath}");
#endif
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
#if UNITY_WEBGL && !UNITY_EDITOR
			PlayerPrefs.SetString(WEB_SAVE_KEY, null);

			if (debug)
				Debug.Log($"Save file deleted successfully");
#else
			if (File.Exists(_savePath))
			{
				File.Delete(_savePath);

				if (debug)
					Debug.Log($"Save file deleted successfully");
			}
#endif
		}
		catch (Exception e)
		{
			Debug.LogError($"Failed to delete save: {e.Message}");
		}
	}

	public bool HasKey (string key)
	{
		return saveData.Any(entry => entry.key == key);
	}

	public bool HasSave ()
	{
#if UNITY_WEBGL && !UNITY_EDITOR
		return !string.IsNullOrEmpty(PlayerPrefs.GetString(WEB_SAVE_KEY));
#else
		return File.Exists(_savePath);
#endif
	}

}
