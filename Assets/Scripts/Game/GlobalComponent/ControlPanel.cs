using UnityEngine;

namespace Game.GlobalComponent
{
	[RequireComponent(typeof(Animator))]
	public class ControlPanel : MonoBehaviour
	{
		public ControlsType ControlsType;

		private Animator panelAnimator;

		public ControlsType GetPanelType()
		{
			return ControlsType;
		}

		public virtual void OnOpen()
		{
		}

		public virtual void OnClose()
		{
		}

		public Animator GetPanelAnimator()
		{
			return panelAnimator ? panelAnimator : (panelAnimator = GetComponent<Animator>());
		}
	}
}
