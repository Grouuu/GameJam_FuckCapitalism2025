using UnityEngine;

public delegate void OnStateEnd ();

public class StateCommand : MonoBehaviour
{
	public event OnStateEnd OnStateEnd;

	[HideInInspector] public GameState state = GameState.None;

	public virtual void StartCommand ()
	{
		Debug.LogWarning($"StartCommand not implemented");
	}

	protected void EndCommand ()
	{
		OnStateEnd?.Invoke();
	}

}
