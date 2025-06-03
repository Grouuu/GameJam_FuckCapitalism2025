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
	public string headerFileName;
	public ResultVarChange[] varChanges;
}

public class EventPanelUI : MonoBehaviour
{
	public GameObject parent;
	public Image headerSprite;
	public TextMeshProUGUI titleUI;
	public TextMeshProUGUI contentUI;
	public Transform resourcesParent;
	public Button continueButton;

	[HideInInspector] public Action onceClickCallback;

	public void Show (EventPanelUIData panelContent)
	{
		titleUI.text = panelContent.title;
		contentUI.text = panelContent.content;

		SetHeader(panelContent.headerFileName);
		AddResourcesValue(panelContent.varChanges);

		parent.SetActive(true);
	}

	public void Hide ()
	{
		parent.SetActive(false);

		titleUI.text = "";
		contentUI.text = "";

		onceClickCallback = null;

		if (GameManager.Instance != null)
			GameManager.Instance.uiManager.RemoveResourceValues(resourcesParent);
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

	private void OnEnable ()
	{
		Hide();
	}

	private void SetHeader (string fileName)
	{
		Sprite sprite = Resources.Load<Sprite>($"EventSprites/{fileName}");

		if (sprite == null)
		{
			Debug.LogWarning($"No sprite found for {fileName}");
			return;
		}

		headerSprite.sprite = sprite;
	}

	private void AddResourcesValue (ResultVarChange[] resourceChanges)
	{
		if (resourceChanges == null || resourceChanges.Length == 0)
			return;

		foreach (ResultVarChange change in resourceChanges)
		{
			VarData varData = GameManager.Instance.varsManager.GetVarData(change.varId);

			if (varData != null && varData.varId != GameVarId.None)
				GameManager.Instance.uiManager.AddResourceValue(varData, change.currentValue, resourcesParent, Color.black);
		}
	}
}
