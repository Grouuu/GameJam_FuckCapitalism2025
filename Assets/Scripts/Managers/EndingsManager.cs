using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndingsManager : MonoBehaviour
{
	private EndingData[] _endings;

	public void InitEndings (EndingData[] endings)
	{
		if (endings == null)
		{
			Debug.LogError($"No endings to init");
			return;
		}

		Debug.Log($"Endings loaded (total: {endings.Length})");

		_endings = endings;
	}

	public bool CheckWin ()
	{
		foreach (EndingData ending in _endings)
		{
			if (!ending.isWinEnding || ending.isUsed)
				continue;

			if (ending.IsRespectRequirements())
				return true;
		}

		return false;
	}

	public bool CheckLose ()
	{
		foreach (EndingData ending in _endings)
		{
			if (ending.isWinEnding)
				continue;

			if (ending.IsRespectRequirements())
				return true;
		}

		return false;
	}

	public void ShowWin (Action onContinue)
	{
		EndingData ending = GetEnding(true);

		if (ending == null)
		{
			Debug.LogError("No win ending detected");
			onContinue();
			return;
		}

		ending.isUsed = true;

		if (ending.result != null)
			ending.result.ApplyResult();

		UpdateSaveData();

		EventPanelUIData panelData = FormatPanelData(ending);
		GameManager.Instance.uiManager.ShowEventPanel(panelData, onContinue);
	}

	public void ShowLose (Action onContinue)
	{
		EndingData ending = GetEnding(false);

		if (ending == null)
		{
			Debug.LogError("No lose ending detected");
			onContinue();
			return;
		}

		EventPanelUIData panelData = FormatPanelData(ending);
		GameManager.Instance.uiManager.ShowEventPanel(panelData, onContinue);
	}

	public void UpdateSaveData ()
	{
		List<string> endingsUsed = new();

		foreach (EndingData endingData in _endings)
		{
			if (endingData.isUsed)
				endingsUsed.Add(endingData.name);
		}

		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.EndingsUsed, endingsUsed);
	}

	public void ApplySave ()
	{
		List<string> endingsUsed = GameManager.Instance.saveManager.GetSaveData<List<string>>(SaveItemKey.DialogsUsed);

		if (endingsUsed != null)
		{
			foreach (string endingName in endingsUsed)
			{
				EndingData endingData = GetEndingByName(endingName);

				if (endingData == null)
					continue;

				endingData.isUsed = true;
			}
		}
	}

	private EndingData GetEnding (bool isWin)
	{
		foreach (EndingData ending in _endings)
		{
			if (ending.isWinEnding != isWin)
				continue;

			if (ending.IsRespectRequirements())
				return ending;
		}

		return null;
	}

	private EndingData GetEndingByName (string endingName)
	{
		foreach (EndingData endingData in _endings)
		{
			EndingData matchEnding = _endings.FirstOrDefault(entry => entry.name == endingName);

			if (matchEnding != null)
				return matchEnding;
		}

		return null;
	}

	private EventPanelUIData FormatPanelData (EndingData ending)
	{
		EventPanelUIData panelData = new();

		panelData.title = ending.title;
		panelData.content = ending.description;

		return panelData;
	}

}
