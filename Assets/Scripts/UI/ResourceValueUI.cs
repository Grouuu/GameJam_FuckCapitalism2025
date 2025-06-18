using System;
using TMPro;
using UnityEngine.UI;

[Serializable]
public class ResourceValueUI
{
	public GameVarId id = GameVarId.None;
	public TextMeshProUGUI textfield;
	public Slider slider;
	public SegmentedProgressBarUI segmentedBar;
	public TooltipUI tooltip;
	public WarningVarUI lowWarning;
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

		if (segmentedBar != null)
		{
			segmentedBar.SetMax(max);
			segmentedBar.SetValue(value);
		}
	}

	public void ShowWarning (bool isShow)
	{
		if (lowWarning != null)
			lowWarning.gameObject.SetActive(isShow);
	}

}
