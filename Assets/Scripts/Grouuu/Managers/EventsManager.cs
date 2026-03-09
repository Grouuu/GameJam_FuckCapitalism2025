using Grouuu.Constants;
using Grouuu.Data;
using Grouuu.Enum;
using Grouuu.PersistentManagers;
using Grouuu.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grouuu.Managers
{
	public class EventsManager : IManager
	{
		private readonly EventsManagerSettings _settings;
		private EventData[] _events = new EventData[0];

		public EventsManager (EventsManagerSettings settings)
		{
			_settings = settings;
		}

		public void InitEvents (EventData[] events)
		{
			if (events == null)
			{
				Debug.LogError($"No events to init");
				return;
			}

			Debug.Log($"Events loaded (total: {events.Length})");

			_events = events;
		}

		public EventData PickEvent (string[] ignoredEvents, bool isRandomEventPlayed)
		{
			EventData selectedEvent = PickFixedDayEvent(ignoredEvents);

			if (selectedEvent != null)
				return selectedEvent;

			selectedEvent = PickRequireTrueEvent(ignoredEvents);

			if (selectedEvent != null)
				return selectedEvent;

			// no random event on day 0
			if (GameController.GameManagers.VarsManager.GetVarValue(GameVarId.Day) != 0)
				selectedEvent = PickRandomEvent(ignoredEvents, isRandomEventPlayed);

			return selectedEvent;
		}

		public void SetEventUsed (string eventName, bool isUsed)
		{
			EventData eventData = GetEventByName(eventName);

			if (eventData != null)
				eventData.isUsed = isUsed;
		}

		public EventData GetEventByName (string eventName)
		{
			foreach (EventData eventData in _events)
			{
				EventData matchEvent = _events.FirstOrDefault(entry => entry.name == eventName);

				if (matchEvent != null)
					return matchEvent;
			}

			return null;
		}

		public void UpdateEventsUsedSaveData ()
		{
			List<string> eventsUsed = new();

			foreach (EventData eventData in _events)
			{
				if (eventData.isUsed)
					eventsUsed.Add(eventData.name);
			}

			GameController.GameManagers.SaveManager.AddToSaveData(SaveItemKey.EventsUsed, eventsUsed);
		}

		public void UpdateEventsDaySaveData ()
		{
			List<KeyValuePair<string, int>> eventsDay = new();

			foreach (EventData eventData in _events)
			{
				eventsDay.Add(new KeyValuePair<string, int>(eventData.name, eventData.day));
			}

			GameController.GameManagers.SaveManager.AddToSaveData(SaveItemKey.EventsDay, eventsDay);
		}

		public void UpdateEventsPlayedTodaySaveData (List<string> eventsName)
		{
			GameController.GameManagers.SaveManager.AddToSaveData(SaveItemKey.EventsPlayedToday, eventsName);
		}

		public void UpdateEventStartedSaveData (string eventName)
		{
			GameController.GameManagers.SaveManager.AddToSaveData(SaveItemKey.EventStarted, eventName);
		}

		public void UpdateRandomEventPlayedTodaySaveData (bool isPlayed)
		{
			GameController.GameManagers.SaveManager.AddToSaveData(SaveItemKey.RandomEventPlayed, isPlayed);
		}

		public void ApplySave ()
		{
			List<string> eventUsed = GameController.GameManagers.SaveManager.GetSaveData<List<string>>(SaveItemKey.EventsUsed);
			List<KeyValuePair<string, int>> eventsDay = GameController.GameManagers.SaveManager.GetSaveData<List<KeyValuePair<string, int>>>(SaveItemKey.EventsDay);

			if (eventUsed != null)
			{
				foreach (string eventName in eventUsed)
				{
					EventData eventData = GetEventByName(eventName);

					if (eventData == null)
						continue;

					eventData.isUsed = true;
				}
			}

			if (eventsDay != null)
			{
				foreach ((string eventName, int eventDay) in eventsDay)
				{
					EventData eventData = GetEventByName(eventName);

					if (eventData == null)
						continue;

					eventData.day = eventDay;
				}
			}
		}

		public EventData[] GetEvents ()
		{
			return _events;
		}

		private EventData PickFixedDayEvent (string[] ignoredEvents)
		{
			int currentDay = GameController.GameManagers.VarsManager.GetVarValue(GameVarId.Day);

			// all available events for the day not already played today
			EventData[] dayEvents = _events
				.Where(eventData => !ignoredEvents.Any(id => id == eventData.name))
				.Where(eventData => eventData.isAvailable() && eventData.day == currentDay)
				.ToArray()
			;

			// sort events by priority
			Array.Sort(dayEvents, delegate (EventData eventA, EventData eventB)
			{
				return eventA.priority.CompareTo(eventB.priority);
			});

			if (_settings.Debug)
			{
				Debug.Log($"---- FIXED DAY EVENTS ----------");
				foreach (var eventData in dayEvents)
					Debug.Log($"<color=#7FFF00>{eventData.name}</color>");
			}

			// pick the prioriter from today events
			EventData selectedEvent = dayEvents.Length == 0 ? null : dayEvents[0];

			if (selectedEvent != null)
				return selectedEvent;

			return null;
		}

		private EventData PickRequireTrueEvent (string[] ignoredEvents)
		{
			// all available events for the day not already played today
			EventData[] availableEvents = _events
				.Where(eventData => !ignoredEvents.Any(id => id == eventData.name))
				.Where(eventData => eventData.isAvailable() && eventData.type == EventDataType.RequireTrue)
				.ToArray()
			;

			// sort events by priority
			Array.Sort(availableEvents, delegate (EventData eventA, EventData eventB)
			{
				return eventA.priority.CompareTo(eventB.priority);
			});

			if (_settings.Debug)
			{
				Debug.Log($"---- REQUIRE TRUE EVENTS ----------");
				foreach (var eventData in availableEvents)
					Debug.Log($"<color=#7FFF00>{eventData.name}</color>");
			}

			// pick the prioriter from today events
			EventData selectedEvent = availableEvents.Length == 0 ? null : availableEvents[0];

			if (selectedEvent != null)
				return selectedEvent;

			return null;
		}

		private EventData PickRandomEvent (string[] ignoredEvents, bool isRandomEventPlayed)
		{
			// all random events not already played today
			EventData[] randomEvents = _events
				.Where(eventData => !ignoredEvents.Any(id => id == eventData.name))
				.Where(eventData => eventData.isAvailable() && eventData.type == EventDataType.Random)
				.ToArray()
			;

			if (_settings.Debug)
			{
				Debug.Log($"---- RANDOM EVENTS ----------");
				Debug.Log($"Random event already played: {isRandomEventPlayed}");
				foreach (var eventData in randomEvents)
					Debug.Log($"<color=#7FFF00>{eventData.name}</color>");
			}

			if (!isRandomEventPlayed && randomEvents.Length > 0)
			{
				int randomIndex = UnityEngine.Random.Range(0, randomEvents.Length);
				EventData randomEvent = randomEvents[randomIndex];

				int maxRandom = GameController.GameManagers.VarsManager.GetVarValue(GameVarId.RandomEventGeneratorMax);
				int weigth = randomEvent.randomWeight;
				bool validated = UnityEngine.Random.Range(0, maxRandom) <= weigth;

				if (validated)
					return randomEvent;
			}

			return null;
		}
	}
}