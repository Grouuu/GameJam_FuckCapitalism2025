using Cysharp.Threading.Tasks;
using Grouuu.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Grouuu.Data;
using Grouuu.Constants;

namespace Grouuu.Managers
{
	public class SceneEffectsManager : MonoBehaviour
	{
		private SceneEffect[] _sceneEffects = { };
		private SceneEffectData[] _sceneEffectsData = { };

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
				SceneEffectData sceneEffectData = GetSceneEffectDataByName(sceneEffect.EffectName);

				if (sceneEffectData == null)
					continue;

				if (sceneEffectData.duration >= 0 && sceneEffectData.isRunning && sceneEffectData.isResumed)
					_ = sceneEffect.Play(true);

				sceneEffectData.isResumed = false;
			}
		}

		public async UniTask UpdateSceneEffects (EditSceneEffect sceneEffects)
		{
			List<UniTask> tasks = new();

			foreach (string sceneEffectName in sceneEffects.playSceneEffects)
				tasks.Add(PlaySceneEffect(sceneEffectName));

			foreach (string sceneEffectName in sceneEffects.stopSceneEffects)
				tasks.Add(StopSceneEffect(sceneEffectName));

			await UniTask.WhenAll(tasks.ToArray());
		}

		public async UniTask PlayStartDayEffects ()
		{
			SceneEffectData[] effectsData = _sceneEffectsData.Where(entry => entry.timing == -1 && entry.playRequirements.IsOK() && !entry.isRunning).ToArray();

			foreach (SceneEffectData effectData in effectsData)
				await PlaySceneEffect(effectData.name);
		}

		public async UniTask PlaySceneEffect (string sceneEffectName)
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

		public async UniTask StopSceneEffect (string sceneEffectName)
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
			return _sceneEffects.FirstOrDefault(entry => entry.EffectName == sceneEffectName);
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
				SceneEffectData sceneEffectData = GetSceneEffectDataByName(sceneEffect.EffectName);

				if (sceneEffectData == null)
				{
					Debug.LogWarning($"Missing effect data for {sceneEffect.EffectName}");
					continue;
				}

				// only save persistent scene effects
				if (sceneEffectData.duration >= 0)
					sceneEffectsState.Add((sceneEffectData.name, sceneEffectData.isRunning));
			}

			GameController.GameManagers.SaveManager.AddToSaveData(SaveItemKey.SceneEffects, sceneEffectsState);
		}

		public void ApplySave ()
		{
			List<(string, bool)> sceenEffectsState = GameController.GameManagers.SaveManager.GetSaveData<List<(string, bool)>>(SaveItemKey.SceneEffects);

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
}