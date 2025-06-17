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
	public string[] EDIT_ANIMS { get; set; }
	public string YES_TEXT { get; set; }
    public string[] YES_RESULT_VARS { get; set; }
    public string[] YES_RESULT_RESOURCES { get; set; }
    public string[] YES_RESULT_EVENT_DAY { get; set; }
    public string[] YES_RESULT_VAR_MAX { get; set; }
    public string[] YES_RESULT_BUILDING_PROGRESS { get; set; }
    public string[] YES_EDIT_ANIMS { get; set; }
    public string NO_TEXT { get; set; }
    public string[] NO_RESULT_VARS { get; set; }
    public string[] NO_RESULT_RESOURCES { get; set; }
    public string[] NO_RESULT_EVENT_DAY { get; set; }
	public string[] NO_RESULT_VAR_MAX { get; set; }
	public string[] NO_RESULT_BUILDING_PROGRESS { get; set; }
	public string[] NO_EDIT_ANIMS { get; set; }
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

			DialogData dialogData = ParseEntry(jsonData);
			list.Add(dialogData);
		}

		_data = list.ToArray();
	}

	private DialogData ParseEntry (DialogDatabaseData jsonData)
	{
		DialogData dialog = new();

		dialog.id = jsonData.ID;
		dialog.name = jsonData.NAME;
		dialog.characterName = jsonData.CHAR_NAME;
		dialog.priority = jsonData.PRIORITY;
		dialog.isRepeateable = jsonData.REPEATABLE;
		dialog.requirements = ParsingUtils.ParseRequirementData(jsonData.REQUIREMENT);
		dialog.yesResult = DialogResultData.CreateFrom(ParsingUtils.ParseResultData(
			jsonData.YES_RESULT_VARS,
			jsonData.YES_RESULT_RESOURCES,
			jsonData.YES_RESULT_EVENT_DAY,
			jsonData.YES_RESULT_VAR_MAX,
			jsonData.YES_RESULT_BUILDING_PROGRESS
		));
		dialog.yesResult.dialogName = dialog.name;
		dialog.yesResult.isYes = true;
		dialog.noResult = DialogResultData.CreateFrom(ParsingUtils.ParseResultData(
			jsonData.NO_RESULT_VARS,
			jsonData.NO_RESULT_RESOURCES,
			jsonData.NO_RESULT_EVENT_DAY,
			jsonData.NO_RESULT_VAR_MAX,
			jsonData.NO_RESULT_BUILDING_PROGRESS
		));
		dialog.noResult.dialogName = dialog.name;
		dialog.noResult.isYes = false;
		(dialog.enterAnimations, dialog.exitAnimations) = ParsingUtils.ParseEditAnimations(jsonData.EDIT_ANIMS);
		(dialog.yesEnterAnimations, dialog.yesExitAnimations) = ParsingUtils.ParseEditAnimations(jsonData.YES_EDIT_ANIMS);
		(dialog.noEnterAnimations, dialog.noExitAnimations) = ParsingUtils.ParseEditAnimations(jsonData.NO_EDIT_ANIMS);

		return dialog;
	}

}
