using System;
using UnityEngine;

namespace Game.GlobalComponent
{
	[Serializable]
	public class MarkContainer
	{
		public UIMarkViewBase mark;

		[SerializeField]
		private Transform target;

		[Range(0f, 100f)]
		public float MinDistanceView = 3f;

		public Vector3 offset;

		public Transform Target
		{
			get
			{
				return target;
			}
			set
			{
				target = value;
			}
		}

		public MarkContainer()
		{
		}

		public MarkContainer(UIMarkViewBase markView, Transform targetTransform, MarkDetails details)
		{
			mark = markView;
			target = targetTransform;
			SetupMark(details);
		}

		public void SetupMark(MarkDetails details)
		{
			if (!(mark == null))
			{
				mark.SetIconSprite(details.icon);
				mark.IconColor = details.color;
				mark.transform.localScale = details.scale;
				MinDistanceView = details.hideDistance;
				offset = details.offset;
			}
		}

		public void FreeResources()
		{
			target = null;
			if (mark != null)
			{
				UnityEngine.Object.Destroy(mark.gameObject);
			}
		}
	}
}
