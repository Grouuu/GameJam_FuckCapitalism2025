using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DialogPanelUIButtonsLayout
{
	None,
	YesNo,
	Continue
}

public class DialogPanelUIData
{
	public string content;
	public CharacterData character;
	public DialogPanelUIButtonsLayout buttons;
	public ResultVarChange[] varChanges;
}

public class DialogPanelUI : MonoBehaviour
{
	public GameObject parent;
	public TextMeshProUGUI request;
	public CharacterAvatarUI avatar;
	public Transform resourcesParent;
	public Button yesButton;
	public Button noButton;
	public Button continueButton;

	[HideInInspector] public Action onceYesCallback;
	[HideInInspector] public Action onceNoCallback;
	[HideInInspector] public Action onceContinueCallback;

	public void Show (DialogPanelUIData panelContent, DialogPanelUIButtonsLayout buttons)
	{
		request.text = panelContent.content;
		avatar.SetAvatarSprite(panelContent.character.avatarFileName);
		avatar.SetAvatarName(panelContent.character.name);
		UpdateButtonsVisibility(buttons);

		AddResourcesValue(panelContent.varChanges);

		parent.SetActive(true);
	}

	public void Hide ()
	{
		parent.SetActive(false);

		request.text = "";

		onceYesCallback = null;
		onceNoCallback = null;
		onceContinueCallback = null;

		if (GameManager.Instance != null)
			GameManager.Instance.uiManager.RemoveResourceValues(resourcesParent);
	}

	/**
	 * Linked in the editor
	 */
	public void OnYesClick ()
	{
		OnClick(onceYesCallback);
	}

	/**
	 * Linked in the editor
	 */
	public void OnNoClick ()
	{
		OnClick(onceNoCallback);
	}

	/**
	 * Linked in the editor
	 */
	public void OnContinueClick ()
	{
		OnClick(onceContinueCallback);
	}

	private void OnEnable ()
	{
		Hide();
	}

	private void OnClick (Action callback)
	{
		onceYesCallback = null;
		onceNoCallback = null;
		onceContinueCallback = null;

		if (callback != null)
			callback();
	}

	private void UpdateButtonsVisibility (DialogPanelUIButtonsLayout layout)
	{
		yesButton.gameObject.SetActive(layout == DialogPanelUIButtonsLayout.YesNo);
		noButton.gameObject.SetActive(layout == DialogPanelUIButtonsLayout.YesNo);
		continueButton.gameObject.SetActive(layout == DialogPanelUIButtonsLayout.Continue);
	}

	private void AddResourcesValue (ResultVarChange[] resourceChanges)
	{
		if (resourceChanges == null || resourceChanges.Length == 0)
			return;

		foreach (ResultVarChange change in resourceChanges)
		{
			VarData varData = GameManager.Instance.varsManager.GetVarData(change.varId);

			if (varData != null && varData.varId != GameVarId.None && varData.type == GameVarType.UIVar)
				GameManager.Instance.uiManager.AddResourceValue(varData, change.currentValue, resourcesParent, Color.white);
		}
	}

}
