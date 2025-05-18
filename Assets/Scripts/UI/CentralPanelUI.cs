using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PanelButtonId
{
	None,
	Yes,
	No,
	Continue,
}

public class CentralPanelUIData
{
	public string title;
	public string content;
}

public class CentralPanelUI : MonoBehaviour
{
	public TextMeshProUGUI titleUI;
	public TextMeshProUGUI contentUI;
	public Button continueButton;

	[HideInInspector] public Action onceClickCallback;

	public void Show (CentralPanelUIData panelContent)
	{
		titleUI.text = panelContent.title;
		contentUI.text = panelContent.content;

		gameObject.SetActive(true);
	}

	public void Hide ()
	{
		titleUI.text = "";
		contentUI.text = "";

		gameObject.SetActive(false);
	}

	/**
	 * Linked in the editor
	 */
	public void OnContinueClick ()
	{
		if (onceClickCallback != null)
			onceClickCallback();

		onceClickCallback = null;
	}

	private void Awake ()
	{
		Hide();
	}
}
