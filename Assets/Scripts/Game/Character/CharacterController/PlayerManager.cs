using Game.Character.CollisionSystem;
using Game.Character.Modes;
using Game.Enemy;
using Game.GlobalComponent.Qwest;
using Game.Tools;
using Game.Weapons;
using UnityEngine;

namespace Game.Character.CharacterController
{
	public class PlayerManager : MonoBehaviour
	{
		public LayerMask PlayerSignlineMask;

		public float PlayerSignlineRadius = 50f;

		public Player Player;

		public WeaponController WeaponController;

		public AnimationController AnimationController;

		public StuffHelper CurrentStuffHelper;

		private GameObject defaultPlayerInstance;

		private PlayerSetuperHelper defaultHelper;

		private GameObject defaultRagdollInstance;

		private CameraMode defaulCameraMode;

		private GameObject currPlayerInstance;

		private static PlayerManager instance;

		private GameObject playerRagdollInstance;

		public static PlayerManager Instance => instance ?? (instance = UnityEngine.Object.FindObjectOfType<PlayerManager>());

		public GameObject PlayerRagdoll => playerRagdollInstance ?? (playerRagdollInstance = UnityEngine.Object.Instantiate(Player.Ragdoll));

		public HitEntity PlayerAsTarget
		{
			get
			{
				if (PlayerInteractionsManager.Instance.inVehicle)
				{
					return PlayerInteractionsManager.Instance.LastDrivableVehicle.CurrentDriver;
				}
				if (Player.CurrentRagdoll != null)
				{
					RagdollStatus componentInChildren = Player.CurrentRagdoll.GetComponentInChildren<RagdollStatus>();
					if (componentInChildren != null)
					{
						return componentInChildren;
					}
				}
				return Player;
			}
		}

		public bool PlayerIsDefault => Player == DefaulPlayer;

		public bool IsGettingInOrOut => AnimationController.IsGettingInOrOut;

		public StuffHelper DefaultStuffHelper
		{
			get;
			protected set;
		}

		public StuffHelper DefaultRagdollStuffHelper
		{
			get;
			protected set;
		}

		public WeaponController DefaultWeaponController
		{
			get;
			protected set;
		}

		public AnimationController DefaulAnimationController
		{
			get;
			protected set;
		}

		public Player DefaulPlayer
		{
			get;
			protected set;
		}

		public bool WeaponIsRecharging
		{
			get
			{
				RangedWeapon rangedWeapon = WeaponController.CurrentWeapon as RangedWeapon;
				return rangedWeapon != null && rangedWeapon.IsRecharging;
			}
		}

		public bool IsOriginalPlayer => currPlayerInstance == defaultPlayerInstance;

		private void Awake()
		{
			instance = this;
			SetDefaultPlayer();
		}

		public bool OnPlayerSignline(Transform target)
		{
			Vector3 position = Player.transform.position;
			Vector3 position2 = target.position;
			return !Physics.Linecast(position, position2, PlayerSignlineMask);
		}

		public bool OnPlayerSignline(HitEntity target)
		{
			return OnPlayerSignline(target, PlayerSignlineRadius);
		}

		public bool OnPlayerSignline(HitEntity target, float maxDistance)
		{
			Vector3 position = Player.transform.position;
			Vector3 up = Player.transform.up;
			Vector3 nPCShootVectorOffset = Player.NPCShootVectorOffset;
			Vector3 a = position + up * nPCShootVectorOffset.y;
			Vector3 right = Player.transform.right;
			Vector3 nPCShootVectorOffset2 = Player.NPCShootVectorOffset;
			Vector3 a2 = a + right * nPCShootVectorOffset2.x;
			Vector3 forward = Player.transform.forward;
			Vector3 nPCShootVectorOffset3 = Player.NPCShootVectorOffset;
			Vector3 vector = a2 + forward * nPCShootVectorOffset3.z;
			Vector3 position2 = target.transform.position;
			Vector3 up2 = target.transform.up;
			Vector3 nPCShootVectorOffset4 = target.NPCShootVectorOffset;
			Vector3 a3 = position2 + up2 * nPCShootVectorOffset4.y;
			Vector3 right2 = target.transform.right;
			Vector3 nPCShootVectorOffset5 = target.NPCShootVectorOffset;
			Vector3 a4 = a3 + right2 * nPCShootVectorOffset5.x;
			Vector3 forward2 = target.transform.forward;
			Vector3 nPCShootVectorOffset6 = target.NPCShootVectorOffset;
			Vector3 a5 = a4 + forward2 * nPCShootVectorOffset6.z;
			float maxDistance2 = Mathf.Min((a5 - vector).magnitude, maxDistance);
			RaycastHit hitInfo;
			return !Physics.Raycast(vector, a5 - vector, out hitInfo, maxDistance2, PlayerSignlineMask);
		}

		public PlayerSetuperHelper GetDefaultHelper()
		{
			if (defaultHelper == null)
			{
				SetDefaultPlayer();
			}
			return defaultHelper;
		}

		public void SetDefaultPlayer()
		{
			DefaulPlayer = (Player = UnityEngine.Object.FindObjectOfType<Player>());
			AnimationController animationController = AnimationController = (DefaulAnimationController = Player.GetComponent<AnimationController>());
			WeaponController weaponController = WeaponController = (DefaultWeaponController = Player.GetComponent<WeaponController>());
			StuffHelper stuffHelper = CurrentStuffHelper = (DefaultStuffHelper = Player.GetComponent<StuffHelper>());
			currPlayerInstance = (defaultPlayerInstance = Player.gameObject);
			if (playerRagdollInstance == null)
			{
				playerRagdollInstance = (defaultRagdollInstance = UnityEngine.Object.Instantiate(Player.Ragdoll));
				DefaultRagdollStuffHelper = playerRagdollInstance.GetComponent<StuffHelper>();
				DefaultRagdollStuffHelper.DefaultClotheses = DefaultStuffHelper.DefaultClotheses;
			}
			playerRagdollInstance.SetActive(value: false);
			defaultHelper = defaultPlayerInstance.GetComponent<PlayerSetuperHelper>();
			defaultHelper.Initializer.Initialaize();
			defaulCameraMode = CameraManager.Instance.ActivateModeOnStart;
		}

		public void SwitchPlayer(GameObject newPlayerPrefab)
		{
			Transformer component = Player.GetComponent<Transformer>();
			if (component != null && component.currentForm == TransformerForm.Car)
			{
				component.ResetToRobotForm();
			}
			if (PlayerInteractionsManager.Instance.inVehicle)
			{
				PlayerInteractionsManager.Instance.InstantExitVehicle();
			}
			Player.ResetRagdoll();
			if (currPlayerInstance != defaultPlayerInstance)
			{
				WeaponManager.Instance.ResetShootSFX();
				UnityEngine.Object.Destroy(currPlayerInstance);
			}
			else
			{
				defaultPlayerInstance.SetActive(value: false);
			}
			currPlayerInstance = (UnityEngine.Object.Instantiate(newPlayerPrefab, Player.transform.position, Player.transform.rotation) as GameObject);
			CurrentStuffHelper = currPlayerInstance.GetComponent<StuffHelper>();
			PlayerSetuperHelper component2 = currPlayerInstance.GetComponent<PlayerSetuperHelper>();
			Player = component2.PlayerScript;
			WeaponController = component2.WeaponController;
			AnimationController = component2.AnimationController;
			if (playerRagdollInstance != null && playerRagdollInstance != defaultRagdollInstance)
			{
				UnityEngine.Object.Destroy(playerRagdollInstance);
			}
			else
			{
				playerRagdollInstance.SetActive(value: false);
			}
			playerRagdollInstance = UnityEngine.Object.Instantiate(Player.Ragdoll);
			playerRagdollInstance.SetActive(value: false);
			PlayerSetuper.SetUpManagers(component2);
			PlayerSetuper.SetUpUI(component2, defaultHelper);
			component2.Initializer.Initialaize();
			if (component2.CameraModeType != CameraManager.Instance.ActivateModeOnStart.Type)
			{
				CameraManager.Instance.ActivateModeOnStart = CameraManager.Instance.GetCameraModeByType(component2.CameraModeType);
			}
			CameraCollision.Instance.SetCollisionConfig(component2.CollisionConfigName);
			CameraManager.Instance.ResetCameraMode();
			GameEventManager.Instance.RefreshQwestArrow();
			SuperKick componentInChildren = defaultPlayerInstance.GetComponentInChildren<SuperKick>();
			if (componentInChildren != null)
			{
				componentInChildren.Reset();
			}
		}

		public void ResetPlayer()
		{
			if (!IsOriginalPlayer)
			{
				Transformer component = Player.GetComponent<Transformer>();
				if (component != null && component.currentForm == TransformerForm.Car)
				{
					Player.GetComponent<Transformer>().ResetToRobotForm();
				}
				if (PlayerInteractionsManager.Instance.inVehicle)
				{
					PlayerInteractionsManager.Instance.InstantExitVehicle();
				}
				Player.ResetRagdoll();
				defaultPlayerInstance.transform.position = currPlayerInstance.transform.position;
				defaultPlayerInstance.transform.forward = currPlayerInstance.transform.forward;
				WeaponManager.Instance.ResetShootSFX();
				UnityEngine.Object.Destroy(currPlayerInstance);
				currPlayerInstance = defaultPlayerInstance;
				defaultPlayerInstance.SetActive(value: true);
				Player = defaultHelper.PlayerScript;
				WeaponController = defaultHelper.WeaponController;
				AnimationController = defaultHelper.AnimationController;
				if (playerRagdollInstance != null)
				{
					UnityEngine.Object.Destroy(playerRagdollInstance);
				}
				playerRagdollInstance = defaultRagdollInstance;
				playerRagdollInstance.SetActive(value: false);
				PlayerSetuper.SetUpManagers(defaultHelper);
				PlayerSetuper.SetUpUI(defaultHelper, defaultHelper);
				defaultHelper.Initializer.Initialaize();
				CameraManager.Instance.ActivateModeOnStart = defaulCameraMode;
				CameraManager.Instance.ResetCameraMode();
				CameraCollision.Instance.SetCollisionConfig("Default");
				GameEventManager.Instance.RefreshQwestArrow();
				SuperKick componentInChildren = defaultPlayerInstance.GetComponentInChildren<SuperKick>();
				if (componentInChildren != null)
				{
					componentInChildren.Reset();
				}
			}
		}
	}
}
