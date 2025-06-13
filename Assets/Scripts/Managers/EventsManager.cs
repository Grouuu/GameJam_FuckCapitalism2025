using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EventDataType
{
	None,
	FixedDay,
	RequireTrue,
	Random,
}

public class EventsManager : MonoBehaviour
{
	[NonSerialized] private EventData[] _events = new EventData[0];

	public bool debug = false;

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

		selectedEvent = PickRandomEvent(ignoredEvents, isRandomEventPlayed);

		if (selectedEvent != null)
			return selectedEvent;

		return null;
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

		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.EventsUsed, eventsUsed);
	}

	public void UpdateEventsDaySaveData ()
	{
		List<KeyValuePair<string, int>> eventsDay = new();

		foreach (EventData eventData in _events)
		{
			eventsDay.Add(new KeyValuePair<string, int>(eventData.name, eventData.day));
		}

		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.EventsDay, eventsDay);
	}

	public void UpdateEventsPlayedTodaySaveData (List<string> eventsName)
	{
		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.EventsPlayedToday, eventsName);
	}

	public void UpdateEventStartedSaveData (string eventName)
	{
		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.EventStarted, eventName);
	}

	public void UpdateRandomEventPlayedTodaySaveData(bool isPlayed)
	{
		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.RandomEventPlayed, isPlayed);
	}

	public void ApplySave ()
	{
		List<string> eventUsed = GameManager.Instance.saveManager.GetSaveData<List<string>>(SaveItemKey.EventsUsed);
		List<KeyValuePair<string, int>> eventsDay = GameManager.Instance.saveManager.GetSaveData<List<KeyValuePair<string, int>>>(SaveItemKey.EventsDay);

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
		int currentDay = GameManager.Instance.varsManager.GetVarValue(GameVarId.Day);

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

		if (debug)
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

		if (debug)
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

		if (debug)
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

			int maxRandom = GameManager.Instance.varsManager.GetVarValue(GameVarId.RandomEventGeneratorMax);
			int weigth = randomEvent.randomWeight;
			bool validated = UnityEngine.Random.Range(0, maxRandom) <= weigth;

			if (validated)
				return randomEvent;
		}

		return null;
	}

}
