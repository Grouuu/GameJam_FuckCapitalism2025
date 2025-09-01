using System;
using UnityEngine;

[Serializable]
public class DialogChoiceDebug
{
	[SerializeField] public bool enabled = false;
	[SerializeField] public Transform yesDebugContainer;
	[SerializeField] public Transform noDebugContainer;

	public void Setup ()
	{
		yesDebugContainer.gameObject.SetActive(false);
		noDebugContainer.gameObject.SetActive(false);
	}

	public void OnHoverIn (bool isYes)
	{
		SetVisibility(true, isYes);
	}

	public void OnHoverOut (bool isYes)
	{
		SetVisibility(false, isYes);
	}

	public void AddDialogResourcesInfluence (DialogData dialogData)
	{
		foreach (ResultVarChange change in dialogData.yesResult.varChanges)
		{
			VarData varData = GameManager.Instance.varsManager.GetVarData(change.varId);
			GameManager.Instance.uiManager.AddResourceValue(varData, change.currentValue, yesDebugContainer, Color.white, Color.black);
		}

		foreach (ResultVarChange change in dialogData.noResult.varChanges)
		{
			VarData varData = GameManager.Instance.varsManager.GetVarData(change.varId);
			GameManager.Instance.uiManager.AddResourceValue(varData, change.currentValue, noDebugContainer, Color.white, Color.black);
		}
	}

	public void RemoveDialogResourcesInfluence ()
	{
		GameManager.Instance.uiManager.RemoveResourceValues(yesDebugContainer);
		GameManager.Instance.uiManager.RemoveResourceValues(noDebugContainer);
	}

	private void SetVisibility (bool isShow, bool isYes)
	{
		if (!enabled)
			return;

		GameObject container = isYes ? yesDebugContainer.gameObject : noDebugContainer.gameObject;
		container.SetActive(isShow);
	}

}
