using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceIconUI : MonoBehaviour
{
	public static readonly string FOLDER_SPRITES = "VarsSprites/";

	public Image iconSprite;
	public TextMeshProUGUI valueText;
	public TextMeshProUGUI tooltipText;
	public Localize tooltipLocalize;

	private Sprite _sprite;

	public void SetColor(Color color)
	{
		iconSprite.color = color;
		valueText.color = color;
		tooltipText.color = color;
	}

	public void SetIcon (string fileName)
	{
		Sprite sprite = Resources.Load<Sprite>($"{UIManager.PATH_SPRITES}{FOLDER_SPRITES}{fileName}");

		if (sprite == null)
		{
			Debug.Log($"No sprite found for {fileName}");
			return;
		}

		_sprite = sprite;
		iconSprite.sprite = _sprite;
	}

	public void SetValue (int value)
	{
		string modifier = value > 0 ? "+" : "";
		valueText.text = $"{modifier}{value}";
	}

	public void SetTooltipName (string varName)
	{
		tooltipLocalize.SetTerm($"{LocCat.VarsNames}/{varName}");
	}

	private void OnDisable ()
	{
		if (_sprite != null)
		{
			Resources.UnloadAsset(_sprite);
			_sprite = null;
		}
	}
}
