using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class EndingDatabaseData
{
	public string ID { get; set; }
	public string NAME { get; set; }
	public string TITLE { get; set; }
	public string DESCRIPTION { get; set; }
	public bool IS_VICTORY { get; set; }
	public string HEADER_FILE_NAME { get; set; }
	public string[] REQUIREMENT { get; set; }
	public string[] RESULT_VARS { get; set; }
	public string[] RESULT_RESOURCES { get; set; }
	public string[] RESULT_EVENT_DAY { get; set; }
	public string[] RESULT_VAR_MAX { get; set; }
}

public class EndingParser : DatabaseParser
{
	private EndingData[] _data;

	public override Type GetDataType () => typeof(EndingData);
	public override EndingData[] GetData<EndingData> () => (EndingData[]) (object) _data;

	public override void ParseData (string json)
	{
		List<EndingData> list = new();

		EndingDatabaseData[] jsonDatas = JsonConvert.DeserializeObject<EndingDatabaseData[]>(json);

		foreach (EndingDatabaseData jsonData in jsonDatas)
		{
			if (jsonData.NAME == "")
				continue;

			EndingData endingData = ParseEntry(jsonData);
			list.Add(endingData);
		}

		_data = list.ToArray();
	}

	private EndingData ParseEntry (EndingDatabaseData jsonData)
	{
		EndingData endingData = new();

		endingData.id = jsonData.ID;
		endingData.name = jsonData.NAME;
		endingData.title = jsonData.TITLE;
		endingData.description = jsonData.DESCRIPTION;
		endingData.isWinEnding = jsonData.IS_VICTORY;
		endingData.headerFileName = jsonData.HEADER_FILE_NAME;
		endingData.requirements = ParsingUtils.ParseRequirementData(jsonData.REQUIREMENT);
		endingData.result = ParsingUtils.ParseResultData(
			jsonData.RESULT_VARS,
			jsonData.RESULT_RESOURCES,
			jsonData.RESULT_EVENT_DAY,
			jsonData.RESULT_VAR_MAX
		);

		return endingData;
	}

}