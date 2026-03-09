using UnityEngine;
using System;
using Grouuu.Types;
using Grouuu.Parsers;

namespace Grouuu.Managers
{
	public class DatabaseManager : IManager
	{
		private readonly DatabaseManagerSettings _settings;
		private DatabaseParser[] _parsers;

		public DatabaseManager (DatabaseManagerSettings settings)
		{
			_settings = settings;
			Init();
		}

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

		private void Init ()
		{
			_parsers = _settings.Parsers;
		}

		private async Awaitable LoadJsonData (DatabaseParser parser)
		{
			if (parser != null)
			{
				ResourceRequest request = Resources.LoadAsync<TextAsset>($"{_settings.DatabaseFolderPath}{parser.databaseFileName}");

				await request;

				TextAsset file = (TextAsset) request.asset;
				string fileContent = file.text;
				parser.ParseData(fileContent);
			}
		}
	}
}
