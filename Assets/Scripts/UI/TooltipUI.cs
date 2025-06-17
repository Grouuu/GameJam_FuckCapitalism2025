using I2.Loc;
using System;
using TMPro;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
	public TextMeshProUGUI text;
	public Localize localize;

	[HideInInspector] public GameVarId varId;

	private void OnEnable ()
	{
		UpdateText();
	}

	private void UpdateText ()
	{
		localize.SetTerm($"{LocCat.VarsNames}/{GameManager.Instance.varsManager.GetVarData(varId).name}");
	}

}
