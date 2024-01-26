using Game.Character.CharacterController;
using Game.GlobalComponent;
using Game.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Vehicle
{
	public class TankGunController : MonoBehaviour
	{
		private const string ShootStateName = "Fire";

		private const int ShootEffectDestroyTime = 3;

		private const int Pi = 180;

		public GameObject Tower;

		public GameObject MainGun;

		public GameObject MainGunOut;

		public GameObject LauncherPrefab;

		public int FirekickForce = 500;

		public int RotationTorque = 100;

		public int MainGunSlerpSpeed = 5;

		public float MaximumAngularVelocity = 1f;

		public int MaximumRotaitonLimit = 160;

		public float MinElevationLimit = 10f;

		public float MaxElevationLimit = 25f;

		public bool UseLimitsForRotation = true;

		private Rigidbody rigidBody;

		private RangeWeaponProjectile tankWeapon;

		private HingeJoint joint;

		private JointLimits jointRotationLimit;

		private float inputSteer;

		private Image shootIndicator;

		private TankController currTankController;

		public void Init(DrivableTank drivableTank, HitEntity driver)
		{
			currTankController = (drivableTank.controller as TankController);
			Tower = drivableTank.Tower;
			MainGun = drivableTank.MainGun;
			MainGunOut = drivableTank.MainGunOut;
			tankWeapon = PoolManager.Instance.GetFromPool(LauncherPrefab).GetComponent<RangeWeaponProjectile>();
			tankWeapon.transform.parent = MainGunOut.transform;
			tankWeapon.transform.localPosition = Vector3.zero;
			tankWeapon.transform.localEulerAngles = Vector3.zero;
			tankWeapon.Init();
			tankWeapon.SetWeaponOwner(driver);
			rigidBody = Tower.GetComponent<Rigidbody>();
			rigidBody.maxAngularVelocity = MaximumAngularVelocity;
			tankWeapon.PerformAttackEvent = WeaponAttackEvent;
			joint = Tower.GetComponent<HingeJoint>();
			JointConfiguration();
		}

		public void DeInit()
		{
			tankWeapon.DeInit();
			Transform transform = MainGun.transform;
			float x = 0f - MaxElevationLimit;
			Vector3 eulerAngles = MainGun.transform.eulerAngles;
			float y = eulerAngles.y;
			Vector3 eulerAngles2 = MainGun.transform.eulerAngles;
			transform.eulerAngles = new Vector3(x, y, eulerAngles2.z);
			PoolManager.Instance.ReturnToPool(tankWeapon.gameObject);
		}

		private void Update()
		{
			if (currTankController.EngineEnabled)
			{
				SetShootIndicator();
				Shooting();
			}
		}

		private void FixedUpdate()
		{
			if (currTankController.EngineEnabled)
			{
				CastResult castResult = TargetManager.Instance.ShootFromCamera(tankWeapon.WeaponOwner, tankWeapon.ScatterVector, tankWeapon);
				MoveBarrel(castResult.HitPosition);
			}
		}

		private void MoveBarrel(Vector3 targetWorldPosition)
		{
			Vector3 vector = Tower.transform.InverseTransformPoint(targetWorldPosition);
			inputSteer = vector.x / vector.magnitude;
			float num = (!(inputSteer > 0f)) ? (-RotationTorque) : RotationTorque;
			rigidBody.AddRelativeTorque(0f, num * Mathf.Abs(inputSteer), 0f, ForceMode.Force);
			Quaternion b = Quaternion.LookRotation(targetWorldPosition - MainGun.transform.position);
			MainGun.transform.rotation = Quaternion.Slerp(MainGun.transform.rotation, b, Time.deltaTime * (float)MainGunSlerpSpeed);
			Vector3 localEulerAngles = MainGun.transform.localEulerAngles;
			if (localEulerAngles.x > 0f)
			{
				Vector3 localEulerAngles2 = MainGun.transform.localEulerAngles;
				if (localEulerAngles2.x < 180f)
				{
					Transform transform = MainGun.transform;
					Vector3 localEulerAngles3 = MainGun.transform.localEulerAngles;
					transform.localEulerAngles = new Vector3(Mathf.Clamp(localEulerAngles3.x, 0f - MinElevationLimit, MinElevationLimit), 0f, 0f);
					return;
				}
			}
			Vector3 localEulerAngles4 = MainGun.transform.localEulerAngles;
			if (localEulerAngles4.x > 180f)
			{
				Vector3 localEulerAngles5 = MainGun.transform.localEulerAngles;
				if (localEulerAngles5.x < 360f)
				{
					Transform transform2 = MainGun.transform;
					Vector3 localEulerAngles6 = MainGun.transform.localEulerAngles;
					transform2.localEulerAngles = new Vector3(Mathf.Clamp(localEulerAngles6.x - 360f, 0f - MaxElevationLimit, MaxElevationLimit), 0f, 0f);
				}
			}
		}

		private void Shooting()
		{
			if (Controls.GetButton("Fire"))
			{
				tankWeapon.Attack(null, tankWeapon.transform.forward);
			}
		}

		private void WeaponAttackEvent(Weapon weapon)
		{
			PointSoundManager.Instance.PlayCustomClipAtPoint(tankWeapon.transform.position, tankWeapon.SoundAttack);
			WeaponManager.Instance.StartShootSFX(tankWeapon.transform, tankWeapon.ShotSfx);
			rigidBody.AddForce(-Tower.transform.forward * FirekickForce, ForceMode.VelocityChange);
			tankWeapon.RechargeCall();
		}

		private void JointConfiguration()
		{
			if (UseLimitsForRotation)
			{
				jointRotationLimit.min = -MaximumRotaitonLimit;
				jointRotationLimit.max = MaximumRotaitonLimit;
				joint.limits = jointRotationLimit;
			}
			else
			{
				joint.useLimits = false;
			}
		}

		private void SetShootIndicator()
		{
			if (!(TankControlPanel.Instance == null))
			{
				if (!shootIndicator)
				{
					shootIndicator = TankControlPanel.Instance.ReloadIndicator;
				}
				shootIndicator.fillAmount = tankWeapon.GetRechargeStatus();
			}
		}
	}
}
