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
    public string PROD_PER_CYCLE { get; set; }
    public string IS_LOW { get; set; }
    public string END_WHEN_0 { get; set; }
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

			VarData character = ParseEntry(jsonData);
			list.Add(character);
		}

		_data = list.ToArray();
	}

	private VarData ParseEntry (VarsDatabaseData jsonData)
	{
		VarData varData = new();

		varData.id = jsonData.ID;
		varData.varId = ParsingUtils.MapServerVarId(jsonData.NAME);
		varData.type = ParsingUtils.MapServerVarType(jsonData.TYPE);
		varData.displayName = jsonData.DISPLAY_NAME;
		varData.iconFileName = jsonData.ICON_FILE_NAME;
		varData.startValue = jsonData.START;
		varData.currentValue = jsonData.START;
		varData.minValue = jsonData.MIN;
		varData.maxValue = jsonData.MAX;

		// production per cycle

		string[] stringArray = jsonData.PROD_PER_CYCLE.Split(new char[] { ',' });

		if (stringArray.Length > 0 && stringArray[0] != "")
		{
			varData.prodPerCycleMin = int.Parse(stringArray[0].Trim());

			if (stringArray.Length > 1)
				varData.prodPerCycleMax = int.Parse(stringArray[1].Trim());
			else
				varData.prodPerCycleMax = varData.prodPerCycleMin;
		} else
		{
			varData.prodPerCycleMin = 0;
			varData.prodPerCycleMax = 0;
		}

		// low threshold

		stringArray = jsonData.IS_LOW.Split(new char[] { ',' });

		if (stringArray.Length == 3 && stringArray[0] != "")
		{
			VarCompareValue compare = new();

			compare.varId = ParsingUtils.MapServerVarId(stringArray[0].Trim());
			compare.checkType = ParsingUtils.MapServerCompareType(stringArray[1].Trim());
			compare.compareValue = int.Parse(stringArray[2].Trim());

			varData.lowThreshold = compare;
		}

		return varData;
	}
}
