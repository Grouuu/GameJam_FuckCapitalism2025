using System.Collections.Generic;
using UnityEngine;

public class PlayDialogState : StateCommand
{
	public int maxDialogsByDay;

	private List<string> _todayPlayedCharactersName;	// prevent to play twice the same character
	private int _todayPlayedDialogTotal;				// cap the number of dialogs by day
	private string forceDialog;							// force to show the same started dialog at restart

	public override void StartCommand (GameState previousState)
	{
		_todayPlayedCharactersName = new();
		_todayPlayedDialogTotal = 0;

		ApplySave();

		NextDialog();
	}

	private void OnEnable () => state = GameState.PlayDialog;

	private void ApplySave ()
	{
		List<string> charactersPlayedToday = GameManager.Instance.saveManager.GetSaveData<List<string>>(SaveItemKey.CharactersPlayedToday);
		string startedDialogName = GameManager.Instance.saveManager.GetSaveData<string>(SaveItemKey.DialogStarted);

		if (charactersPlayedToday != null && charactersPlayedToday.Count > 0)
			_todayPlayedCharactersName = charactersPlayedToday;

		if (GameManager.Instance.saveManager.HasKey(SaveItemKey.DialogsPlayedToday))
			_todayPlayedDialogTotal = GameManager.Instance.saveManager.GetSaveData<int>(SaveItemKey.DialogsPlayedToday);

		if (!string.IsNullOrEmpty(startedDialogName))
			forceDialog = startedDialogName;

		Debug.Log($"ApplySave on dialogs: played char total = {_todayPlayedCharactersName.Count}, played dialog total = {_todayPlayedDialogTotal}, force dialog = {forceDialog}");
	}

	private async void NextDialog ()
	{
		if (_todayPlayedDialogTotal > maxDialogsByDay)
		{
			EndCommand();
			return;
		}

		CharacterData selectedCharacter;
		DialogData selectedDialog;

		if (string.IsNullOrEmpty(forceDialog))
			(selectedCharacter, selectedDialog) = GameManager.Instance.charactersManager.PickDialog(_todayPlayedCharactersName.ToArray());
		else
		{
			selectedDialog = GameManager.Instance.charactersManager.GetDialogByName(forceDialog);
			selectedCharacter = GameManager.Instance.charactersManager.GetCharacterByDialogName(forceDialog);
		}

		forceDialog = null;

		if (selectedDialog != null)
		{
			_todayPlayedCharactersName.Add(selectedCharacter.name);
			_todayPlayedDialogTotal++;
			selectedDialog.isUsed = true;

			PlayDialog(selectedCharacter, selectedDialog);
		}
		else
		{
			Debug.Log("No dialog found");

			// clear save
			GameManager.Instance.charactersManager.UpdateCharactersPlayedTodaySaveData(new());
			GameManager.Instance.charactersManager.UpdateDialogStartedSaveData("");
			GameManager.Instance.charactersManager.UpdateTotalDialogPlayedTodaySaveData(0);
			await GameManager.Instance.saveManager.SaveData();

			EndCommand();
		}
	}

	private async void PlayDialog (CharacterData characterData, DialogData dialogData)
	{
		GameManager.Instance.charactersManager.UpdateDialogStartedSaveData(dialogData.name);
		await GameManager.Instance.saveManager.SaveData();

		dialogData.GenerateResultValue();

		DialogPanelUIData panelData = FormatDialogPanelRequestTexts(characterData, dialogData);
		GameManager.Instance.uiManager.ShowDialogPanel(panelData, () => OnYes(characterData, dialogData), () => OnNo(characterData, dialogData));
	}

	private async void OnYes (CharacterData characterData, DialogData dialogData)
	{
		ApplyResult(dialogData.yesResult);

		GameManager.Instance.charactersManager.UpdateDialogStartedSaveData("");
		GameManager.Instance.charactersManager.UpdateDialogsUsedSaveData();
		GameManager.Instance.charactersManager.UpdateCharactersPlayedTodaySaveData(_todayPlayedCharactersName);
		GameManager.Instance.charactersManager.UpdateTotalDialogPlayedTodaySaveData(_todayPlayedDialogTotal);
		await GameManager.Instance.saveManager.SaveData();

		PlayResponse(characterData, dialogData, dialogData.yesResult);
	}

	private async void OnNo (CharacterData characterData, DialogData dialogData)
	{
		ApplyResult(dialogData.noResult);

		GameManager.Instance.charactersManager.UpdateDialogStartedSaveData("");
		GameManager.Instance.charactersManager.UpdateDialogsUsedSaveData();
		GameManager.Instance.charactersManager.UpdateCharactersPlayedTodaySaveData(_todayPlayedCharactersName);
		GameManager.Instance.charactersManager.UpdateTotalDialogPlayedTodaySaveData(_todayPlayedDialogTotal);
		await GameManager.Instance.saveManager.SaveData();

		PlayResponse(characterData, dialogData, dialogData.noResult);
	}

	private void PlayResponse (CharacterData characterData, DialogData dialogData, DialogResultData result)
	{
		DialogPanelUIData panelData = FormatDialogPanelResponseTexts(characterData, dialogData, result);
		GameManager.Instance.uiManager.ShowDialogPanel(panelData, () => EndDialog());
	}

	private void EndDialog ()
	{
		if (GameManager.Instance.endingsManager.CheckLose())
		{
			// no need to save
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
			NextDialog();
	}

	private void AfterWin ()
	{
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
