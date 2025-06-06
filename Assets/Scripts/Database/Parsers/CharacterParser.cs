using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

public class CharacterDatabaseData
{
    public string ID { get; set; }
    public string NAME { get; set; }
    public string DISPLAY_NAME { get; set; }
    public string FILE_NAME { get; set; }
    public string[] MAIN_PROD { get; set; }
	public string[] REQUIREMENT { get; set; }
}

public class CharacterParser : DatabaseParser
{
	private CharacterData[] _data;

	public override Type GetDataType () => typeof(CharacterData);
	public override CharacterData[] GetData<CharacterData> () => (CharacterData[]) (object) _data;

	public override void ParseData (string json)
	{
		List<CharacterData> list = new();

		CharacterDatabaseData[] jsonDatas = JsonConvert.DeserializeObject<CharacterDatabaseData[]>(json);

		foreach (CharacterDatabaseData jsonData in jsonDatas)
		{
			if (jsonData.NAME == "")
				continue;

			CharacterData character = ParseEntry(jsonData);
			list.Add(character);
		}

		_data = list.ToArray();
	}

	private CharacterData ParseEntry (CharacterDatabaseData jsonData)
	{
		CharacterData characterData = new();

		characterData.id = jsonData.ID;
		characterData.name = jsonData.NAME;
		characterData.displayName = jsonData.DISPLAY_NAME;
		characterData.avatarFileName = jsonData.FILE_NAME;
		characterData.relatedGameVars = jsonData.MAIN_PROD
			.Where(entry => !string.IsNullOrEmpty(entry))
			.Select(entry => ParsingUtils.MapServerVarId(entry.Trim()))
			.Where(entry => entry != GameVarId.None)
			.ToArray()
		;

		// requirements

		RequirementData requirement = ParsingUtils.ParseRequirementData(jsonData.REQUIREMENT);

		if (requirement != null)
			characterData.requirements = new RequirementData[1] { requirement };

		return characterData;
	}
}
