using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Grouuu.Constants;
using Grouuu.Data;
using Grouuu.Enum;
using Grouuu.PersistentManagers;
using Grouuu.UI;
using Grouuu.Utils;
using UnityEngine;

namespace Grouuu.States
{
	public class PlayDialogState : StateCommand
	{
		public int maxDialogsByDay;

		public override GameState State => GameState.PlayDialog;

		private List<string> _todayPlayedCharactersName;    // prevent to play twice the same character
		private int _todayPlayedDialogTotal;                // cap the number of dialogs by day
		private string forceDialog;                         // force to show the same started dialog at restart

		public override void StartCommand (GameState previousState)
		{
			_todayPlayedCharactersName = new();
			_todayPlayedDialogTotal = 0;

			ApplySave();

			NextDialog();
		}

		private void ApplySave ()
		{
			List<string> charactersPlayedToday = GameController.GameManagers.SaveManager.GetSaveData<List<string>>(SaveItemKey.CharactersPlayedToday);
			string startedDialogName = GameController.GameManagers.SaveManager.GetSaveData<string>(SaveItemKey.DialogStarted);

			if (charactersPlayedToday != null && charactersPlayedToday.Count > 0)
				_todayPlayedCharactersName = charactersPlayedToday;

			if (GameController.GameManagers.SaveManager.HasKey(SaveItemKey.DialogsPlayedToday))
				_todayPlayedDialogTotal = GameController.GameManagers.SaveManager.GetSaveData<int>(SaveItemKey.DialogsPlayedToday);

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
				(selectedCharacter, selectedDialog) = GameController.GameManagers.CharactersManager.PickDialog(_todayPlayedCharactersName.ToArray());
			else
			{
				selectedDialog = GameController.GameManagers.CharactersManager.GetDialogByName(forceDialog);
				selectedCharacter = GameController.GameManagers.CharactersManager.GetCharacterByDialogName(forceDialog);
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

			GameController.GameManagers.CharactersManager.UpdateDialogStartedSaveData(dialogData.name);
			await GameController.GameManagers.SaveManager.SaveData();

			dialogData.GenerateResultValue();

			GameController.GameManagers.DebugManager.DialogChoice.Clear();
			GameController.GameManagers.DebugManager.DialogChoice.AddDialogResourcesInfluence(dialogData);

			DialogPanelUIData panelData = FormatDialogPanelRequestTexts(characterData, dialogData);
			GameController.GameManagers.UiManager.ShowDialogPanel(panelData, () => OnYes(characterData, dialogData), () => OnNo(characterData, dialogData));
		}

		private async void OnYes (CharacterData characterData, DialogData dialogData)
		{
			GameController.GameManagers.DebugManager.DialogChoice.RemoveDialogResourcesInfluence();

			ApplyResult(dialogData.yesResult);

			await SaveAnswer();

			await dialogData.UpdateYesEnterSceneEffects();

			PlayResponse(characterData, dialogData, dialogData.yesResult, true);
		}

		private async void OnNo (CharacterData characterData, DialogData dialogData)
		{
			GameController.GameManagers.DebugManager.DialogChoice.RemoveDialogResourcesInfluence();

			ApplyResult(dialogData.noResult);

			await SaveAnswer();

			await dialogData.UpdateNoEnterSceneEffects();

			PlayResponse(characterData, dialogData, dialogData.noResult, false);
		}

		private async Awaitable SaveAnswer ()
		{
			GameController.GameManagers.CharactersManager.UpdateDialogStartedSaveData("");
			GameController.GameManagers.CharactersManager.UpdateDialogsUsedSaveData();
			GameController.GameManagers.CharactersManager.UpdateCharactersPlayedTodaySaveData(_todayPlayedCharactersName);
			GameController.GameManagers.CharactersManager.UpdateTotalDialogPlayedTodaySaveData(_todayPlayedDialogTotal);
			await GameController.GameManagers.SaveManager.SaveData();
		}

		private void PlayResponse (CharacterData characterData, DialogData dialogData, DialogResultData result, bool isYes)
		{
			DialogPanelUIData panelData = FormatDialogPanelResponseTexts(characterData, dialogData, result);
			GameController.GameManagers.UiManager.ShowDialogPanel(panelData, () => EndDialog(dialogData, isYes));
		}

		private async void EndDialog (DialogData dialogData, bool isYes)
		{
			List<UniTask> tasks = new();

			tasks.Add(dialogData.UpdateExitSceneEffects());

			if (isYes)
				tasks.Add(dialogData.UpdateYesExitSceneEffects());
			else
				tasks.Add(dialogData.UpdateNoExitSceneEffects());

			await UniTask.WhenAll(tasks.ToArray());

			if (GameController.GameManagers.EndingsManager.CheckLose() != null)
			{
				// no need to save
				base.EndCommand(GameState.EndGame);
				return;
			}

			CheckWin();
		}

		private async void CheckWin ()
		{
			EndingData endingData = GameController.GameManagers.EndingsManager.CheckWin();

			if (endingData != null)
			{
				await endingData.UpdateEnterSceneEffects();
				GameController.GameManagers.EndingsManager.ShowWin(() => NextDialog());
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
			GameController.GameManagers.CharactersManager.UpdateCharactersPlayedTodaySaveData(new());
			GameController.GameManagers.CharactersManager.UpdateDialogStartedSaveData("");
			GameController.GameManagers.CharactersManager.UpdateTotalDialogPlayedTodaySaveData(0);
			await GameController.GameManagers.SaveManager.SaveData();
		}
	}
}