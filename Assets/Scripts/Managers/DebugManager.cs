using UnityEngine;

public class DebugManager : MonoBehaviour
{
	public DialogChoiceDebug dialogChoice;

	public void OnYesHoverIn () => dialogChoice.OnHoverIn(true);
	public void OnYesHoverOut () => dialogChoice.OnHoverOut(true);
	public void OnNoHoverIn () => dialogChoice.OnHoverIn(false);
	public void OnNoHoverOut () => dialogChoice.OnHoverOut(false);

	private void Start ()
	{
		dialogChoice.Setup();

#if !UNITY_EDITOR
		dialogChoice.enabled = false;
#endif
	}

}
