using System;
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

	private EventPanelUIData FormatPanelData (EndingData ending)
	{
		EventPanelUIData panelData = new();

		panelData.title = ending.title;
		panelData.content = ending.description;

		return panelData;
	}

}
