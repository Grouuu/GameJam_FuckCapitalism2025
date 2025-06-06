using System;
using UnityEngine;

public class DatabaseHandler
{
	protected DatabaseParser[] parsers = new DatabaseParser[0];

	public async Awaitable LoadDatabase (DatabaseParser[] parsers, string folderPath)
	{
		if (parsers == null)
		{
			Debug.LogWarning($"Invalid parsers reference");
			return;
		}

		this.parsers = parsers;
		folderPath = folderPath.Trim();

		if (!folderPath.EndsWith("/"))
			folderPath = $"{folderPath}/";

		foreach (DatabaseParser parser in this.parsers)
			await LoadJsonData(parser, folderPath);
	}

	public T[] GetData<T> ()
	{
		var parser = Array.Find(parsers, parser => parser.GetDataType() == typeof(T));

		if (parser != null)
			return parser.GetData<T>();

		return new T[0];
	}

	protected async Awaitable LoadJsonData (DatabaseParser parser, string folderPath)
	{
		if (parser == null)
		{
			Debug.LogWarning($"Invalid parser to load: null");
			return;
		}

		string fileContent;

		try
		{
			ResourceRequest request = Resources.LoadAsync<TextAsset>($"{folderPath}{parser.databaseFileName}");

			await request;

			TextAsset file = (TextAsset) request.asset;
			fileContent = file.text;
		}
		catch (Exception e)
		{
			Debug.LogError($"Error when loading the database json: {e.Message}");
			return;
		}

		if (string.IsNullOrEmpty(fileContent))
		{
			Debug.LogWarning($"Invalid data loaded: empty");
			return;
		}

		try
		{
			parser.ParseData(fileContent);
		}
		catch(Exception e)
		{
			Debug.LogError($"Error when parsing the database json data: {e.Message}");
		}
	}

}
