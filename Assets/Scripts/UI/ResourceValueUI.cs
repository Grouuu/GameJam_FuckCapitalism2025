using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceValueUI : MonoBehaviour
{
	public Image imgBackground;
	public Image imgIcon;
	public TextMeshProUGUI txtValue;
	public TextMeshProUGUI txtTitle;
	public string title;

	public void SetBackground (string sourceImagePath)
	{
		SetBackground(Resources.Load<Sprite>(sourceImagePath));
	}

	public void SetBackground (Sprite sourceImage)
	{
		imgBackground.sprite = sourceImage;
	}

	public void SetIcon (string sourceImagePath)
	{
		SetIcon(Resources.Load<Sprite>(sourceImagePath));
	}

	public void SetIcon (Sprite sourceImage)
	{
		imgIcon.sprite = sourceImage;
	}

	public void SetValue (int value)
	{
		SetValue($"{value}");
	}

	public void SetValue (string value)
	{
		txtValue.text = value;
	}

	private void OnValidate ()
	{
		txtTitle.text = title;
	}

}
