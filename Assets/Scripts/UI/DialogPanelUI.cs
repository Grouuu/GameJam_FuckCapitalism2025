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
}

public class DialogPanelUI : MonoBehaviour
{
	public GameObject parent;
	public TextMeshProUGUI request;
	public CharacterAvatarUI avatar;
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

		parent.SetActive(true);
	}

	public void Hide ()
	{
		request.text = "";

		parent.SetActive(false);
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

	private void Awake ()
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

}
