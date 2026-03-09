using Grouuu.Constants;
using Grouuu.Data;
using Grouuu.Enum;
using I2.Loc;
using TMPro;
using UnityEngine;

namespace Grouuu.UI
{
	public class TooltipUI : MonoBehaviour
	{
		public TextMeshProUGUI varNameText;
		public Localize varNameLocalize;
		public TextMeshProUGUI varValuesText;
		public LocalizationParamsManager varValuesParamsLocalize;
		public bool showValues = true;

		[HideInInspector] public GameVarId varId;

		private void OnEnable ()
		{
			UpdateText();
		}

		private void UpdateText ()
		{
			VarData varData = GameController.GameManagers.VarsManager.GetVarData(varId);

			if (varData != null)
			{
				varNameLocalize.SetTerm($"{LocCat.VarsNames}/{varData.name}");
				varValuesParamsLocalize.SetParameterValue(LocParam.TooltipCurrent, $"{varData.currentValue}");
				varValuesParamsLocalize.SetParameterValue(LocParam.TooltipMax, $"{varData.maxValue}");

				varValuesText.gameObject.SetActive(showValues);
			}
		}
	}
}