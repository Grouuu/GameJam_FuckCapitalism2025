using System;

public class EditAnimations
{
	public string[] startAnimations;
	public string[] stopAnimations;
}

[Serializable]
public class AnimationData
{
	public string name;
	public bool isPersistent;

	// runtime
	[NonSerialized] public bool isRunning = false;
	[NonSerialized] public bool isResumed = false;
}
