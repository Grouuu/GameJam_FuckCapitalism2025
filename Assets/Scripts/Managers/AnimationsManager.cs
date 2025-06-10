using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public enum GameAnimationKey
{
	Intro,
}

public class GameAnimation : MonoBehaviour
{
	public GameAnimationKey animationKey;

	public virtual async Awaitable Play ()
	{
		await Task.CompletedTask;
	}
}

public class AnimationsManager : MonoBehaviour
{
	private GameAnimation[] _animations = { };

	public Awaitable PlayAnimation (GameAnimationKey animationKey)
	{
		GameAnimation animation = _animations.FirstOrDefault(entry => entry.animationKey == animationKey);

		if (animation != null)
			return animation.Play();

		return null;
	}

	private void OnEnable ()
	{
		_animations = GetComponents<GameAnimation>();
	}

}
