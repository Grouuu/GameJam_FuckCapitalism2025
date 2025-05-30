using System.Collections.Generic;
using UnityEngine;

public class PlayEventState : StateCommand
{
	private List<string> _todayPlayedEventsName;        // prevent to play twice the same event

	public override void StartCommand (GameState previousState)
	{
		_todayPlayedEventsName = new();

		ApplySave();

		NextEvent();
	}

	private void OnEnable () => state = GameState.PlayEvent;

	private void ApplySave ()
	{
		List<string> eventsPlayedToday = GameManager.Instance.saveManager.GetSaveData<List<string>>(SaveItemKey.CharactersPlayedToday);

		if (eventsPlayedToday != null && eventsPlayedToday.Count > 0)
			_todayPlayedEventsName = eventsPlayedToday;
	}

	private async void NextEvent ()
	{
		EventData selectedEvent = GameManager.Instance.eventsManager.PickEvent(_todayPlayedEventsName.ToArray());

		if (selectedEvent != null)
		{
			_todayPlayedEventsName.Add(selectedEvent.name);
			selectedEvent.isUsed = true;

			PlayEvent(selectedEvent);
		}
		else
		{
			Debug.Log("No event found");

			// clear save
			GameManager.Instance.eventsManager.UpdateEventsPlayedTodaySaveData(new());
			await GameManager.Instance.saveManager.SaveData();

			EndCommand();
		}
	}

	private async void PlayEvent (EventData eventData)
	{
		eventData.GenerateResultValue();
		ApplyResult(eventData.result);

		GameManager.Instance.eventsManager.UpdateEventsPlayedTodaySaveData(_todayPlayedEventsName);
		GameManager.Instance.eventsManager.UpdateEventsUsedSaveData();
		GameManager.Instance.eventsManager.UpdateEventsDaySaveData();
		await GameManager.Instance.saveManager.SaveData();

		EventPanelUIData panelData = FormatEventPanelTexts(eventData);
		GameManager.Instance.uiManager.ShowEventPanel(panelData, () => EndEvent());
	}

	private void EndEvent ()
	{
		if (GameManager.Instance.endingsManager.CheckLose())
		{
			// no need to save
			EndCommand(GameState.EndGame);
			return;
		}

		CheckWin();
	}

	private void CheckWin ()
	{
		if (GameManager.Instance.endingsManager.CheckWin())
			GameManager.Instance.endingsManager.ShowWin(() => AfterWin());
		else
			NextEvent();
	}

	private void AfterWin ()
	{
		NextEvent();
	}

	private void ApplyResult (ResultData resultData)
	{
		resultData.ApplyResult();
	}

	private EventPanelUIData FormatEventPanelTexts (EventData eventData)
	{
		EventPanelUIData panelData = new();

		panelData.title = eventData.title;

		string content = "";

		content += $"{eventData.description}";

		ResultVarChange[] changes = eventData.result.varChanges;

		if (changes != null && changes.Length > 0)
		{
			content += "\n\n";

			foreach (ResultVarChange varChange in changes)
			{
				if (varChange.currentValue == 0)
					continue;

				VarData varData = GameManager.Instance.varsManager.GetVarData(varChange.varId);

				if (varData.type != GameVarType.UIVar)
					continue;

				string name = varData.displayName;
				string modifier = varChange.currentValue > 0 ? "+" : "";
				content += $"{name} : {modifier}{varChange.currentValue}\n";
			}
		}

		panelData.content = content;

		return panelData;
	}

}
