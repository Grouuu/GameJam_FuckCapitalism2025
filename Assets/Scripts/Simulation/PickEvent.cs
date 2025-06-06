using System.Collections.Generic;
using System.Linq;

public class PickEvent
{
	protected DatabaseController database;

	protected List<string> ignoredEvents = new();
	protected bool isRandomEventPlayed = false;

	public void Init (DatabaseController database)
	{
		this.database = database;
	}

	public void Reset ()
	{
		ignoredEvents = new();
		isRandomEventPlayed = false;
	}

	public EventData GetNextEvent ()
	{
		EventData eventData = Next();

		if (eventData != null)
			UpdatePick(eventData);
		else
			Reset();

		return eventData;
	}

	protected void UpdatePick (EventData eventData)
	{
		ignoredEvents.Add(eventData.name);

		eventData.isUsed = true;
		eventData.GenerateResultValue();

		if (eventData.type == EventType.Random)
			isRandomEventPlayed = true;
	}

	protected EventData Next ()
	{
		EventData selectedEvent = PickFixedDayEvent();

		if (selectedEvent != null)
			return selectedEvent;

		selectedEvent = PickRequireTrueEvent();

		if (selectedEvent != null)
			return selectedEvent;

		selectedEvent = PickRandomEvent();

		if (selectedEvent != null)
			return selectedEvent;

		return null;
	}

	private EventData PickFixedDayEvent ()
	{
		int currentDay = database.GetVarData(GameVarId.Day).currentValue;

		EventData[] dayEvents = database.eventsData
			.Where(eventData => !ignoredEvents.Any(id => id == eventData.name) && eventData.isAvailable() && eventData.day == currentDay)
			.OrderBy(eventData => eventData.priority)
			.ToArray()
		;

		return dayEvents.Length == 0 ? null : dayEvents[0];
	}

	private EventData PickRequireTrueEvent ()
	{
		EventData[] availableEvents = database.eventsData
			.Where(eventData => !ignoredEvents.Any(id => id == eventData.name) && eventData.isAvailable() && eventData.type == EventType.RequireTrue)
			.OrderBy(eventData => eventData.priority)
			.ToArray()
		;

		return availableEvents.Length == 0 ? null : availableEvents[0];
	}

	private EventData PickRandomEvent ()
	{
		if (isRandomEventPlayed)
			return null;

		EventData[] randomEvents = database.eventsData
			.Where(eventData => !ignoredEvents.Any(id => id == eventData.name))
			.Where(eventData => !ignoredEvents.Any(id => id == eventData.name) && eventData.isAvailable() && eventData.type == EventType.Random)
			.ToArray()
		;

		if (randomEvents.Length > 0)
		{
			int maxRandom = database.GetVarData(GameVarId.RandomEventGeneratorMax).currentValue;
			EventData randomEvent = randomEvents[UnityEngine.Random.Range(0, randomEvents.Length)];
			int weigth = randomEvent.randomWeight;
			bool validated = UnityEngine.Random.Range(0, maxRandom) <= weigth;

			if (validated)
				return randomEvent;
		}

		return null;
	}

}
