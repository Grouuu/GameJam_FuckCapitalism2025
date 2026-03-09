using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Grouuu.Utils
{
	public class SceneEffect : MonoBehaviour
	{
		public virtual string EffectName => null;

		public virtual async UniTask Play (bool isResumed = false)
		{
			await UniTask.CompletedTask;
		}

		public virtual async UniTask Stop ()
		{
			await UniTask.CompletedTask;
		}
	}
}