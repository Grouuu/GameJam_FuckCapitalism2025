using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class EventDatabaseData
{
    public string ID { get; set; }
    public string NAME { get; set; }
    public string TYPE { get; set; }
    public int RANDOM_WEIGHT { get; set; }
    public string TITLE { get; set; }
    public string DESCRIPTION { get; set; }
    public string HEADER_FILE_NAME { get; set; }
    public int PRIORITY { get; set; }
    public bool REPEATABLE { get; set; }
	public string MIN_DAY { get; set; }
	public string MAX_DAY { get; set; }
    public string[] REQUIREMENT { get; set; }
    public string[] RESULT_VARS { get; set; }
    public string[] RESULT_RESOURCES { get; set; }
    public string[] RESULT_EVENT_DAY { get; set; }
    public string[] RESULT_VAR_MAX { get; set; }
	public string[] RESULT_BUILDING_PROGRESS { get; set; }
}

public class EventParser : DatabaseParser
{
	private EventData[] _data;

	public override Type GetDataType () => typeof(EventData);
	public override EventData[] GetData<EventData> () => (EventData[]) (object) _data;

	public override void ParseData (string json)
	{
		List<EventData> list = new();

		EventDatabaseData[] jsonDatas = JsonConvert.DeserializeObject<EventDatabaseData[]>(json);

		foreach (EventDatabaseData jsonData in jsonDatas)
		{
			if (jsonData.NAME == "")
				continue;

			EventData eventData = ParseEntry(jsonData);
			list.Add(eventData);
		}

		_data = list.ToArray();
	}

	private EventData ParseEntry (EventDatabaseData jsonData)
	{
		EventData eventData = new();

		eventData.id = jsonData.ID;
		eventData.name = jsonData.NAME;
		eventData.type = ParsingUtils.MapServerEventType(jsonData.TYPE.Trim());
		eventData.title = jsonData.TITLE;
		eventData.randomWeight = jsonData.RANDOM_WEIGHT;
		eventData.description = jsonData.DESCRIPTION;
		eventData.priority = jsonData.PRIORITY;
		eventData.isRepeateable = jsonData.REPEATABLE;
		eventData.day = ParsingUtils.ParseAndSolveMinMaxInt(jsonData.MIN_DAY, jsonData.MAX_DAY);
		eventData.headerFileName = jsonData.HEADER_FILE_NAME;
		eventData.requirements = ParsingUtils.ParseRequirementData(jsonData.REQUIREMENT);
		eventData.result = ParsingUtils.ParseResultData(
			jsonData.RESULT_VARS,
			jsonData.RESULT_RESOURCES,
			jsonData.RESULT_EVENT_DAY,
			jsonData.RESULT_VAR_MAX,
			jsonData.RESULT_BUILDING_PROGRESS
		);

		return eventData;
	}

}
