using Grouuu.Enum;
using Grouuu.States;
using UnityEngine;

namespace Grouuu.Managers
{
	[CreateAssetMenu(fileName = "GameStateManagerSettings", menuName = "GameStateManagerSettings")]
	public class GameStateManagerSettings : ScriptableObject
	{
		public GameState InitialState;
		public StateCommand[] StateCommands;
	}
}