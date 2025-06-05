using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VarsManager))]
public class VarsManagerEditor : Editor
{
	private VarsManager targetComponent;

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector();

		if (!Application.isPlaying)
			return;

		EditorGUILayout.LabelField("Debug vars", EditorStyles.boldLabel);

		VarData[] varsData = targetComponent.GetAllVars();

		foreach (VarData varData in varsData)
		{
			EditorGUI.BeginChangeCheck();

			int newValue = EditorGUILayout.IntField($"{ParsingUtils.MapVars[varData.varId]}", varData.currentValue);

			if (EditorGUI.EndChangeCheck())
				targetComponent.SetValueToVar(varData.varId, newValue, true);
		}
	}

	private void OnEnable ()
	{
		targetComponent = (VarsManager) target;
	}

}
