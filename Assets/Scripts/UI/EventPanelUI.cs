using I2.Loc;
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
	public string titleTermKey;
	public string titleTermCat;
	public string contentTermKey;
	public string contentTermCat;
	public string headerFileName;
	public ResultVarChange[] varChanges;
}

public class EventPanelUI : MonoBehaviour
{
	public static readonly string FOLDER_SPRITES = "EventSprites/";

	public GameObject parent;
	public Image headerSprite;
	public TextMeshProUGUI titleUI;
	public TextMeshProUGUI contentUI;
	public Transform resourcesParent;
	public Button continueButton;
	public Localize titleLocalize;
	public Localize contentLocalize;

	[HideInInspector] public Action onceClickCallback;

	public void Show (EventPanelUIData panelContent)
	{
		titleLocalize.SetTerm($"{panelContent.titleTermCat}/{panelContent.titleTermKey}");
		contentLocalize.SetTerm($"{panelContent.contentTermCat}/{panelContent.contentTermKey}");

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
		Sprite sprite = Resources.Load<Sprite>($"{UIManager.PATH_SPRITES}{FOLDER_SPRITES}{fileName}");

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

			if (varData != null && varData.varId != GameVarId.None && varData.type == GameVarType.UIVar)
				GameManager.Instance.uiManager.AddResourceValue(varData, change.currentValue, resourcesParent, Color.black);
		}
	}
}
