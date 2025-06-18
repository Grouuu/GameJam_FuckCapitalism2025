using TMPro;
using UnityEngine;

public class LanguageChangeUI : MonoBehaviour
{
	public TMP_Dropdown languageDropdown;

	/**
	 * Linked in the editor
	 */
	public void OnLanguageChange ()
	{
		string i2Name = LocalizationUtils.GetI2LanguageByDisplayName(languageDropdown.options[languageDropdown.value].text);
		PersistentManager.Instance.localizationManager.SetLanguage(i2Name);
	}

	private void Start ()
	{
		string currentLanguage = PersistentManager.Instance.localizationManager.currentLangage;
		string displayName = LocalizationUtils.GetDisplayLanguageByI2Name(currentLanguage);
		int index = languageDropdown.options.FindIndex(entry => entry.text == displayName);
		languageDropdown.value = index;
	}
}
