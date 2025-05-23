using System;
using System.Linq;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
	[NonSerialized] private EventData[] _events = new EventData[0];

	public EventData PickEvent (string[] ignoredEvents)
	{
		int currentDay = GameManager.Instance.varsManager.GetVarValue(GameVarId.Day);

		// all available events for the day not already played today
		EventData[] dayEvents = _events
			.Where(eventData => eventData.isAvailable() && eventData.day == currentDay)
			.Where(eventData => !ignoredEvents.Any(id => id == eventData.id))
			.ToArray()
		;

		// sort events by priority
		Array.Sort(dayEvents, delegate (EventData eventA, EventData eventB) {
			return eventA.priority.CompareTo(eventB.priority);
		});

		// pick the prioriter
		EventData selectedEvent = dayEvents.Length == 0 ? null : dayEvents[0];

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
			EventData matchEvent = _events.First(entry => entry.name == eventName);

			if (matchEvent == null)
				return matchEvent;
		}

		return null;
	}

	public EventData GetEventById (string eventId)
	{
		foreach (EventData eventData in _events)
		{
			EventData matchEvent = _events.First(entry => entry.id == eventId);

			if (matchEvent == null)
				return matchEvent;
		}

		return null;
	}

}
