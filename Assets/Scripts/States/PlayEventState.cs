using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayEventState : StateCommand
{
	private List<string> _todayPlayedEventsId;

	public override void StartCommand ()
	{
		_todayPlayedEventsId = new();

		NextEvent();
	}

	protected override void Reset () {}

	private void OnEnable () => state = GameState.PlayEvent;

	private void NextEvent ()
	{
		GameManager gm = GameManager.Instance;

		gm.eventsManager.UpdateAvailableEvents();

		int currentDay = gm.resourcesManager.GetResourceValue(ResourceId.Days);

		// all available events for the day not already played today
		EventData[] dayEvents = gm.eventsManager.SelectEventsByDay(currentDay)
			.Where(eventData => !_todayPlayedEventsId.Any(id => id == eventData.id))
			.ToArray()
		;

		// sort events by priority
		Array.Sort(dayEvents, delegate (EventData eventA, EventData eventB) {
			return eventA.priority.CompareTo(eventB.priority);
		});

		// pick the prioriter
		EventData selectedEvent = dayEvents.Length == 0 ? null : dayEvents[0];

		if (selectedEvent)
		{
			_todayPlayedEventsId.Add(selectedEvent.id);

			selectedEvent.isUsed = true;

			PlayEvent(selectedEvent);
		}
		else
		{
			EndCommand();
		}
	}

	private void PlayEvent (EventData eventData)
	{
		// Generate random values
		eventData.result.UpdateResult();

		GameManager gm = GameManager.Instance;
		PanelUIData data = FormatEventPanelTexts(eventData);

		gm.uiManager.ShowEvent(data, () => WaitForContinue(eventData));
	}

	private void WaitForContinue (EventData eventData)
	{
		ApplyResult(eventData);
		NextEvent();
	}

	private void ApplyResult (EventData eventData)
	{
		eventData.result.ApplyResult();
	}

	private PanelUIData FormatEventPanelTexts (EventData eventData)
	{
		PanelUIData panelData = new();

		panelData.title = eventData.title;

		string content = "";

		content += $"{eventData.description}";

		if (eventData.result.resourcesChanged != null && eventData.result.resourcesChanged.Length > 0)
		{
			content += "\n\n";

			foreach (ResourceData resourceChange in eventData.result.resourcesChanged)
			{
				content += $"{resourceChange.id} : {resourceChange.value}\n";
			}
		}

		panelData.content = content;

		return panelData;
	}

}
