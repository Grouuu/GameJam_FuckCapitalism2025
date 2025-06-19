using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class SceneEffectName
{
	public static string IntroResilienceShip = "IntroResilienceShip";
}

public class SceneEffect : MonoBehaviour
{
	public virtual string effectName => null;

	public virtual async Task Play (bool isResumed = false)
	{
		await Task.CompletedTask;
	}

	public virtual async Task Stop ()
	{
		await Task.CompletedTask;
	}
}

public class SceneEffectsManager : MonoBehaviour
{
	private SceneEffectData[] _sceneEffectsData = { };
	private SceneEffect[] _sceneEffects = { };

	public void InitSceneEffects (SceneEffectData[] sceneEffectsData)
	{
		if (sceneEffectsData == null)
		{
			Debug.LogError($"No scene effects to init");
			return;
		}

		Debug.Log($"Scene effects loaded (total: {sceneEffectsData.Length})");

		_sceneEffectsData = sceneEffectsData;
	}

	public void ResumeSceneEffects ()
	{
		foreach (SceneEffect sceneEffect in _sceneEffects)
		{
			SceneEffectData sceneEffectData = GetSceneEffectDataByName(sceneEffect.effectName);

			if (sceneEffectData == null)
				continue;

			if (sceneEffectData.duration >= 0 && sceneEffectData.isRunning && sceneEffectData.isResumed)
				_ = sceneEffect.Play(true);

			sceneEffectData.isResumed = false;
		}
	}

	public async Task UpdateSceneEffects (EditSceneEffect sceneEffects)
	{
		List<Task> tasks = new();

		foreach (string sceneEffectName in sceneEffects.playSceneEffects)
			tasks.Add(PlaySceneEffect(sceneEffectName));

		foreach (string sceneEffectName in sceneEffects.stopSceneEffects)
			tasks.Add(StopSceneEffect(sceneEffectName));

		await Task.WhenAll(tasks.ToArray());
	}

	public async Task PlayStartDayEffects ()
	{
		SceneEffectData[] effectsData = _sceneEffectsData.Where(entry => entry.timing == -1 && entry.playRequirements.IsOK() && !entry.isRunning).ToArray();

		foreach (SceneEffectData effectData in effectsData)
			await PlaySceneEffect(effectData.name);
	}

	public async Task PlaySceneEffect (string sceneEffectName)
	{
		SceneEffect sceneEffect = GetSceneEffectByName(sceneEffectName);
		SceneEffectData sceneEffectData = GetSceneEffectDataByName(sceneEffectName);

		if (sceneEffect == null || sceneEffectData == null)
		{
			Debug.LogWarning($"Missing effect or data for {sceneEffectName} : effect = {sceneEffect != null}, data = {sceneEffectData != null}");
			return;
		}

		if (!sceneEffectData.playRequirements.IsOK())
			return;

		sceneEffectData.isRunning = true;

		UpdateSaveData();

		await sceneEffect.Play();

		// if instant effect, stop it
		if (sceneEffectData.duration == -1)
			await StopSceneEffect(sceneEffectName);
	}

	public async Task StopSceneEffect (string sceneEffectName)
	{
		SceneEffect sceneEffect = GetSceneEffectByName(sceneEffectName);
		SceneEffectData sceneEffectData = GetSceneEffectDataByName(sceneEffectName);

		if (sceneEffect == null || sceneEffectData == null)
		{
			Debug.LogWarning($"Missing effect or data for {sceneEffectName} : effect = {sceneEffect != null}, data = {sceneEffectData != null}");
			return;
		}

		// do not prevent to stop instant effect
		if (sceneEffectData.duration != -1 && !sceneEffectData.stopRequirements.IsOK())
			return;

		sceneEffectData.isRunning = false;

		UpdateSaveData();

		await sceneEffect.Stop();
	}

	public SceneEffect GetSceneEffectByName (string sceneEffectName)
	{
		return _sceneEffects.FirstOrDefault(entry => entry.effectName == sceneEffectName);
	}

	public SceneEffectData GetSceneEffectDataByName (string sceneEffectName)
	{
		return _sceneEffectsData.FirstOrDefault(entry => entry.name == sceneEffectName);
	}

	public void UpdateSaveData ()
	{
		List<(string, bool)> sceneEffectsState = new();

		foreach (SceneEffect sceneEffect in _sceneEffects)
		{
			SceneEffectData sceneEffectData = GetSceneEffectDataByName(sceneEffect.effectName);

			if (sceneEffectData == null)
			{
				Debug.LogWarning($"Missing effect data for {sceneEffect.effectName}");
				continue;
			}

			// only save persistent scene effects
			if (sceneEffectData.duration >= 0)
				sceneEffectsState.Add((sceneEffectData.name, sceneEffectData.isRunning));
		}

		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.SceneEffects, sceneEffectsState);
	}

	public void ApplySave ()
	{
		List<(string, bool)> sceenEffectsState = GameManager.Instance.saveManager.GetSaveData<List<(string, bool)>>(SaveItemKey.SceneEffects);

		if (sceenEffectsState == null)
			return;

		foreach ((string name, bool isRunning) in sceenEffectsState)
		{
			SceneEffectData sceneEffectData = GetSceneEffectDataByName(name);

			if (sceneEffectData == null)
			{
				Debug.LogWarning($"Missing effect data for {name}");
				continue;
			}

			sceneEffectData.isRunning = isRunning;
			sceneEffectData.isResumed = true;
		}
	}

	private void OnEnable ()
	{
		_sceneEffects = GetComponents<SceneEffect>();
	}

}
