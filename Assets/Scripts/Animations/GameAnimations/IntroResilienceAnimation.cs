using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Splines;
using static UnityEngine.Splines.SplineAnimate;

public class IntroResilienceAnimation : GameAnimation
{
	public SplineAnimate animate;
	public SplineContainer splineIntro;
	public SplineContainer splineLoop;
	public int delayIntro;
	public EasingMode introEase;
	public int introSpeed;
	public EasingMode loopEase;
	public int loopSpeed;

	public override async Awaitable Play ()
	{
		bool playIntro = !SaveManager.IsRunStarted;

		if (playIntro)
		{
			PlayIntro();
			await Task.Delay(Mathf.RoundToInt(delayIntro * 1000));
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
