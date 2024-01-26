using Game.Character.CharacterController;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Enemy
{
	public class RagdollDrowning : MonoBehaviour
	{
		private const float FloatingForсe = 1100f;

		private const float ClampVelocity = 0.5f;

		private float waterHight;

		private Rigidbody hips;

		private bool isInit;

		public void Init(Transform hips, float hight)
		{
			isInit = true;
			waterHight = hight;
			this.hips = hips.GetComponent<Rigidbody>();
			this.hips.velocity = Vector3.zero;
		}

		private void OnDisable()
		{
			isInit = false;
		}

		private void FixedUpdate()
		{
			if (isInit)
			{
				if (!PlayerManager.Instance.OnPlayerSignline(base.transform))
				{
					PoolManager.Instance.ReturnToPool(this);
					return;
				}
				float num = waterHight;
				Vector3 position = hips.transform.position;
				float d = num - position.y;
				hips.AddForce(Vector3.up * 1100f * d);
				Rigidbody rigidbody = hips;
				Vector3 velocity = hips.velocity;
				float x = Mathf.Clamp(velocity.x, -0.5f, 0.5f);
				Vector3 velocity2 = hips.velocity;
				float y = velocity2.y;
				Vector3 velocity3 = hips.velocity;
				rigidbody.velocity = new Vector3(x, y, Mathf.Clamp(velocity3.z, -0.5f, 0.5f));
				Rigidbody rigidbody2 = hips;
				Vector3 angularVelocity = hips.angularVelocity;
				float x2 = angularVelocity.x;
				Vector3 angularVelocity2 = hips.angularVelocity;
				rigidbody2.angularVelocity = new Vector3(x2, 0f, angularVelocity2.z);
				Transform transform = hips.transform;
				Vector3 position2 = hips.transform.position;
				Vector3 position3 = hips.transform.position;
				float x3 = position3.x;
				float y2 = waterHight;
				Vector3 position4 = hips.transform.position;
				transform.position = Vector3.Lerp(position2, new Vector3(x3, y2, position4.z), Time.deltaTime);
			}
		}
	}
}
