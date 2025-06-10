using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class ProductionDatabaseData
{
    public string ID { get; set; }
    public string VAR_NAME { get; set; }
    public int RANDOM_WEIGHT { get; set; }
}

public class ProductionParser : DatabaseParser
{
	private ProductionData[] _data;

	public override Type GetDataType () => typeof(ProductionData);
	public override ProductionData[] GetData<ProductionData> () => (ProductionData[]) (object) _data;

	public override void ParseData (string json)
	{
		List<ProductionData> list = new();

		ProductionDatabaseData[] jsonDatas = JsonConvert.DeserializeObject<ProductionDatabaseData[]>(json);

		foreach (ProductionDatabaseData jsonData in jsonDatas)
		{
			if (jsonData.VAR_NAME == "")
				continue;

			ProductionData productionData = ParseEntry(jsonData);
			list.Add(productionData);
		}

		_data = list.ToArray();
	}

	private ProductionData ParseEntry (ProductionDatabaseData jsonData)
	{
		ProductionData productionData = new();

		productionData.id = jsonData.ID;
		productionData.varId = ParsingUtils.MapServerVarId(jsonData.VAR_NAME.Trim());
		productionData.randomWeight = jsonData.RANDOM_WEIGHT;

		return productionData;
	}
}
