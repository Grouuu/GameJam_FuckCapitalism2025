using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
	public EventData[] events;

	public EventData PickEvent (string[] ignoredEvents)
	{
		int currentDay = GameManager.Instance.resourcesManager.GetResourceValue(ResourceId.Days);

		// all available events for the day not already played today
		EventData[] dayEvents = events
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

}
