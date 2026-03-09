using System;
using UnityEngine;

namespace Grouuu.Parsers
{
	public class DatabaseParser : MonoBehaviour
	{
		public string databaseFileName;
		public string GetJsonFileName () => $"{databaseFileName}.json";

		public virtual Type GetDataType () => typeof(object);
		public virtual T[] GetData<T> () => default;
		public virtual void ParseData (string data) { }
	}
}