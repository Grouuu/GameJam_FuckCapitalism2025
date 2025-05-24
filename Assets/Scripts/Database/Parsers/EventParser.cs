using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class EventDatabaseData
{
    public string ID { get; set; }
    public string NAME { get; set; }
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

		return eventData;
	}

}
