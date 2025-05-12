using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
	public EventData[] events;

	private List<EventData> _eventsAvailable = new();

	public void UpdateAvailableEvents ()
	{
		_eventsAvailable = new List<EventData>();

		if (events == null)
			return;

		foreach (EventData eventData in events)
		{
			if (eventData.isAvailable())
				_eventsAvailable.Add(eventData);
		}
	}

	public EventData[] SelectEventsByDay (int day)
	{
		return _eventsAvailable.Where(e => e.day == day).ToArray();
	}

}
