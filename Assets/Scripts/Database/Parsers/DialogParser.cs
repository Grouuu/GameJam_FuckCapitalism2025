using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class DialogDatabaseData
{
    public string ID { get; set; }
    public string NAME { get; set; }
    public string CHAR_NAME { get; set; }
    public int PRIORITY { get; set; }
    public string REQUEST { get; set; }
    public bool REPEATABLE { get; set; }
    public string[] REQUIREMENT { get; set; }
    public string YES_TEXT { get; set; }
    public string[] YES_RESULT_VARS { get; set; }
    public string[] YES_RESULT_RESOURCES { get; set; }
    public string[] YES_RESULT_EVENT_DAY { get; set; }
    public string[] YES_RESULT_VAR_MAX { get; set; }
    public string NO_TEXT { get; set; }
    public string[] NO_RESULT_VARS { get; set; }
    public string[] NO_RESULT_RESOURCES { get; set; }
    public string[] NO_RESULT_EVENT_DAY { get; set; }
	public string[] NO_RESULT_VAR_MAX { get; set; }
}

public class DialogParser : DatabaseParser
{
	private DialogData[] _data;

	public override Type GetDataType () => typeof(DialogData);
	public override DialogData[] GetData<DialogData> () => (DialogData[]) (object) _data;

	public override void ParseData (string json)
	{
		List<DialogData> list = new();

		DialogDatabaseData[] jsonDatas = JsonConvert.DeserializeObject<DialogDatabaseData[]>(json);

		foreach (DialogDatabaseData jsonData in jsonDatas)
		{
			if (jsonData.NAME == "")
				continue;

			DialogData character = ParseEntry(jsonData);
			list.Add(character);
		}

		_data = list.ToArray();
	}

	private DialogData ParseEntry (DialogDatabaseData jsonData)
	{
		DialogData dialog = new();

		dialog.id = jsonData.ID;
		dialog.name = jsonData.NAME;
		dialog.characterName = jsonData.CHAR_NAME;
		dialog.request = jsonData.REQUEST;
		dialog.priority = jsonData.PRIORITY;
		dialog.isRepeateable = jsonData.REPEATABLE;

		// requirements

		RequirementData requirement = ParsingUtils.ParseRequirementData(jsonData.REQUIREMENT);

		if (requirement != null)
			dialog.requirements = new RequirementData[1] { requirement };

		// results

		dialog.yesResult = DialogResultData.CreateFrom(ParsingUtils.ParseResultData(
			jsonData.YES_RESULT_VARS,
			jsonData.YES_RESULT_RESOURCES,
			jsonData.YES_RESULT_EVENT_DAY,
			jsonData.YES_RESULT_VAR_MAX
		));
		dialog.yesResult.response = jsonData.YES_TEXT;

		dialog.noResult = DialogResultData.CreateFrom(ParsingUtils.ParseResultData(
			jsonData.NO_RESULT_VARS,
			jsonData.NO_RESULT_RESOURCES,
			jsonData.NO_RESULT_EVENT_DAY,
			jsonData.NO_RESULT_VAR_MAX
		));
		dialog.noResult.response = jsonData.NO_TEXT;

		return dialog;
	}

}
