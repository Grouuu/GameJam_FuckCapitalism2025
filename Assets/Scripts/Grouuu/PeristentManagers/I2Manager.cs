using Grouuu.Utils;
using UnityEngine;

namespace Grouuu.PersistentManagers
{
	public class I2Manager : MonoBehaviour
	{
		public string CurrentLangage => LocalizationUtils.GetCurrentLanguage();

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
				if (language != CurrentLangage)
				{
					LocalizationUtils.SetCurrentLanguage(language);
					break;
				}
			}
		}
#endif

	}
}
