using Grouuu.PersistentManagers;
using Grouuu.Utils;
using TMPro;
using UnityEngine;

namespace Grouuu.UI
{
	public class LanguageChangeUI : MonoBehaviour
	{
		public TMP_Dropdown languageDropdown;

		/**
		 * Linked in the editor
		 */
		public void OnLanguageChange ()
		{
			string i2Name = LocalizationUtils.GetI2LanguageByDisplayName(languageDropdown.options[languageDropdown.value].text);
			PersistentController.Instance.LocalizationManager.SetLanguage(i2Name);
		}

		private void Start ()
		{
			string currentLanguage = PersistentController.Instance.LocalizationManager.CurrentLangage;
			string displayName = LocalizationUtils.GetDisplayLanguageByI2Name(currentLanguage);
			int index = languageDropdown.options.FindIndex(entry => entry.text == displayName);
			languageDropdown.value = index;
		}
	}
}