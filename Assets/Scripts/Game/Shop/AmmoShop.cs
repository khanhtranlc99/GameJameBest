using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Modes;
using Game.Weapons;
using System;
using System.Collections;
using UnityEngine;

namespace Game.Shop
{
	public class AmmoShop : MonoBehaviour
	{
		[Serializable]
		public class WeaponInSale
		{
			public WeaponNameList WeaponName;

			public WeaponArchetype WeaponArchetype;

			public GameObject WeaponObject;

			[Tooltip("Only for ranged weapons")]
			public GameObject BulletsObject;

			[Range(0f, 10f)]
			public float Damage;

			[Range(0f, 10f)]
			public float AttackSpeed;

			public int Price;

			public int BulletPrice;
		}

		public WeaponInSale[] Weapons;

		public int PushStrength = -1;

		public float PushSpeed = 0.1f;

		private int currentWeapIndex;

		private CameraMode cameraMode;

		private GameObject lastMovableObject;

		private CameraMode oldCameraMode;

		private Transform oldCameraTarget;

		private PlayerStoreProfile playerProfile;

		private Player playerController;

		private Color textColor;

		public void DeInitShop()
		{
			StartCoroutine(MoveWeapon(lastMovableObject, forward: false));
			DeInitCamera();
		}

		private IEnumerator MoveWeapon(GameObject weaponGameObject, bool forward)
		{
			float xCord = forward ? PushStrength : 0;
			float x = xCord;
			Vector3 localPosition = weaponGameObject.transform.localPosition;
			float y = localPosition.y;
			Vector3 localPosition2 = weaponGameObject.transform.localPosition;
			Vector3 movePoint = new Vector3(x, y, localPosition2.z);
			while (true)
			{
				weaponGameObject.transform.localPosition = Vector3.MoveTowards(weaponGameObject.transform.localPosition, movePoint, PushSpeed);
				if (Vector3.Distance(weaponGameObject.transform.localPosition, movePoint) < 0.1f)
				{
					break;
				}
				yield return new WaitForEndOfFrame();
			}
		}

		private void MoveWeaponImmediatly(GameObject weaponGameObject, bool forward)
		{
			StopAllCoroutines();
			float num = forward ? PushStrength : 0;
			float x = num;
			Vector3 localPosition = weaponGameObject.transform.localPosition;
			float y = localPosition.y;
			Vector3 localPosition2 = weaponGameObject.transform.localPosition;
			Vector3 localPosition3 = new Vector3(x, y, localPosition2.z);
			weaponGameObject.transform.localPosition = localPosition3;
		}

		private void InitCamera()
		{
			oldCameraMode = CameraManager.Instance.GetCurrentCameraMode();
			oldCameraTarget = CameraManager.Instance.CameraTarget;
			CameraManager.Instance.SetMode(cameraMode);
			CameraManager.Instance.SetCameraTarget(Weapons[currentWeapIndex].WeaponObject.transform);
		}

		private void DeInitCamera()
		{
			CameraManager.Instance.SetCameraTarget(oldCameraTarget);
			CameraManager.Instance.SetMode(oldCameraMode);
		}
	}
}
