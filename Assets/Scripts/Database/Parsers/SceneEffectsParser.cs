using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class SceneEffectDatabaseData
{
	public string ID { get; set; }
	public string NAME { get; set; }
	public int TIMING { get; set; }
	public int DURATION { get; set; }
	public string[] PLAY_REQUIREMENT { get; set; }
	public string[] STOP_REQUIREMENT { get; set; }
	public string[] PLAY_RESULT_VARS { get; set; }
	public string[] PLAY_RESULT_RESOURCES { get; set; }
	public string[] PLAY_RESULT_EVENT_DAY { get; set; }
	public string[] PLAY_RESULT_VAR_MAX { get; set; }
	public string[] PLAY_RESULT_BUILDING_PROGRESS { get; set; }
	public string[] STOP_RESULT_VARS { get; set; }
	public string[] STOP_RESULT_RESOURCES { get; set; }
	public string[] STOP_RESULT_EVENT_DAY { get; set; }
	public string[] STOP_RESULT_VAR_MAX { get; set; }
	public string[] STOP_RESULT_BUILDING_PROGRESS { get; set; }
	public string[] EDIT_SCENE_EFFECTS { get; set; }
}

public class SceneEffectsParser : DatabaseParser
{
	private SceneEffectData[] _data;

	public override Type GetDataType () => typeof(SceneEffectData);
	public override SceneEffectData[] GetData<SceneEffectData> () => (SceneEffectData[]) (object) _data;

	public override void ParseData (string json)
	{
		List<SceneEffectData> list = new();

		SceneEffectDatabaseData[] jsonDatas = JsonConvert.DeserializeObject<SceneEffectDatabaseData[]>(json);

		foreach (SceneEffectDatabaseData jsonData in jsonDatas)
		{
			if (jsonData.NAME == "")
				continue;

			SceneEffectData sceneEffectData = ParseEntry(jsonData);
			list.Add(sceneEffectData);
		}

		_data = list.ToArray();
	}

	private SceneEffectData ParseEntry (SceneEffectDatabaseData jsonData)
	{
		SceneEffectData sceneEffectData = new();

		sceneEffectData.id = jsonData.ID;
		sceneEffectData.name = jsonData.NAME;
		sceneEffectData.timing = jsonData.TIMING;
		sceneEffectData.duration = jsonData.DURATION;
		sceneEffectData.playRequirements = ParsingUtils.ParseRequirementData(jsonData.PLAY_REQUIREMENT);
		sceneEffectData.stopRequirements = ParsingUtils.ParseRequirementData(jsonData.STOP_REQUIREMENT);
		sceneEffectData.playResult = ParsingUtils.ParseResultData(
			jsonData.PLAY_RESULT_VARS,
			jsonData.PLAY_RESULT_RESOURCES,
			jsonData.PLAY_RESULT_EVENT_DAY,
			jsonData.PLAY_RESULT_VAR_MAX,
			jsonData.PLAY_RESULT_BUILDING_PROGRESS
		);
		sceneEffectData.stopResult = ParsingUtils.ParseResultData(
			jsonData.STOP_RESULT_VARS,
			jsonData.STOP_RESULT_RESOURCES,
			jsonData.STOP_RESULT_EVENT_DAY,
			jsonData.STOP_RESULT_VAR_MAX,
			jsonData.STOP_RESULT_BUILDING_PROGRESS
		);
		(sceneEffectData.enterSceneEffets, sceneEffectData.exitSceneEffets) = ParsingUtils.ParseEditSceneEffects(jsonData.EDIT_SCENE_EFFECTS);

		return sceneEffectData;
	}
}
