using Grouuu.Data;
using Grouuu.Managers;
using System;
using UnityEngine;

namespace Grouuu.GameDebug
{
	[Serializable]
	public class DialogChoiceDebug
	{
		[SerializeField] public bool Enabled = false;
		[SerializeField] public Transform YesDebugContainer;
		[SerializeField] public Transform NoDebugContainer;

		public void Setup ()
		{
			YesDebugContainer.gameObject.SetActive(false);
			NoDebugContainer.gameObject.SetActive(false);
		}

		public void Clear ()
		{
			SetVisibility(false, true);
			SetVisibility(false, false);
			RemoveDialogResourcesInfluence();
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
				VarData varData = GameController.GameManagers.VarsManager.GetVarData(change.varId);
				GameController.GameManagers.UiManager.AddResourceValue(varData, change.currentValue,YesDebugContainer, Color.white, Color.black);
			}

			foreach (ResultVarChange change in dialogData.noResult.varChanges)
			{
				VarData varData = GameController.GameManagers.VarsManager.GetVarData(change.varId);
				GameController.GameManagers.UiManager.AddResourceValue(varData, change.currentValue, NoDebugContainer, Color.white, Color.black);
			}
		}

		public void RemoveDialogResourcesInfluence ()
		{
			GameController.GameManagers.UiManager.RemoveResourceValues(YesDebugContainer);
			GameController.GameManagers.UiManager.RemoveResourceValues(NoDebugContainer);
		}

		public void SetVisibility (bool isShow, bool isYes)
		{
			if (!Enabled)
				return;

			GameObject container = isYes ? YesDebugContainer?.gameObject : NoDebugContainer?.gameObject;
			container?.SetActive(isShow);
		}
	}
}
