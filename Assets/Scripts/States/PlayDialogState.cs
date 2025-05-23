using System.Collections.Generic;

public class PlayDialogState : StateCommand
{
	public int maxDialogsByDay;

	private List<string> _todayPlayedCharactersId;	// prevent to play twice the same character
	private int _todayPlayedDialogTotal;			// cap the number of dialogs by day

	public override void StartCommand ()
	{
		_todayPlayedCharactersId = new();
		_todayPlayedDialogTotal = 0;

		NextDialog();
	}

	private void OnEnable () => state = GameState.PlayDialog;

	private void NextDialog ()
	{
		if (_todayPlayedDialogTotal > maxDialogsByDay)
		{
			EndCommand();
			return;
		}

		(CharacterData selectedCharacter, DialogData selectedDialog) = GameManager.Instance.charactersManager.PickDialog(_todayPlayedCharactersId.ToArray());

		if (selectedDialog != null)
		{
			_todayPlayedCharactersId.Add(selectedCharacter.id);
			_todayPlayedDialogTotal++;
			selectedDialog.isUsed = true;

			PlayDialog(selectedCharacter, selectedDialog);
		}
		else
			EndCommand();
	}

	private void PlayDialog (CharacterData characterData, DialogData dialogData)
	{
		dialogData.GenerateResultValue();

		DialogPanelUIData panelData = FormatDialogPanelRequestTexts(characterData, dialogData);
		GameManager.Instance.uiManager.ShowDialogPanel(panelData, () => OnYes(characterData, dialogData), () => OnNo(characterData, dialogData));
	}

	private void OnYes (CharacterData characterData, DialogData dialogData)
	{
		PlayResponse(characterData, dialogData, dialogData.yesResult);
	}

	private void OnNo (CharacterData characterData, DialogData dialogData)
	{
		PlayResponse(characterData, dialogData, dialogData.noResult);
	}

	private void PlayResponse (CharacterData characterData, DialogData dialogData, DialogResultData result)
	{
		DialogPanelUIData panelData = FormatDialogPanelResponseTexts(characterData, dialogData, result);
		GameManager.Instance.uiManager.ShowDialogPanel(panelData, () => EndDialog(result));
	}

	private void EndDialog (DialogResultData result)
	{
		ApplyResult(result);
		NextDialog();
	}

	private void ApplyResult (ResultData resultData)
	{
		resultData.ApplyResult();
	}

	private DialogPanelUIData FormatDialogPanelRequestTexts (CharacterData characterData, DialogData dialogData)
	{
		DialogPanelUIData panelData = new();

		panelData.content = dialogData.request;
		panelData.character = characterData;
		panelData.buttons = DialogPanelUIButtonsLayout.YesNo;

		return panelData;
	}

	private DialogPanelUIData FormatDialogPanelResponseTexts (CharacterData characterData, DialogData dialogData, DialogResultData result)
	{
		DialogPanelUIData panelData = new();

		panelData.content = result.response;
		panelData.character = characterData;
		panelData.buttons = DialogPanelUIButtonsLayout.Continue;

		return panelData;
	}

}
