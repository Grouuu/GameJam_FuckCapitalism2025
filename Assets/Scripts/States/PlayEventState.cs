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

		if (selectedEvent != null)
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
		eventData.GenerateResultValue();

		EventPanelUIData panelData = FormatEventPanelTexts(eventData);
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

	private EventPanelUIData FormatEventPanelTexts (EventData eventData)
	{
		EventPanelUIData panelData = new();

		panelData.title = eventData.title;

		string content = "";

		content += $"{eventData.description}";

		ResultVarChange[] changes = eventData.result.varChanges;

		if (changes != null && changes.Length > 0)
		{
			content += "\n\n";

			foreach (ResultVarChange varChange in changes)
			{
				if (varChange.currentValue == 0)
					continue;

				string name = GameManager.Instance.varsManager.GetVarDisplayName(varChange.varId);
				string modifier = varChange.currentValue > 0 ? "+" : "";
				content += $"{name} : {modifier}{varChange.currentValue}\n";
			}
		}

		panelData.content = content;

		return panelData;
	}

}
