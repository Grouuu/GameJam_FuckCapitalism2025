using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReportResourceValueUI : MonoBehaviour
{
	public Image iconSprite;
	public TextMeshProUGUI valueText;

	private Sprite _sprite;

	public void SetColor(Color color)
	{
		iconSprite.color = color;
		valueText.color = color;
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

	public void SetValue (int diff)
	{
		string modifier = diff > 0 ? "+" : "";
		valueText.text = $"{modifier}{diff}";
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
