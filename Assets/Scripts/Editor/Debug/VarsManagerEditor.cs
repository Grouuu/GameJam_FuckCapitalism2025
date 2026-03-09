using Grouuu;
using Grouuu.Data;
using Grouuu.Managers;
using Grouuu.Utils;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameController))]
public class VarsManagerEditor : Editor
{
	//private GameController targetComponent;

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector();

		if (!Application.isPlaying)
			return;

		EditorGUILayout.LabelField("Debug vars", EditorStyles.boldLabel);

		// update the list with values modified by the game
		serializedObject.Update();

		// show the current game values
		if (serializedObject.hasModifiedProperties || serializedObject.UpdateIfRequiredOrScript())
			Repaint();

		VarData[] varsData = GameController.GameManagers.VarsManager.GetAllVars();

		foreach (VarData varData in varsData)
		{
			EditorGUI.BeginChangeCheck();

			// check for a change from the inspector
			int newValue = EditorGUILayout.IntField($"{ParsingUtils.MapVars[varData.varId]}", varData.currentValue);

			// set the change to the current game value
			if (EditorGUI.EndChangeCheck())
				GameController.GameManagers.VarsManager.SetValueToVar(varData.varId, newValue, true);
		}
	}

	//private void OnEnable ()
	//{
	//	targetComponent = (GameController) target;
	//}
}