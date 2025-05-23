using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ReportPanelButtonId
{
	None,
	Yes,
	No,
	Continue,
}

public class ReportPanelUIData
{
	public string title;
	public string content;
}

public class ReportPanelUI : MonoBehaviour
{
	public GameObject parent;
	public TextMeshProUGUI titleUI;
	public TextMeshProUGUI contentUI;
	public Button continueButton;

	[HideInInspector] public Action onceClickCallback;

	public void Show (ReportPanelUIData panelContent)
	{
		titleUI.text = panelContent.title;
		contentUI.text = panelContent.content;

		parent.SetActive(true);
	}

	public void Hide ()
	{
		titleUI.text = "";
		contentUI.text = "";

		parent.SetActive(false);
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
