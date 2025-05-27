using System.Collections.Generic;

public class PlayEventState : StateCommand
{
	private List<string> _todayPlayedEventsId;

	public override void StartCommand (GameState previousState)
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
		ApplyResult(eventData.result);

		EventPanelUIData panelData = FormatEventPanelTexts(eventData);
		GameManager.Instance.uiManager.ShowEventPanel(panelData, () => EndEvent());
	}

	private void EndEvent ()
	{
		if (GameManager.Instance.endingsManager.CheckLose())
		{
			EndCommand(GameState.EndGame);
			return;
		}

		CheckWin();
	}

	private void CheckWin ()
	{
		if (GameManager.Instance.endingsManager.CheckWin())
			GameManager.Instance.endingsManager.ShowWin(() => AfterWin());
		else
			NextEvent();
	}

	private void AfterWin ()
	{
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

				VarData varData = GameManager.Instance.varsManager.GetVarData(varChange.varId);

				if (varData.type != GameVarType.UIVar)
					continue;

				string name = varData.displayName;
				string modifier = varChange.currentValue > 0 ? "+" : "";
				content += $"{name} : {modifier}{varChange.currentValue}\n";
			}
		}

		panelData.content = content;

		return panelData;
	}

}
