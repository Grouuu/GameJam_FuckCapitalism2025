using Grouuu.PersistentManagers;

namespace Grouuu.Managers
{
	public class GameManagers
	{
		// persitent
		public SaveManager SaveManager;
		public SoundManager SoundManager;
		public I2Manager LocalizationManager;

		// scene
		public DebugManager DebugManager;
		public UIManager UiManager;
		public SceneEffectsManager SceneEffectsManager;

		// logic
		public DatabaseManager DatabaseManager;
		public VarsManager VarsManager;
		public GameStateManager GameStateManager;
		public CharactersManager CharactersManager;
		public EventsManager EventsManager;
		public EndingsManager EndingsManager;
		public ProductionManager ProductionManager;
		public BuildingsManager BuildingsManager;
	}
}