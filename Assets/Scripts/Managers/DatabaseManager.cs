using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public class DatabaseParser : MonoBehaviour
{
	public string databaseFileName;

	public string GetJsonFileName () => $"{databaseFileName}.json";

	public virtual Type GetDataType () => typeof(object);
	public virtual T[] GetData<T> () => default;
	public virtual void ParseData (string data) { }
}

public class DatabaseManager : MonoBehaviour
{
	[SerializeField] public DefaultAsset jsonFolderPath;

	private DatabaseParser[] _parsers;

	public async Awaitable LoadDatabase ()
	{
		if (jsonFolderPath == null)
		{
			Debug.LogWarning($"No valid folder set for the database files (no reference)");
			return;
		}

		string folderPath = AssetDatabase.GetAssetPath(jsonFolderPath);

		if (string.IsNullOrEmpty(folderPath))
		{
			Debug.LogWarning($"No valid folder set for the database files (url: {folderPath})");
			return;
		}

		if (!Directory.Exists(folderPath))
		{
			Debug.LogWarning($"No valid folder set for the database file (reference is not a folder, url: {folderPath})");
			return;
		}

		string[] jsonFilePaths = AssetDatabase.FindAssets("", new[] { folderPath })
			.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
			.ToArray()
		;

		foreach (string filePath in jsonFilePaths)
		{
			await LoadJsonData(filePath);
		}
	}

	public T[] GetData<T> ()
	{
		var parser = Array.Find(_parsers, entry => entry.GetDataType() == typeof(T));

		if (parser != null)
			return parser.GetData<T>();

		return default;
	}

	private void OnEnable ()
	{
		_parsers = GetComponents<DatabaseParser>();
	}

	private async Awaitable LoadJsonData (string filePath)
	{
		string fileName = Regex.Match(filePath, @"[^\/]*$").Value;
		var parser = Array.Find(_parsers, entry => entry.GetJsonFileName() == fileName);

		if (parser != null)
		{
			string fileContent = await File.ReadAllTextAsync(filePath);
			parser.ParseData(fileContent);
		}
	}
}
