using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayDialogState : StateCommand
{
	public int maxDialogsByDay;

	public override GameState state => GameState.PlayDialog;

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
	}

	private async void NextDialog ()
	{
		if (_todayPlayedDialogTotal > maxDialogsByDay)
		{
			Debug.Log("Max dialogs played");
			await ClearSaveData();
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
			await ClearSaveData();
			EndCommand();
		}
	}

	private async void PlayDialog (CharacterData characterData, DialogData dialogData)
	{
		await dialogData.UpdateEnterSceneEffects();

		GameManager.Instance.charactersManager.UpdateDialogStartedSaveData(dialogData.name);
		await GameManager.Instance.saveManager.SaveData();

		dialogData.GenerateResultValue();

		DialogPanelUIData panelData = FormatDialogPanelRequestTexts(characterData, dialogData);
		GameManager.Instance.uiManager.ShowDialogPanel(panelData, () => OnYes(characterData, dialogData), () => OnNo(characterData, dialogData));
	}

	private async void OnYes (CharacterData characterData, DialogData dialogData)
	{
		ApplyResult(dialogData.yesResult);

		await SaveAnswer();

		await dialogData.UpdateYesEnterSceneEffects();

		PlayResponse(characterData, dialogData, dialogData.yesResult, true);
	}

	private async void OnNo (CharacterData characterData, DialogData dialogData)
	{
		ApplyResult(dialogData.noResult);

		await SaveAnswer();

		await dialogData.UpdateNoEnterSceneEffects();

		PlayResponse(characterData, dialogData, dialogData.noResult, false);
	}

	private async Awaitable SaveAnswer ()
	{
		GameManager.Instance.charactersManager.UpdateDialogStartedSaveData("");
		GameManager.Instance.charactersManager.UpdateDialogsUsedSaveData();
		GameManager.Instance.charactersManager.UpdateCharactersPlayedTodaySaveData(_todayPlayedCharactersName);
		GameManager.Instance.charactersManager.UpdateTotalDialogPlayedTodaySaveData(_todayPlayedDialogTotal);
		await GameManager.Instance.saveManager.SaveData();
	}

	private void PlayResponse (CharacterData characterData, DialogData dialogData, DialogResultData result, bool isYes)
	{
		DialogPanelUIData panelData = FormatDialogPanelResponseTexts(characterData, dialogData, result);
		GameManager.Instance.uiManager.ShowDialogPanel(panelData, () => EndDialog(dialogData, isYes));
	}

	private async void EndDialog (DialogData dialogData, bool isYes)
	{
		List<Task> tasks = new();

		tasks.Add(dialogData.UpdateExitSceneEffects());

		if (isYes)
			tasks.Add(dialogData.UpdateYesExitSceneEffects());
		else
			tasks.Add(dialogData.UpdateNoExitSceneEffects());

		await Task.WhenAll(tasks.ToArray());

		if (GameManager.Instance.endingsManager.CheckLose() != null)
		{
			// no need to save
			EndCommand(GameState.EndGame);
			return;
		}

		CheckWin();
	}

	private async void CheckWin ()
	{
		EndingData endingData = GameManager.Instance.endingsManager.CheckWin();

		if (endingData != null)
		{
			await endingData.UpdateEnterSceneEffects();
			GameManager.Instance.endingsManager.ShowWin(() => NextDialog());
			await endingData.UpdateExitSceneEffects();
		}
		else
			NextDialog();
	}

	private void ApplyResult (ResultData resultData)
	{
		resultData.ApplyResult();
	}

	private DialogPanelUIData FormatDialogPanelRequestTexts (CharacterData characterData, DialogData dialogData)
	{
		DialogPanelUIData panelData = new();

		panelData.contentTermKey = dialogData.name;
		panelData.contentTermCat = LocCat.DialogsRequests;
		panelData.character = characterData;
		panelData.buttons = DialogPanelUIButtonsLayout.YesNo;

		return panelData;
	}

	private DialogPanelUIData FormatDialogPanelResponseTexts (CharacterData characterData, DialogData dialogData, DialogResultData result)
	{
		DialogPanelUIData panelData = new();

		panelData.contentTermKey = dialogData.name;
		panelData.contentTermCat = result.isYes ? LocCat.DialogsYes : LocCat.DialogsNo;
		panelData.varChanges = result.varChanges;
		panelData.character = characterData;
		panelData.buttons = DialogPanelUIButtonsLayout.Continue;

		return panelData;
	}

	private async Awaitable ClearSaveData ()
	{
		// clear save
		GameManager.Instance.charactersManager.UpdateCharactersPlayedTodaySaveData(new());
		GameManager.Instance.charactersManager.UpdateDialogStartedSaveData("");
		GameManager.Instance.charactersManager.UpdateTotalDialogPlayedTodaySaveData(0);
		await GameManager.Instance.saveManager.SaveData();
	}

}
