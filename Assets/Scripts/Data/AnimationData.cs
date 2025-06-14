using System;

[Serializable]
public class AnimationData
{
	public GameAnimationKey key;
	public bool isPersistent;

	// runtime
	[NonSerialized] public bool isRunning = false;
	[NonSerialized] public bool isResumed = false;
}
