using UnityEngine;

namespace Game.Character.Extras
{
	[RequireComponent(typeof(Projector))]
	public class RTSProjector : MonoBehaviour
	{
		public static RTSProjector Instance;

		public float AnimTimeout;

		public float Distance;

		public float FovMax;

		public float FovMin;

		private Projector projector;

		private float timeout;

		private void Awake()
		{
			Instance = this;
			projector = GetComponent<Projector>();
		}

		public void Enable()
		{
			projector.enabled = true;
		}

		public void Disable()
		{
			projector.enabled = false;
		}

		public void Project(Vector3 pos, Color color)
		{
			projector.material.color = color;
			Enable();
			projector.fieldOfView = FovMax;
			timeout = AnimTimeout;
			base.transform.position = pos + Vector3.up * Distance;
		}

		private void Update()
		{
			timeout -= Time.deltaTime;
			if (timeout > 0f)
			{
				float num = timeout / AnimTimeout;
				float num2 = (!((double)num > 0.5)) ? (num / 0.5f) : ((num - 0.5f) / 0.5f);
				projector.fieldOfView = FovMin + (FovMax - FovMin) * num2;
			}
			else
			{
				projector.fieldOfView = FovMax;
			}
		}
	}
}
