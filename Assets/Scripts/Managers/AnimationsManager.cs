using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class GameAnimationName
{
	public static string IntroResilienceShip = "IntroResilienceShip";
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

	public void UpdateAnimations (EditAnimations animations)
	{
		foreach (string animationName in animations.startAnimations)
			_ = PlayAnimation(animationName);

		foreach (string animationName in animations.stopAnimations)
			_ = StopAnimation(animationName);
	}

	public async Awaitable PlayAnimation (string animationName)
	{
		GameAnimation animation = GetAnimationByName(animationName);

		if (animation == null)
			return;

		animation.data.isRunning = true;

		UpdateSaveData();

		await animation.Play();

		if (!animation.data.isPersistent)
			await StopAnimation(animationName);
	}

	public async Awaitable StopAnimation (string animationName)
	{
		GameAnimation animation = GetAnimationByName(animationName);

		if (animation == null)
			return;

		animation.data.isRunning = false;

		UpdateSaveData();

		await animation.Stop();
	}

	public GameAnimation GetAnimationByName (string animationName)
	{
		return _animations.FirstOrDefault(entry => entry.data.name == animationName);
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
		List<(string, bool)> animationsState = new();

		foreach (GameAnimation animation in _animations)
		{
			// only save persistent animations
			if (animation.data.isPersistent)
				animationsState.Add((animation.data.name, animation.data.isRunning));
		}

		GameManager.Instance.saveManager.AddToSaveData(SaveItemKey.AnimationsState, animationsState);
	}

	public void ApplySave ()
	{
		List<(string, bool)> animationsState = GameManager.Instance.saveManager.GetSaveData<List<(string, bool)>>(SaveItemKey.AnimationsState);

		if (animationsState == null)
			return;

		foreach ((string name, bool isRunning) in animationsState)
		{
			GameAnimation animation = GetAnimationByName(name);

			if (animation == null)
			{
				Debug.LogWarning($"Missing animation with name: {name}");
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
