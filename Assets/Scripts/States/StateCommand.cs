using UnityEngine;

public delegate void OnStateEnd (GameState state = GameState.None);

public class StateCommand : MonoBehaviour
{
	public event OnStateEnd OnStateEnd;

	public virtual GameState state => GameState.None;

	public virtual void StartCommand (GameState previousState = GameState.None)
	{
		Debug.LogWarning($"StartCommand not implemented");
	}

	public virtual void EndCommand (GameState nextState = GameState.None)
	{
		OnStateEnd?.Invoke(nextState);
	}

}
