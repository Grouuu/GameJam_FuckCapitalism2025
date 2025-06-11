using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Splines;

public class IntroAnimation : GameAnimation
{
	public SplineAnimate spline;
	public int delayInSecond;

	public override async Awaitable Play ()
	{
		spline.Play();

		if (SaveManager.IsRunStarted)
		{
			spline.ElapsedTime = spline.Duration;
			return;
		}

		await Task.Delay(Mathf.RoundToInt(delayInSecond * 1000));
	}
}
