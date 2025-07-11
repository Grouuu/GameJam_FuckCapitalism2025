using System.Collections.Generic;
using UnityEngine;

public class PlayEventState : StateCommand
{
	public override GameState state => GameState.PlayEvent;

	private List<string> _todayPlayedEventsName;        // prevent to play twice the same event
	private bool _randomEventPlayed;                    // one attempt of picking a random event
	private string forceEvent;                          // force to show the same started event at restart

	public override void StartCommand (GameState previousState)
	{
		_todayPlayedEventsName = new();
		_randomEventPlayed = false;

		ApplySave();

		NextEvent();
	}

	private void ApplySave ()
	{
		List<string> eventsPlayedToday = GameManager.Instance.saveManager.GetSaveData<List<string>>(SaveItemKey.EventsPlayedToday);
		string startedEventName = GameManager.Instance.saveManager.GetSaveData<string>(SaveItemKey.EventStarted);

		if (eventsPlayedToday != null && eventsPlayedToday.Count > 0)
			_todayPlayedEventsName = eventsPlayedToday;

		if (GameManager.Instance.saveManager.HasKey(SaveItemKey.RandomEventPlayed))
			_randomEventPlayed = GameManager.Instance.saveManager.GetSaveData<bool>(SaveItemKey.RandomEventPlayed);

		if (!string.IsNullOrEmpty(startedEventName))
			forceEvent = startedEventName;
	}

	private async void NextEvent ()
	{
		EventData selectedEvent;

		if (string.IsNullOrEmpty(forceEvent))
			selectedEvent = GameManager.Instance.eventsManager.PickEvent(_todayPlayedEventsName.ToArray(), _randomEventPlayed);
		else
			selectedEvent = GameManager.Instance.eventsManager.GetEventByName(forceEvent);

		forceEvent = null;

		if (selectedEvent != null)
		{
			_todayPlayedEventsName.Add(selectedEvent.name);
			selectedEvent.isUsed = true;

			if (selectedEvent.type == EventDataType.Random)
				_randomEventPlayed = true;

			PlayEvent(selectedEvent);
		}
		else
		{
			Debug.Log("No event found");
			await ClearSaveData();
			EndCommand();
		}
	}

	private async void PlayEvent (EventData eventData)
	{
		await eventData.UpdateEnterSceneEffects();

		// TODO generated results are not conserved on restart
		eventData.GenerateResultValue();

		GameManager.Instance.eventsManager.UpdateEventStartedSaveData(eventData.name);
		await GameManager.Instance.saveManager.SaveData();

		EventPanelUIData panelData = FormatEventPanelTexts(eventData);
		GameManager.Instance.uiManager.ShowEventPanel(panelData, () => EndEvent(eventData));
	}

	private async void EndEvent (EventData eventData)
	{
		ApplyResult(eventData.result);

		if (eventData.type == EventDataType.Random)
			GameManager.Instance.eventsManager.UpdateRandomEventPlayedTodaySaveData(true);

		GameManager.Instance.eventsManager.UpdateEventsPlayedTodaySaveData(_todayPlayedEventsName);
		GameManager.Instance.eventsManager.UpdateEventStartedSaveData("");
		GameManager.Instance.eventsManager.UpdateEventsUsedSaveData();
		GameManager.Instance.eventsManager.UpdateEventsDaySaveData();
		await GameManager.Instance.saveManager.SaveData();

		await eventData.UpdateExitSceneEffects();

		if (GameManager.Instance.endingsManager.CheckLose() != null)
		{
			// no need to save
			EndCommand(GameState.EndGame);
			return;
		}

		CheckWin();
	}

	private async void CheckWin ()
	{
		EndingData endingData = GameManager.Instance.endingsManager.CheckWin();

		if (endingData != null)
		{
			await endingData.UpdateEnterSceneEffects();
			GameManager.Instance.endingsManager.ShowWin(() => NextEvent());
			await endingData.UpdateExitSceneEffects();
		}
		else
			NextEvent();
	}

	private void ApplyResult (ResultData resultData)
	{
		resultData.ApplyResult();
	}

	private EventPanelUIData FormatEventPanelTexts (EventData eventData)
	{
		EventPanelUIData panelData = new();

		panelData.titleTermKey = eventData.name;
		panelData.titleTermCat = LocCat.EventsTitles;
		panelData.contentTermKey = eventData.name;
		panelData.contentTermCat = LocCat.EventsDescriptions;
		panelData.headerFileName = eventData.headerFileName;
		panelData.varChanges = eventData.result.varChanges;

		return panelData;
	}

	private async Awaitable ClearSaveData ()
	{
		// clear save
		GameManager.Instance.eventsManager.UpdateEventsPlayedTodaySaveData(new());
		GameManager.Instance.eventsManager.UpdateEventStartedSaveData("");
		GameManager.Instance.eventsManager.UpdateRandomEventPlayedTodaySaveData(false);
		await GameManager.Instance.saveManager.SaveData();
	}

}
