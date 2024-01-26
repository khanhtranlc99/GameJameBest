using UnityEngine;

namespace Game.Enemy
{
	public class BaseControllerNPC : MonoBehaviour
	{
		public Animator AnimatorWithController;

		protected BaseNPC CurrentControlledNpc;

		public bool IsInited
		{
			get;
			protected set;
		}

		public virtual void Init(BaseNPC controlledNPC)
		{
			CurrentControlledNpc = controlledNPC;
			IsInited = true;
			if ((bool)AnimatorWithController)
			{
				controlledNPC.NPCAnimator.runtimeAnimatorController = AnimatorWithController.runtimeAnimatorController;
			}
		}

		public virtual void DeInit()
		{
			CurrentControlledNpc = null;
			IsInited = false;
		}

		protected virtual void Update()
		{
			if (IsInited)
			{
			}
		}
	}
}
