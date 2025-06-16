using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceIconUI : MonoBehaviour
{
	public Image iconSprite;
	public TextMeshProUGUI valueText;
	public TextMeshProUGUI tooltipText;

	private Sprite _sprite;

	public void SetColor(Color color)
	{
		iconSprite.color = color;
		valueText.color = color;
		tooltipText.color = color;
	}

	public void SetIcon (string fileName)
	{
		Sprite sprite = Resources.Load<Sprite>($"VarsSprites/{fileName}");

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

	public void SetTooltipName (string name)
	{
		tooltipText.text = name;
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
