using Grouuu.Data;
using Grouuu.Enum;
using Grouuu.UI;
using System;
using UnityEngine;

namespace Grouuu.Managers
{
	public class UIManager : MonoBehaviour
	{
		public string SpritesFolderPath = "Sprites/";

		public ResourceIconUI PrefabResourceIcon;
		public ResourceValueSeparatorUI PrefabResourceSeparator;
		public ResourceValueUI[] ResourceValuesUI;

		public DialogPanelUI DialogPanel;
		public EventPanelUI EventPanel;
		public DailyReportPanelUI ReportPanel;
		public OptionsPanelUI OptionsPanel;

		public void SetResourceValue (GameVarId id, int value, int max)
		{
			if (id == GameVarId.None)
				return;

			GetUIResourceComponent(id)?.SetValue(value, max);
		}

		public void ShowEventPanel (EventPanelUIData panelData, Action onContinue)
		{
			EventPanel.onceClickCallback = () => {
				HideEventPanel();
				onContinue?.Invoke();
			};

			EventPanel.Show(panelData);
		}

		public void ShowReportPanel (DailyReportPanelUIData panelData, Action onContinue)
		{
			ReportPanel.onceClickCallback = () => {
				HideReportPanel();
				onContinue?.Invoke();
			};

			ReportPanel.Show(panelData);
		}

		public void ShowDialogPanel (DialogPanelUIData contentData, Action onContinue)
		{
			ShowDialogPanel(contentData, onContinue, null);
		}

		public void ShowDialogPanel (DialogPanelUIData contentData, Action onYes, Action onNo)
		{
			bool isYesNoContent = onNo != null;

			if (isYesNoContent)
			{
				// request layout
				DialogPanel.onceYesCallback = () => {
					HideDialogPanel();
					if (onYes != null)
						onYes();
				};

				DialogPanel.onceNoCallback = () => {
					HideDialogPanel();
					if (onNo != null)
						onNo();
				};
			}
			else
			{
				// response layout
				DialogPanel.onceContinueCallback = () => {
					HideDialogPanel();
					if (onYes != null)
						onYes();
				};
			}

			DialogPanel.Show(contentData, isYesNoContent ? DialogPanelUIButtonsLayout.YesNo : DialogPanelUIButtonsLayout.Continue);
		}

		public void HideEventPanel ()
		{
			EventPanel.Hide();
		}

		public void HideReportPanel ()
		{
			ReportPanel.Hide();
		}

		public void HideDialogPanel ()
		{
			DialogPanel.Hide();
		}

		public void AddResourceValue (VarData varData, int diff, Transform parent, Color color, Color secondColor)
		{
			// TODO use pool
			ResourceIconUI resource = GameObject.Instantiate(PrefabResourceIcon, parent);

			resource.SetColor(color, color, secondColor, color);
			resource.SetIcon(varData.iconFileName);
			resource.SetValue(diff);
			resource.SetTooltipName(varData.name);
		}

		public void AddResourceSeparator (Transform parent)
		{
			// TODO use pool
			GameObject.Instantiate(PrefabResourceSeparator, parent);
		}

		public void RemoveResourceValues (Transform parent)
		{
			ResourceIconUI[] resources = parent.GetComponentsInChildren<ResourceIconUI>();

			foreach (ResourceIconUI resource in resources)
			{
				GameObject.Destroy(resource.gameObject);
			}

			ResourceValueSeparatorUI[] separators = parent.GetComponentsInChildren<ResourceValueSeparatorUI>();

			foreach (ResourceValueSeparatorUI separator in separators)
			{
				GameObject.Destroy(separator.gameObject);
			}
		}

		public void ShowResourceLowWarning (GameVarId varId, bool isLow)
		{
			GetUIResourceComponent(varId)?.ShowLowWarning(isLow);
		}

		public void ShowResourceMaxWarning (GameVarId varId, bool isMax)
		{
			GetUIResourceComponent(varId)?.ShowMaxWarning(isMax);
		}

		private void OnEnable ()
		{
			InitResourceValueUI();
		}

		private void InitResourceValueUI ()
		{
			foreach (ResourceValueUI valueUI in ResourceValuesUI)
				valueUI.Update();
		}

		private ResourceValueUI GetUIResourceComponent (GameVarId resourceId)
		{
			return Array.Find(ResourceValuesUI, component => component.id == resourceId);
		}
	}
}