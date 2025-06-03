using System.Collections.Generic;
using UnityEngine;

public class PlayEventState : StateCommand
{
	private List<string> _todayPlayedEventsName;        // prevent to play twice the same event
	private bool _randomEventPlayed;					// one attempt of picking a random event

	public override void StartCommand (GameState previousState)
	{
		_todayPlayedEventsName = new();
		_randomEventPlayed = false;

		ApplySave();

		NextEvent();
	}

	private void OnEnable () => state = GameState.PlayEvent;

	private void ApplySave ()
	{
		List<string> eventsPlayedToday = GameManager.Instance.saveManager.GetSaveData<List<string>>(SaveItemKey.EventsPlayedToday);

		if (eventsPlayedToday != null && eventsPlayedToday.Count > 0)
			_todayPlayedEventsName = eventsPlayedToday;

		if (GameManager.Instance.saveManager.HasKey(SaveItemKey.RandomEventPlayed))
			_randomEventPlayed = GameManager.Instance.saveManager.GetSaveData<bool>(SaveItemKey.RandomEventPlayed);
	}

	private async void NextEvent ()
	{
		EventData selectedEvent = GameManager.Instance.eventsManager.PickEvent(_todayPlayedEventsName.ToArray(), _randomEventPlayed);

		if (selectedEvent != null)
		{
			_todayPlayedEventsName.Add(selectedEvent.name);
			selectedEvent.isUsed = true;

			if (selectedEvent.type == EventType.Random)
				_randomEventPlayed = true;

			PlayEvent(selectedEvent);
		}
		else
		{
			Debug.Log("No event found");

			// clear save
			GameManager.Instance.eventsManager.UpdateEventsPlayedTodaySaveData(new());
			GameManager.Instance.eventsManager.UpdateRandomEventPlayedTodaySaveData(false);
			await GameManager.Instance.saveManager.SaveData();

			EndCommand();
		}
	}

	private async void PlayEvent (EventData eventData)
	{
		eventData.GenerateResultValue();
		ApplyResult(eventData.result);

		if (eventData.type == EventType.Random)
			GameManager.Instance.eventsManager.UpdateRandomEventPlayedTodaySaveData(true);

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
		panelData.content = eventData.description;
		panelData.varChanges = eventData.result.varChanges;

		return panelData;
	}

}
