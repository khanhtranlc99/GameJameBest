using UnityEngine;

namespace IKanimations
{
	public class PreviewHandTarget : MonoBehaviour
	{
		private PreviewIKHandler IKHandler;

		private Transform LeftHandTargetOnPlayer;

		public Transform LeftHandHintOnWeapon;

		private Transform LeftHandHintOnPlayer;

		private void OnEnable()
		{
			IKHandler = UnityEngine.Object.FindObjectOfType<PreviewIKHandler>();
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
				LeftHandTargetOnPlayer.localRotation = transform.localRotation;
			}
			if ((bool)LeftHandHintOnWeapon && (bool)LeftHandHintOnPlayer)
			{
				LeftHandHintOnPlayer.position = LeftHandHintOnWeapon.transform.position;
			}
		}
	}
}
