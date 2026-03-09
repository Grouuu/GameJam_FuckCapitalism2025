using Grouuu.Constants;
using Grouuu.Data;
using Grouuu.Enum;
using Grouuu.PersistentManagers;
using Grouuu.UI;
using Grouuu.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Grouuu.States
{
	public class PlayEventState : StateCommand
	{
		public override GameState State => GameState.PlayEvent;

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
			List<string> eventsPlayedToday = GameController.GameManagers.SaveManager.GetSaveData<List<string>>(SaveItemKey.EventsPlayedToday);
			string startedEventName = GameController.GameManagers.SaveManager.GetSaveData<string>(SaveItemKey.EventStarted);

			if (eventsPlayedToday != null && eventsPlayedToday.Count > 0)
				_todayPlayedEventsName = eventsPlayedToday;

			if (GameController.GameManagers.SaveManager.HasKey(SaveItemKey.RandomEventPlayed))
				_randomEventPlayed = GameController.GameManagers.SaveManager.GetSaveData<bool>(SaveItemKey.RandomEventPlayed);

			if (!string.IsNullOrEmpty(startedEventName))
				forceEvent = startedEventName;
		}

		private async void NextEvent ()
		{
			EventData selectedEvent;

			if (string.IsNullOrEmpty(forceEvent))
				selectedEvent = GameController.GameManagers.EventsManager.PickEvent(_todayPlayedEventsName.ToArray(), _randomEventPlayed);
			else
				selectedEvent = GameController.GameManagers.EventsManager.GetEventByName(forceEvent);

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

			GameController.GameManagers.EventsManager.UpdateEventStartedSaveData(eventData.name);
			await GameController.GameManagers.SaveManager.SaveData();

			EventPanelUIData panelData = FormatEventPanelTexts(eventData);
			GameController.GameManagers.UiManager.ShowEventPanel(panelData, () => EndEvent(eventData));
		}

		private async void EndEvent (EventData eventData)
		{
			ApplyResult(eventData.result);

			if (eventData.type == EventDataType.Random)
				GameController.GameManagers.EventsManager.UpdateRandomEventPlayedTodaySaveData(true);

			GameController.GameManagers.EventsManager.UpdateEventsPlayedTodaySaveData(_todayPlayedEventsName);
			GameController.GameManagers.EventsManager.UpdateEventStartedSaveData("");
			GameController.GameManagers.EventsManager.UpdateEventsUsedSaveData();
			GameController.GameManagers.EventsManager.UpdateEventsDaySaveData();
			await GameController.GameManagers.SaveManager.SaveData();

			await eventData.UpdateExitSceneEffects();

			if (GameController.GameManagers.EndingsManager.CheckLose() != null)
			{
				// no need to save
				base.EndCommand(GameState.EndGame);
				return;
			}

			CheckWin();
		}

		private async void CheckWin ()
		{
			EndingData endingData = GameController.GameManagers.EndingsManager.CheckWin();

			if (endingData != null)
			{
				await endingData.UpdateEnterSceneEffects();
				GameController.GameManagers.EndingsManager.ShowWin(() => NextEvent());
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
			GameController.GameManagers.EventsManager.UpdateEventsPlayedTodaySaveData(new());
			GameController.GameManagers.EventsManager.UpdateEventStartedSaveData("");
			GameController.GameManagers.EventsManager.UpdateRandomEventPlayedTodaySaveData(false);
			await GameController.GameManagers.SaveManager.SaveData();
		}
	}
}