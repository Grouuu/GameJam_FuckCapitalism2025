using Grouuu.Parsers;
using UnityEngine;

namespace Grouuu.Managers
{
	[CreateAssetMenu(fileName = "DatabaseManagerSettings", menuName = "DatabaseManagerSettings")]
	public class DatabaseManagerSettings : ScriptableObject
	{
		public string DatabaseFolderPath = "Database/";
		public DatabaseParser[] Parsers;
	}
}