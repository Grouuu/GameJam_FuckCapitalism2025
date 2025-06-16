using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class BuildingsDatabaseData
{
    public string ID { get; set; }
    public string NAME { get; set; }
    public string DISPLAY_NAME { get; set; }
    public string DESCRIPTION { get; set; }
    public int CONSTRUCTION_TIME { get; set; }
    public int BUILD_LIMIT { get; set; }
    public string[] COST { get; set; }
    public string[] RESULT_VARS { get; set; }
    public string[] RESULT_RESOURCES { get; set; }
	public string[] RESULT_EVENT_DAY { get; set; }
	public string[] RESULT_VAR_MAX { get; set; }
	public string[] RESULT_BUILDING_PROGRESS { get; set; }
	public string[] DAILY_PRODUCTION_FLAT { get; set; }
    public string[] PROD_MULTIPLIER { get; set; }
}

public class BuildingsParser : DatabaseParser
{
    private BuildingData[] _data;

    public override Type GetDataType () => typeof(BuildingData);
    public override BuildingData[] GetData<BuildingData> () => (BuildingData[]) (object) _data;

	public override void ParseData (string json)
	{
		List<BuildingData> list = new();

		BuildingsDatabaseData[] jsonDatas = JsonConvert.DeserializeObject<BuildingsDatabaseData[]>(json);

		foreach (BuildingsDatabaseData jsonData in jsonDatas)
		{
			if (jsonData.NAME == "")
				continue;

			BuildingData buildingData = ParseEntry(jsonData);
			list.Add(buildingData);
		}

		_data = list.ToArray();
	}

	private BuildingData ParseEntry (BuildingsDatabaseData jsonData)
	{
		BuildingData buildingData = new();

		buildingData.id = jsonData.ID;
		buildingData.name = jsonData.NAME;
		buildingData.constructionTime = jsonData.CONSTRUCTION_TIME;
		buildingData.buildLimit = jsonData.BUILD_LIMIT;
		buildingData.costs = ParsingUtils.ParseResultVarChanges(jsonData.COST);
		buildingData.result = DialogResultData.CreateFrom(ParsingUtils.ParseResultData(
			jsonData.RESULT_VARS,
			jsonData.RESULT_RESOURCES,
			jsonData.RESULT_EVENT_DAY,
			jsonData.RESULT_VAR_MAX,
			jsonData.RESULT_BUILDING_PROGRESS
		));
		buildingData.production = ParsingUtils.ParseResultVarChanges(jsonData.DAILY_PRODUCTION_FLAT);
		buildingData.productionMultipliers = ParsingUtils.ParseProductionMultipliers(jsonData.PROD_MULTIPLIER);

		return buildingData;
	}
}
