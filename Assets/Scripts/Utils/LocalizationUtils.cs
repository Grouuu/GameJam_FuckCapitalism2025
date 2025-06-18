using I2.Loc;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LocCat
{
	public static string UI = "UI";
	public static string VarsNames = "VarsNames";
	public static string CharactersNames = "CharactersNames";
	public static string BuildingsNames = "BuildingsNames";
	public static string BuildingsDescriptions = "BuildingsDescriptions";
	public static string DialogsRequests = "DialogsRequests";
	public static string DialogsYes = "DialogsYes";
	public static string DialogsNo = "DialogsNo";
	public static string EventsTitles = "EventsTitles";
	public static string EventsDescriptions = "EventsDescriptions";
	public static string EndingsTitles = "EndingsTitles";
	public static string EndingsDescriptions = "EndingsDescriptions";
}

public static class LocValue
{
	public static string DailyFoodValue = "VALUE";
	public static string DailyQoLValue = "VALUE";
}

public static class LocalizationUtils
{
	public static Dictionary<string, string> MapDisplayToI2LanguageName = new()
	{
		{ "EN", "english" },
		{ "FR", "french" },
	};

	public static string GetI2LanguageByDisplayName (string displayName)
	{
		return MapDisplayToI2LanguageName.FirstOrDefault(entry => entry.Key == displayName).Value;
	}

	public static string GetDisplayLanguageByI2Name (string i2Name)
	{
		return MapDisplayToI2LanguageName.FirstOrDefault(entry => entry.Value == i2Name).Key;
	}

	public static string GetText (string key, string category)
	{
		string text = LocalizationManager.GetTranslation($"{category}/{key}");

		if (string.IsNullOrEmpty(text))
			Debug.LogWarning($"Missing translation for {category}/{key}");

		return text;
	}

	public static string ReplaceValue (this string text, string key, string value)
	{
		return text.Replace("{[" + key + "]}", $"{value}");
	}

	public static string GetCurrentLanguage ()
	{
		return LocalizationManager.CurrentLanguage;
	}

	public static void SetCurrentLanguage (string language)
	{
		LocalizationManager.SetLanguageAndCode(language, LocalizationManager.GetLanguageCode(language));
	}

	public static string[] GetSupportedLanguages ()
	{
		return LocalizationManager.GetAllLanguages().ToArray();
	}
}
