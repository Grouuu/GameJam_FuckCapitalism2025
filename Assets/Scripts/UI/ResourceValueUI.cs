using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceValueUI : MonoBehaviour
{
	public ResourceId id = ResourceId.None;
	public string title;

	[Space(10)]
	public Image imgBackground;
	public Image imgIcon;
	public TextMeshProUGUI txtValue;
	public TextMeshProUGUI txtTitle;

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
		txtValue.text = $"{value}";
	}

	public void SetVisibible (bool visible)
	{
		gameObject.SetActive(visible);
	}

	private void OnValidate ()
	{
		txtTitle.text = title;
	}

}
