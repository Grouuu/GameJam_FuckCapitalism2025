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
    public string NO_TEXT { get; set; }
    public string[] NO_RESULT_VARS { get; set; }
    public string[] NO_RESULT_RESOURCES { get; set; }
    public string[] NO_RESULT_EVENT_DAY { get; set; }
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

		if (jsonData.REQUIREMENT.Length == 3 && jsonData.REQUIREMENT[0] != "")
		{
			GameVarId varId = ParsingUtils.MapServerVarId(jsonData.REQUIREMENT[0].Trim());

			if (varId != GameVarId.None)
			{
				CompareValueType compareType = ParsingUtils.MapServerCompareType(jsonData.REQUIREMENT[1].Trim());
				int compareValue = int.Parse(jsonData.REQUIREMENT[2].Trim());

				VarCompareValue compare = new();
				compare.varId = varId;
				compare.checkType = compareType;
				compare.compareValue = compareValue;

				RequirementData require = new();
				require.varsChecks = new VarCompareValue[1] { compare };
			}
		}

		// results

		dialog.yesResult = ParseResult(jsonData.YES_TEXT, jsonData.YES_RESULT_VARS, jsonData.YES_RESULT_RESOURCES, jsonData.YES_RESULT_EVENT_DAY);
		dialog.noResult = ParseResult(jsonData.NO_TEXT, jsonData.NO_RESULT_VARS, jsonData.NO_RESULT_RESOURCES, jsonData.NO_RESULT_EVENT_DAY);

		return dialog;
	}

	private DialogResultData ParseResult (string text, string[] changeVar, string[] changeResources, string[] editDays)
	{
		DialogResultData result = new();
		result.response = text;

		// vars change

		List<ResultVarChange> varChanges = new();

		if (changeVar.Length >= 3 && changeVar[0] != "")
		{
			GameVarId varId = ParsingUtils.MapServerVarId(changeVar[0].Trim());

			if (varId != GameVarId.None)
			{
				ResultVarChange change = new();
				change.varId = varId;
				change.modifierType = ParsingUtils.MapServerModifierType(changeVar[1].Trim());
				change.modifierValueMin = int.Parse(changeVar[2].Trim());

				if (changeVar.Length == 4)
					change.modifierValueMax = int.Parse(changeVar[3].Trim());
				else
					change.modifierValueMax = change.modifierValueMin;

				change.currentValue = change.modifierValueMin;

				varChanges.Add(change);
			}
		}

		if (changeResources.Length > 0 && changeResources[0] != "")
		{
			for (int i = 0; i < changeResources.Length; i += 2)
			{
				GameVarId varId = ParsingUtils.MapServerVarId(changeResources[i].Trim());

				if (varId != GameVarId.None)
				{
					ResultVarChange change = new();
					change.varId = varId;
					change.modifierType = ChangeValueType.Add;
					change.modifierValueMin = int.Parse(changeResources[i + 1].Trim());
					change.modifierValueMax = change.modifierValueMin;
					change.currentValue = change.modifierValueMin;

					varChanges.Add(change);
				}
			}
		}

		result.varChanges = varChanges.ToArray();

		// edit days

		List<EditEventDay> eventDayChanges = new();

		if (editDays.Length >= 3 && editDays[0] != "")
		{
			EditEventDay edit = new();
			edit.eventName = editDays[0].Trim();

			ChangeValueType modifier = ParsingUtils.MapServerModifierType(editDays[1].Trim());

			if (modifier == ChangeValueType.Set)
			{
				edit.setDay = int.Parse(editDays[2].Trim());
			}
			else
			{
				edit.addMinDays = int.Parse(editDays[2].Trim());

				if (editDays.Length == 4)
					edit.addMaxDays = int.Parse(editDays[3].Trim());
				else
					edit.addMaxDays = edit.addMinDays;
			}

			eventDayChanges.Add(edit);
		}

		result.eventsDay = eventDayChanges.ToArray();

		return result;
	}

}
