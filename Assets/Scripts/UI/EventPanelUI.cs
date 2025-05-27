using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum EventPanelButtonId
{
	None,
	Continue,
}

public class EventPanelUIData
{
	public string title;
	public string content;
}

public class EventPanelUI : MonoBehaviour
{
	public GameObject parent;
	public TextMeshProUGUI titleUI;
	public TextMeshProUGUI contentUI;
	public Button continueButton;

	[HideInInspector] public Action onceClickCallback;

	public void Show (EventPanelUIData panelContent)
	{
		titleUI.text = panelContent.title;
		contentUI.text = panelContent.content;

		parent.SetActive(true);
	}

	public void Hide ()
	{
		titleUI.text = "";
		contentUI.text = "";

		onceClickCallback = null;

		parent.SetActive(false);
	}

	/**
	 * Linked in the editor
	 */
	public void OnContinueClick ()
	{
		OnClick(onceClickCallback);
	}

	private void OnClick (Action callback)
	{
		onceClickCallback = null;

		if (callback != null)
			callback();
	}

	private void Awake ()
	{
		Hide();
	}
}
