using UnityEngine;
using UnityEngine.InputSystem;

public class DebugManager : MonoBehaviour
{
	[SerializeField] private InputActionAsset _inputAction;

	//private InputAction _deleteGameLocalData;

	private void OnEnable ()
	{
		//_deleteGameLocalData = _inputAction.FindAction("Debug/DeleteGameLocalData");
		//_deleteGameLocalData.performed += DeleteGameLocalData;
	}

	private void OnDisable ()
	{
		//_deleteGameLocalData.performed -= DeleteGameLocalData;
	}

	//private void DeleteGameLocalData (InputAction.CallbackContext context)
	//{
	//	if (Application.isFocused)
	//	{
	//		Debug.LogWarning("Destroyed local data");
	//		PersistentManager.Instance.saveManager.DeleteSave();
	//	}
	//}

}
