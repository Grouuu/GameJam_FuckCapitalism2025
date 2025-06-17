using System;
using TMPro;
using UnityEngine.UI;

[Serializable]
public class ResourceValueUI
{
	public GameVarId id = GameVarId.None;
	public TextMeshProUGUI textfield;
	public Slider slider;
	public TooltipUI tooltip;
	public WarningVarUI warning;
	public bool showMax;

	public void Update ()
	{
		if (tooltip != null)
			tooltip.varId = id;
	}

	public void SetValue (int value, int max)
	{
		if (textfield != null)
		{
			string text = $"{value}";

			if (showMax)
				text += $"/{max}";

			textfield.text = text;
		}

		if (slider != null)
		{
			slider.maxValue = max;
			slider.value = value;
		}
	}

	public void ShowWarning (bool isShow)
	{
		if (warning != null)
			warning.gameObject.SetActive(isShow);
	}

}
