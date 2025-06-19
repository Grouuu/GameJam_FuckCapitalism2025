using System;

public class EditSceneEffect
{
	public string[] playSceneEffects = { };
	public string[] stopSceneEffects = { };
}

[Serializable]
public class SceneEffectData
{
	public string id;
	public string name;
	public int timing;
	public int duration;
	public RequirementData playRequirements;
	public RequirementData stopRequirements;
	public ResultData playResult;
	public ResultData stopResult;
	public EditSceneEffect enterSceneEffets;
	public EditSceneEffect exitSceneEffets;

	// runtime values
	[NonSerialized] public bool isRunning = false;
	[NonSerialized] public bool isResumed = false;
}
