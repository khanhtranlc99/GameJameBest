using UnityEngine;

namespace RopeNamespace
{
	internal class CPoint
	{
		private float r;

		private float phi;

		public float z
		{
			get;
			private set;
		}

		public float x => r * Mathf.Cos(phi);

		public float y => r * Mathf.Sin(phi);

		public CPoint(float r, float phi, float z)
		{
			this.r = r;
			this.phi = phi;
			this.z = z;
		}

		public CPoint decreaseR(float progress)
		{
			return new CPoint(r * (1f - progress), phi, z);
		}

		public void SetR(float r)
		{
			this.r = ((!(r > 0f)) ? 0f : r);
		}

		public Vector3 ToVector3()
		{
			return new Vector3(x, y, z);
		}
	}
}
