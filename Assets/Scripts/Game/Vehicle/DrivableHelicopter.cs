using Game.Character.CharacterController;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Vehicle
{
	public class DrivableHelicopter : DrivableVehicle
	{
		public const string WaterLayerName = "Water";

		private static int waterLayerNumber = -1;

		[Separator("Helicopter Links")]
		public HelicopterGunController GunController;

		public GameObject MinigunPoint;

		public GameObject[] RocketsPoints;

		public GameObject BigBlade;

		public GameObject SmallBlade;

		public GameObject AdditionalObstacles;

		public float RigidbodyMaxDrag = 1f;

		private float bladesRotateSpeed;

		private HelicopterGunController gunControllerLocal;

		private AudioSource bladesAudioSource;

		private float maxBladeRotateSpeed;

		public float CurrentBladesRotateSpeed
		{
			get
			{
				return bladesRotateSpeed;
			}
			set
			{
				bladesRotateSpeed = value;
			}
		}

		public bool IsGrounded
		{
			get
			{
				HelicopterController helicopterController = controller as HelicopterController;
				return !helicopterController || helicopterController.IsGrounded;
			}
		}

		public override VehicleType GetVehicleType()
		{
			return VehicleType.Copter;
		}

		public override void Init()
		{
			base.Init();
			bladesRotateSpeed = 0f;
		}

		public override void Drive(Player driver)
		{
			base.Drive(driver);
			StopAllCoroutines();
			if (GunController != null)
			{
				gunControllerLocal = PoolManager.Instance.GetFromPool(GunController);
				PoolManager.Instance.AddBeforeReturnEvent(gunControllerLocal, delegate
				{
					gunControllerLocal.DeInit();
				});
				gunControllerLocal.transform.parent = base.transform;
				gunControllerLocal.transform.localPosition = Vector3.zero;
				gunControllerLocal.transform.localEulerAngles = Vector3.zero;
				gunControllerLocal.Init(this, driver);
			}
		}

		public override void GetOut()
		{
			base.GetOut();
			if (gunControllerLocal != null)
			{
				PoolManager.Instance.ReturnToPool(gunControllerLocal);
				gunControllerLocal = null;
			}
		}

		public override bool AddObstacle()
		{
			bool flag = base.AddObstacle();
			if (flag)
			{
				AdditionalObstacles.SetActive(value: true);
			}
			return flag;
		}

		public override void RemoveObstacle()
		{
			base.RemoveObstacle();
			if (AdditionalObstacles.activeInHierarchy)
			{
				AdditionalObstacles.SetActive(value: false);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			if (waterLayerNumber == -1)
			{
				waterLayerNumber = LayerMask.NameToLayer("Water");
			}
			bladesAudioSource = GetComponentInChildren<AudioSource>();
			bladesAudioSource.Stop();
			HelicopterController component = VehicleControllerPrefab.GetComponent<HelicopterController>();
			if (component != null)
			{
				maxBladeRotateSpeed = component.MaxBladesRotateSpeed;
			}
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
			BladesRotateControll();
			BladesSoundControl();
			RigidbodyDragControll();
		}

		private void BladesRotateControll()
		{
			if (bladesRotateSpeed > 0f)
			{
				if (!controller)
				{
					bladesRotateSpeed -= Time.deltaTime;
				}
				BigBlade.transform.Rotate(0f, bladesRotateSpeed, 0f, Space.Self);
				SmallBlade.transform.Rotate(bladesRotateSpeed, 0f, 0f, Space.Self);
			}
		}

		private void BladesSoundControl()
		{
			if (bladesRotateSpeed > 0f)
			{
				if (!bladesAudioSource.isPlaying)
				{
					bladesAudioSource.Play();
				}
				float num = bladesRotateSpeed / maxBladeRotateSpeed;
				if (num >= 0f)
				{
					bladesAudioSource.pitch = num;
				}
			}
			else if (bladesAudioSource.isPlaying)
			{
				bladesAudioSource.Stop();
			}
		}

		private void RigidbodyDragControll()
		{
			base.MainRigidbody.drag = bladesRotateSpeed / maxBladeRotateSpeed * RigidbodyMaxDrag;
		}
	}
}
