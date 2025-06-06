using UnityEngine;
using System;

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
	public DatabaseParser[] Parsers => GetComponents<DatabaseParser>();
	public string FolderPath => _jsonFolderPath;

	private DatabaseParser[] _parsers;
	private string _jsonFolderPath = "Database";

	public async Awaitable LoadDatabase ()
	{
		foreach (DatabaseParser parser in _parsers)
			await LoadJsonData(parser);
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
		_parsers = Parsers;
	}

	private async Awaitable LoadJsonData (DatabaseParser parser)
	{
		if (parser != null)
		{
			ResourceRequest request = Resources.LoadAsync<TextAsset>($"{_jsonFolderPath}/{parser.databaseFileName}");

			await request;

			TextAsset file = (TextAsset) request.asset;
			string fileContent = file.text;
			parser.ParseData(fileContent);
		}
	}

}
