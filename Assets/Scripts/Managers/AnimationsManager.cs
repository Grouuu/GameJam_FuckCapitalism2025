using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public enum GameAnimationKey
{
	IntroResilience,
}

public class GameAnimation : MonoBehaviour
{
	public AnimationData data;

	public virtual async Awaitable Play (bool isResumed = false)
	{
		await Task.CompletedTask;
	}

	public virtual async Awaitable Stop ()
	{
		await Task.CompletedTask;
	}
}

public class AnimationsManager : MonoBehaviour
{
	private GameAnimation[] _animations = { };

	public async Awaitable PlayAnimation (GameAnimationKey key)
	{
		GameAnimation animation = GetAnimationByKey(key);

		if (animation == null)
			return;

		animation.data.isRunning = true;

		UpdateSaveData();

		await animation.Play();

		if (!animation.data.isPersistent)
			await StopAnimation(key);
	}

	public async Awaitable StopAnimation (GameAnimationKey key)
	{
		GameAnimation animation = GetAnimationByKey(key);

		if (animation == null)
			return;

		animation.data.isRunning = false;

		UpdateSaveData();

		await animation.Stop();
	}

	public GameAnimation GetAnimationByKey (GameAnimationKey key)
	{
		return _animations.FirstOrDefault(entry => entry.data.key == key);
	}

	public void ResumeAnimations ()
	{
		foreach (GameAnimation animation in _animations)
		{
			if (animation != null && animation.data.isPersistent && animation.data.isRunning && animation.data.isResumed)
				_ = animation.Play(true);

			animation.data.isResumed = false;
		}
	}

	public void UpdateSaveData ()
	{
		List<(GameAnimationKey, bool)> animationsState = new();

		foreach (GameAnimation animation in _animations)
		{
			// only save persistent animations
			if (animation.data.isPersistent)
				animationsState.Add((animation.data.key, animation.data.isRunning));
		}

		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.AnimationsState, animationsState);
	}

	public void ApplySave ()
	{
		List<(GameAnimationKey, bool)> animationsState = GameManager.Instance.saveManager.GetSaveData<List<(GameAnimationKey, bool)>>(SaveItemKey.AnimationsState);

		if (animationsState == null)
			return;

		foreach ((GameAnimationKey key, bool isRunning) in animationsState)
		{
			GameAnimation animation = GetAnimationByKey(key);

			if (animation == null)
			{
				Debug.LogWarning($"Missing animation with key: {key}");
				continue;
			}

			animation.data.isRunning = isRunning;
			animation.data.isResumed = true;
		}
	}

	private void OnEnable ()
	{
		_animations = GetComponents<GameAnimation>();
	}

}
