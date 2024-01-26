using System.Collections.Generic;
using UnityEngine;

namespace RopeNamespace
{
	internal class RopePointsMaker : List<CPoint>
	{
		private float distance;

		private float phiSpeed;

		private float maxR;

		private float maxL;

		private float dz;

		private float curR;

		private float curPhi;

		private float curZ;

		private Vector3[] points;

		public RopePointsMaker(float phiSpeed, float maxR, float maxL, float dz)
		{
			this.maxL = maxL;
			this.maxR = maxR;
			this.dz = dz;
			this.phiSpeed = phiSpeed;
		}

		public Vector3[] CreateCurve(float distance)
		{
			float num = Mathf.Tan(maxR / maxL);
			float num2 = maxL;
			if (distance < maxL)
			{
				num2 = distance;
			}
			int num3 = (int)(num2 / dz);
			curPhi = 0f;
			curR = 0f;
			curZ = 0f;
			Clear();
			Add(new CPoint(0f, 0f, 0f));
			for (int i = 0; i < num3; i++)
			{
				curPhi += phiSpeed;
				curZ += dz;
				curR = ((!(curR < 0f)) ? ((num2 - curZ) * num) : 0f);
				Add(new CPoint(curR, curPhi, curZ));
			}
			Add(new CPoint(0f, 0f, distance));
			points = new Vector3[Count];
			for (int j = 0; j < Count; j++)
			{
				points[j] = this[j].ToVector3();
			}
			return points;
		}

		public Vector3[] straighteningPoints(float progress)
		{
			Vector3[] array = new Vector3[Count];
			int num = 0;
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CPoint current = enumerator.Current;
					array[num] = current.decreaseR(progress).ToVector3();
					num++;
				}
				return array;
			}
		}
	}
}
