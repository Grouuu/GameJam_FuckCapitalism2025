using System.Collections.Generic;

public class PlayEventState : StateCommand
{
	private List<string> _todayPlayedEventsId;

	public override void StartCommand ()
	{
		_todayPlayedEventsId = new();

		NextEvent();
	}

	private void OnEnable () => state = GameState.PlayEvent;

	private void NextEvent ()
	{
		EventData selectedEvent = GameManager.Instance.eventsManager.PickEvent(_todayPlayedEventsId.ToArray());

		if (selectedEvent)
		{
			_todayPlayedEventsId.Add(selectedEvent.id);
			selectedEvent.isUsed = true;

			PlayEvent(selectedEvent);
		}
		else
			EndCommand();
	}

	private void PlayEvent (EventData eventData)
	{
		// Generate random values
		eventData.result.UpdateResult();

		CentralPanelUIData panelData = FormatEventPanelTexts(eventData);

		GameManager.Instance.uiManager.ShowEventPanel(panelData, () => OnContinue(eventData));
	}

	private void OnContinue (EventData eventData)
	{
		ApplyResult(eventData.result);
		NextEvent();
	}

	private void ApplyResult (ResultData resultData)
	{
		resultData.ApplyResult();
	}

	private CentralPanelUIData FormatEventPanelTexts (EventData eventData)
	{
		CentralPanelUIData panelData = new();

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
