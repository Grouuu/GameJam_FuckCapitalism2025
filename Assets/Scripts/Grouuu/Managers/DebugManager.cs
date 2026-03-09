using Grouuu.GameDebug;
using UnityEngine;

namespace Grouuu.Managers
{
	public class DebugManager : MonoBehaviour
	{
		public DialogChoiceDebug DialogChoice;

		public void OnYesHoverIn ()		=> DialogChoice?.OnHoverIn(true);
		public void OnYesHoverOut ()	=> DialogChoice?.OnHoverOut(true);
		public void OnNoHoverIn ()		=> DialogChoice?.OnHoverIn(false);
		public void OnNoHoverOut ()		=> DialogChoice?.OnHoverOut(false);

		private void OnEnable ()
		{
			DialogChoice.Setup();
			PreventUseInProduction();
		}

		private void PreventUseInProduction ()
		{
#if !UNITY_EDITOR
		DialogChoice.Enabled = false;
#endif
		}
	}
}
