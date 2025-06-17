using I2.Loc;
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

public static class LocalizationUtils
{
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
}
