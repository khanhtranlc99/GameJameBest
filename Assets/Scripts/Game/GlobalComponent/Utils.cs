using UnityEngine;

namespace Game.GlobalComponent
{
	public class Utils
	{
		public const float MsecToKmh = 3.6f;

		public const float KmhToMsec = 5f / 18f;

		public static void CopyTransforms(Transform ethalon, Transform subject)
		{
			subject.localPosition = ethalon.localPosition;
			subject.localRotation = ethalon.localRotation;
			subject.localScale = ethalon.localScale;
			for (int i = 0; i < ethalon.childCount; i++)
			{
				Transform child = ethalon.GetChild(i);
				Transform child2 = subject.GetChild(i);
				if (child2 != null && child.name.Equals(child2.name))
				{
					CopyTransforms(child, child2);
					continue;
				}
				child2 = subject.Find(child.name);
				if (child2 != null)
				{
					CopyTransforms(child, child2);
				}
			}
		}
	}
}
