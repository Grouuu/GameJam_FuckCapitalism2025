using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : Editor
{
	private SaveManager saveManager => (SaveManager) target;

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("Open PlayerPrefs folder"))
		{
			OpenPlayerPrefsFolder();
		}

		if (GUILayout.Button("Delete PlayerPrefs"))
		{
			DeleteSave();
		}
	}

	private void OpenPlayerPrefsFolder ()
	{
		string folderPath = saveManager.savePath;

		try
		{
			EditorUtility.RevealInFinder(folderPath);
			Debug.Log($"Opened folder: {folderPath}");
		}
		catch (Exception e)
		{
			Debug.LogError($"Failed to open folder at {folderPath}: {e.Message}");
		}
	}

	private void DeleteSave ()
	{
		saveManager.DeleteSave();
	}

}
