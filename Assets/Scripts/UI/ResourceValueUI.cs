using System;
using TMPro;
using UnityEngine.UI;

[Serializable]
public class ResourceValueUI
{
	public GameVarId id = GameVarId.None;
	public TextMeshProUGUI textfield;
	public Slider slider;
	public WarningVarUI warning;

	public void SetValue (int value)
	{
		if (textfield != null)
			textfield.text = $"{value}";

		if (slider != null)
			slider.value = value;
	}

	public void ShowWarning (bool isShow)
	{
		if (warning != null)
			warning.gameObject.SetActive(isShow);
	}

}
