using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class VarsDatabaseData
{
    public string ID { get; set; }
    public string NAME { get; set; }
    public string TYPE { get; set; }
    public string DISPLAY_NAME { get; set; }
    public string ICON_FILE_NAME { get; set; }
    public int MIN { get; set; }
    public int MAX { get; set; }
    public int START { get; set; }
    public string[] IS_LOW { get; set; }
}

public class VarsParser : DatabaseParser
{
	private VarData[] _data;

	public override Type GetDataType () => typeof(VarData);
	public override VarData[] GetData<VarData> () => (VarData[]) (object) _data;

	public override void ParseData (string json)
	{
		List<VarData> list = new();

		VarsDatabaseData[] jsonDatas = JsonConvert.DeserializeObject<VarsDatabaseData[]>(json);

		foreach (VarsDatabaseData jsonData in jsonDatas)
		{
			if (jsonData.NAME == "")
				continue;

			VarData varData = ParseEntry(jsonData);
			list.Add(varData);
		}

		_data = list.ToArray();
	}

	private VarData ParseEntry (VarsDatabaseData jsonData)
	{
		VarData varData = new();

		varData.id = jsonData.ID;
		varData.name = jsonData.NAME;
		varData.varId = ParsingUtils.MapServerVarId(jsonData.NAME);
		varData.type = ParsingUtils.MapServerVarType(jsonData.TYPE);
		varData.iconFileName = jsonData.ICON_FILE_NAME;
		varData.startValue = jsonData.START;
		varData.currentValue = jsonData.START;
		varData.minValue = jsonData.MIN;
		varData.maxValue = jsonData.MAX;

		VarCompareValue[] comparers = ParsingUtils.ParseVarCompareValues(jsonData.IS_LOW);

		if (comparers.Length > 0)
			varData.lowThreshold = comparers[0];

		return varData;
	}
}
