using System;
using TMPro;
using UnityEngine.UI;

[Serializable]
public class ResourceValueUI
{
	public GameVarId id = GameVarId.None;
	public TextMeshProUGUI textfield;
	public Slider slider;

	public void SetValue (int value)
	{
		if (textfield != null)
			textfield.text = $"{value}";

		if (slider != null)
			slider.value = value;
	}

}
