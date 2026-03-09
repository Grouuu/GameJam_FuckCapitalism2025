using Grouuu.Types;
using UnityEngine;

namespace Grouuu.Managers
{
	[CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings")]
	public class GameSettings : ScriptableObject, IManagerSettings
	{
		public DatabaseManagerSettings DatabaseManagerSettings;
		public GameStateManagerSettings GameStateManagerSettings;
		public VarsManagerSettings VarsManagerSettings;
		public CharactersManagerSettings CharactersManagerSettings;
		public EventsManagerSettings EventsManagerSettings;
		public EndingsManagerSettings EndingsManagerSettings;
		public ProductionManagerSettings ProductionManagerSettings;
		public BuildingsManagerSettings BuildingsManagerSettings;
	}
}