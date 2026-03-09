
namespace Grouuu.Constants
{
	public static class SaveItemKey
	{
		public static string Version = "Version";                               // string
		public static string MusicVolume = "MusicVolume";                       // float
		public static string MusicMute = "MusicMute";                           // bool

		public static string Date = "Date";                                     // string
		public static string RunStarted = "RunStarted";                         // bool
		public static string State = "State";                                   // GameState
		public static string VarsValue = "VarsValue";                           // List<(GameVarId, int, int)>
		public static string StartDayVarsValues = "StartDayVarsValues";         // List<KeyValuePair<GameVarId, int>>
		public static string CharactersPlayedToday = "CharactersPlayedToday";   // List<string>
		public static string DialogsUsed = "DialogsUsed";                       // List<string>
		public static string DialogsPlayedToday = "DialogsPlayedToday";         // int
		public static string DialogStarted = "DialogStarted";                   // string
		public static string EventsDay = "EventsDay";                           // List<KeyValuePair<string, int>>
		public static string EventsUsed = "EventsUsed";                         // List<string
		public static string EventsPlayedToday = "EventsPlayedToday";           // List<string>
		public static string EventStarted = "EventStarted";                     // string
		public static string RandomEventPlayed = "RandomEventPlayed";           // bool
		public static string EndingsUsed = "EndingsUsed";                       // List<string>
		public static string BuildingsState = "BuildingsState";                 // List<(string, bool, int)>
		public static string SceneEffects = "SceneEffects";                     // List<(SceneEffectName, bool)>

		public static string[] ProtectedKeys = new[]
		{
			Version,
			MusicVolume,
			MusicMute,
		};
	}
}