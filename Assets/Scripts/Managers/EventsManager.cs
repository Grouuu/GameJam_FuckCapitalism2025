using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
	[NonSerialized] private EventData[] _events = new EventData[0];

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

	public EventData PickEvent (string[] ignoredEvents)
	{
		int currentDay = GameManager.Instance.varsManager.GetVarValue(GameVarId.Day);

		// all available events for the day not already played today
		EventData[] dayEvents = _events
			.Where(eventData => !ignoredEvents.Any(id => id == eventData.id))
			.Where(eventData => eventData.isAvailable() && eventData.day == currentDay)
			.ToArray()
		;

		// sort events by priority
		Array.Sort(dayEvents, delegate (EventData eventA, EventData eventB)
		{
			return eventA.priority.CompareTo(eventB.priority);
		});

		// all random events not already played today
		EventData[] randomEvents = _events
			.Where(eventData => !ignoredEvents.Any(id => id == eventData.id))
			.Where(eventData => eventData.isAvailable() && eventData.day == -1)
			.ToArray()
		;

		// sort events by priority
		Array.Sort(randomEvents, delegate (EventData eventA, EventData eventB)
		{
			return eventA.priority.CompareTo(eventB.priority);
		});

		// pick the prioriter from today events
		EventData selectedEvent = dayEvents.Length == 0 ? null : dayEvents[0];

		// otherwise pick the prioriter from random events
		if (selectedEvent == null)
			selectedEvent = randomEvents.Length == 0 ? null : randomEvents[0];

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

	public EventData GetEventById (string eventId)
	{
		foreach (EventData eventData in _events)
		{
			EventData matchEvent = _events.FirstOrDefault(entry => entry.id == eventId);

			if (matchEvent != null)
				return matchEvent;
		}

		return null;
	}

	public void UpdateSaveData ()
	{
		List<KeyValuePair<string, int>> eventsDay = new();
		List<string> eventsUsed = new();

		foreach (EventData eventData in _events)
		{
			eventsDay.Add(new KeyValuePair<string, int>(eventData.name, eventData.day));

			if (eventData.isUsed)
				eventsUsed.Add(eventData.name);
		}

		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.EventsDay, eventsDay);
		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.EventsUsed, eventsDay);
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

}
