using UnityEngine;

namespace IKanimations
{
	public class IKPointOnGun : MonoBehaviour
	{
		private ShootinIKHandler IKHandler;

		private Transform LeftHandTargetOnPlayer;

		public Transform LeftHandHintOnWeapon;

		private Transform LeftHandHintOnPlayer;

		private void OnEnable()
		{
			IKHandler = UnityEngine.Object.FindObjectOfType<ShootinIKHandler>();
			IKHandler.DoIKAnimation = true;
			LeftHandTargetOnPlayer = IKHandler.leftIKHandTarget;
			LeftHandHintOnPlayer = IKHandler.leftIKHandHint;
		}

		private void OnDisable()
		{
			IKHandler.DoIKAnimation = false;
		}

		private void Update()
		{
			if ((bool)LeftHandTargetOnPlayer)
			{
				LeftHandTargetOnPlayer.position = base.transform.position;
			}
			if ((bool)LeftHandHintOnWeapon && (bool)LeftHandHintOnPlayer)
			{
				LeftHandHintOnPlayer.position = LeftHandHintOnWeapon.transform.position;
			}
		}
	}
}
