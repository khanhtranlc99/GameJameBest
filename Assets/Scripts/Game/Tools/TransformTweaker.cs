using UnityEngine;

namespace Game.Tools
{
	public class TransformTweaker : MonoBehaviour
	{
		public Transform TargetTransform;

		public Transform ChangeableTransform;

		public bool recursive;

		public bool copyLocalValue;

		public int howDeep;

		[Separator("What to copy")]
		public bool copyPosition;

		public bool copyRotation;

		public bool copyScale;

		[InspectorButton("Tweak")]
		public bool tweak;

		private int counterDepth;

		public void Tweak()
		{
			if (!TargetTransform || !ChangeableTransform)
			{
				UnityEngine.Debug.LogWarning("Not set target transform or changable transform");
				return;
			}
			if (!copyPosition && !copyRotation && !copyScale)
			{
				UnityEngine.Debug.LogWarning("At least one option should be selected in 'what to copy'");
				return;
			}
			counterDepth = 0;
			TweakingTransform(TargetTransform, ChangeableTransform);
		}

		private void TweakingTransform(Transform target, Transform changeable)
		{
			CopyValues(target, changeable);
			if (recursive && counterDepth <= howDeep)
			{
				counterDepth++;
				foreach (Transform item in changeable)
				{
					Transform transform2 = target.Find(item.name);
					if ((bool)transform2)
					{
						TweakingTransform(transform2, item);
					}
				}
			}
		}

		private void CopyValues(Transform target, Transform changeable)
		{
			if (copyPosition)
			{
				if (copyLocalValue)
				{
					changeable.localPosition = target.localPosition;
				}
				else
				{
					changeable.position = target.position;
				}
			}
			if (copyRotation)
			{
				if (copyLocalValue)
				{
					changeable.localRotation = target.localRotation;
				}
				else
				{
					changeable.rotation = target.rotation;
				}
			}
			if (copyScale)
			{
				changeable.localScale = target.localScale;
			}
		}
	}
}
