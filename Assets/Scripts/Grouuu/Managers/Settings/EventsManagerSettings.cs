using UnityEngine;

namespace Grouuu.Managers
{
	[CreateAssetMenu(fileName = "EventsManagerSettings", menuName = "EventsManagerSettings")]
	public class EventsManagerSettings : ScriptableObject
	{
		public bool Debug = false;
	}
}