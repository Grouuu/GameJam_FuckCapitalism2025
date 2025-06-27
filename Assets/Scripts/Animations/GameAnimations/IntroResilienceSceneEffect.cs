using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Splines;
using static UnityEngine.Splines.SplineAnimate;

public class IntroResilienceSceneEffect : SceneEffect
{
	public SplineAnimate animate;
	public SplineContainer splineIntro;
	public SplineContainer splineLoop;
	public int delayIntro;
	public EasingMode introEase;
	public int introSpeed;
	public EasingMode loopEase;
	public int loopSpeed;

	public override string effectName => SceneEffectName.IntroResilienceShip;

	public override async UniTask Play (bool isResumed)
	{
		// do not resume it automatically
		if (isResumed)
			return;

		bool playIntro = !SaveManager.IsRunStarted;

		if (playIntro)
		{
			PlayIntro();
			await UniTask.Delay(Mathf.RoundToInt(delayIntro * 1000));
		}
		else
		{
			PlayLoop();
		}
	}

	private void OnDisable ()
	{
		animate.Completed -= PlayLoop;
	}

	private void PlayIntro ()
	{
		animate.Container = splineIntro;

		animate.Easing = introEase;
		animate.MaxSpeed = introSpeed;
		animate.Loop = LoopMode.Once;
		animate.Play();

		animate.Completed += PlayLoop;
	}

	private void PlayLoop ()
	{
		animate.Completed -= PlayLoop;

		animate.Container = splineLoop;

		animate.Easing = loopEase;
		animate.MaxSpeed = loopSpeed;
		animate.Loop = LoopMode.Loop;
		animate.Restart(true);
	}

}
