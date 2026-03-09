using UnityEngine;

namespace Grouuu.Managers
{
	[CreateAssetMenu(fileName = "CharactersManagerSettings", menuName = "CharactersManagerSettings")]
	public class CharactersManagerSettings : ScriptableObject
	{
		public int NumberOfDialogPriorities = 6;
		public bool Debug = false;
	}
}