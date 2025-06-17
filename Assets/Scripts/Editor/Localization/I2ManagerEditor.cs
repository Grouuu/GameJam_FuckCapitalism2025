using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(I2Manager))]
public class I2ManagerEditor : Editor
{
	I2Manager manager => (I2Manager) target;

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("Toogle language"))
		{
			manager.ToogleLanguage();
			SceneView.RepaintAll();
		}
	}
}
