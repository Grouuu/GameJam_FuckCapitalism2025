using System.Linq;
using UnityEngine;

public class DatabaseController : MonoBehaviour
{
	public VarData[] varsData = { };
	public CharacterData[] charactersData = { };
	public DialogData[] dialogsData = { };
	public EventData[] eventsData = { };
	public EndingData[] endingsData = { };

	protected DatabaseHandler handler = new();

	public async Awaitable LoadDatabase (DatabaseParser[] parsers, string folderpath)
	{
		await handler.LoadDatabase(parsers, folderpath);

		varsData = handler.GetData<VarData>();
		charactersData = handler.GetData<CharacterData>();
		dialogsData = handler.GetData<DialogData>();
		eventsData = handler.GetData<EventData>();
		endingsData = handler.GetData<EndingData>();

		// inject dialogs into their related character
		foreach (DialogData dialogData in dialogsData)
		{
			CharacterData character = GetCharacterData(dialogData.characterName);

			if (character == null)
			{
				Debug.LogWarning($"No character found from dialog (name: {dialogData.characterName})");
				continue;
			}

			character.characterDialogs.Add(dialogData);
		}
	}

	public VarData GetVarData (GameVarId varId) => varsData.FirstOrDefault(entry => entry.varId == varId);
	public CharacterData GetCharacterData (string characterName) => charactersData.FirstOrDefault(entry => entry.name == characterName);
	public DialogData GetDialogData (string dialogName) => dialogsData.FirstOrDefault(entry => entry.name == dialogName);
	public EventData GetEventData (string eventName) => eventsData.FirstOrDefault(entry => entry.name == eventName);
	public EndingData GetEndingData (string endingName) => endingsData.FirstOrDefault(entry => entry.name == endingName);

	public void SetValueToVar (GameVarId varId, int value)
	{
		if (varId == GameVarId.None)
			return;

		VarData varData = GetVarData(varId);

		if (varData != null)
			varData.currentValue = value;
	}

	public void UpdateCheckVars ()
	{
		// FoodAfterConsumption
		VarData foodData = GetVarData(GameVarId.Food);
		VarData populationData = GetVarData(GameVarId.Population);

		if (foodData != null && populationData != null)
			SetValueToVar(GameVarId.FoodAfterConsumption, foodData.currentValue - populationData.currentValue);

		// NextBattlePreview
		// not supported yet
	}

	public bool IsVarLow (GameVarId varId)
	{
		VarData varData = GetVarData(varId);

		if (varData == null || varData.lowThreshold == null)
			return false;

		return varData.lowThreshold.IsValueOK(varData.currentValue);
	}

	public bool IsVarCompareOk (VarCompareValue compare)
	{
		if (compare == null)
			return true;

		VarData varData = GetVarData(compare.varId);

		if (varData == null)
			return true;

		return compare.IsValueOK(varData.currentValue);
	}

	public bool IsResultSafe (ResultData result)
	{
		ResultVarChange[] varChanges = result.varChanges;

		if (varChanges == null)
			return true;

		foreach (ResultVarChange varChange in varChanges)
		{
			VarData varData = GetVarData(varChange.varId);

			if (varData == null)
			{
				Debug.LogWarning($"Var data null for varId: {varChange.varId}");
				continue;
			}

			if (varData.type != GameVarType.UIVar)
				continue;

			int currentValue = varData.currentValue;
			int minValue = varData.minValue;
			int maxModifier = Mathf.Min(varChange.modifierValueMin, varChange.modifierValueMax);

			if (currentValue + maxModifier < minValue)
				return false;
		}

		return true;
	}

	public void ApplyResultOnVar (ResultVarChange varChange)
	{
		if (varChange == null)
		{
			Debug.LogWarning($"varChange null");
			return;
		}

		VarData varData = GetVarData(varChange.varId);

		if (varData == null)
		{
			Debug.LogWarning($"Var data null for varId: {varChange.varId}");
			return;
		}

		if (varChange.modifierType == ChangeValueType.Add)
			SetValueToVar(varChange.varId, varData.currentValue + varChange.currentValue);
		else if (varChange.modifierType == ChangeValueType.Set)
			SetValueToVar(varChange.varId, varChange.currentValue);
	}

	public void ApplyEditDayOnEvent (EditEventDay editDay)
	{
		if (editDay == null)
		{
			Debug.LogWarning($"editDay null");
			return;
		}

		EventData eventData = GetEventData(editDay.eventName);

		if (eventData == null)
		{
			Debug.LogWarning($"Event data null for name: {editDay.eventName}");
			return;
		}

		VarData dayData = GetVarData(GameVarId.Day);

		if (dayData == null)
		{
			Debug.Log($"Day data null");
			return;
		}

		eventData.day = editDay.GetNewDay(dayData.currentValue);
	}

}
