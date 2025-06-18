using UnityEngine;

public class I2Manager : MonoBehaviour
{
	public string currentLangage => LocalizationUtils.GetCurrentLanguage();

	public void SetLanguage (string language)
	{
		LocalizationUtils.SetCurrentLanguage(language);
	}

#if UNITY_EDITOR
	public void ToogleLanguage ()
	{
		string[] languages = LocalizationUtils.GetSupportedLanguages();

		foreach (string language in languages)
		{
			if (language != currentLangage)
			{
				LocalizationUtils.SetCurrentLanguage(language);
				break;
			}
		}
	}
#endif

}
