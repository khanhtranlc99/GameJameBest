using Game.Character.CharacterController;
using Game.Character.Modes;
using Game.GlobalComponent;
using Game.GlobalComponent.Qwest;
using Game.MiniMap;
using Game.Vehicle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Character
{
	public class PlayerInteractionsManager : MonoBehaviour
	{
		private const float DropFromSpeedCar = 15f;

		private const float DropFromSpeedMoto = 7f;

		private const float DropFromSpeedFastMoto = 20f;

		private const float waitingTimeForVehicleButtons = 5f;

		private const float distanceForDisableGetIn = 6f;

		private const float JumpOutFromCarAnimationLenght = 1.05f;

		private const float JumpOutFromMotoAnimationLenght = 0.2f;

		private const float JumpOutFromHelicopterAnimationLenght = 2.7f;

		private const int GetInMotorcycleAnimationLength = 2;

		private static PlayerInteractionsManager instance;

		public DrivableVehicle LastDrivableVehicle;

		public float MovePlayerToVehicleSpeed = 10f;

		public float RotatePlayerToVehicleSpeed = 1f;

		private bool MovePlayerToVehicle;

		private Coroutine m_TeleportCoroutine;

		[Separator("Buttons")]
		public Button GetInVehicleButton;

		public SpritesForGetInButton GetInSprites;

		public Button GetOutVehicleButton;

		public Button GetInMetroButton;

		private Transform skeletonOfCharacter;

		private Transform characterHips;

		private Transform skeletonInVehicle;

		private Transform driverHips;

		public float TweakingTic = 0.2f;

		private float defoultTic = 0.2f;

		private float tweakTimer;

		private float tweakTimeOut;

		private bool forceEnter;

		private List<DrivableVehicle> vehiclesWithObstacles = new List<DrivableVehicle>();

		private List<DrivableVehicle> nearestVehicles = new List<DrivableVehicle>();

		private bool outFromCar;

		private bool currentStateForGetInButton;

		private SlowUpdateProc slowUpdateProc;

		[HideInInspector]
		public bool inVehicle;

		public bool sitInVehicle;

		public static PlayerInteractionsManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = UnityEngine.Object.FindObjectOfType<PlayerInteractionsManager>();
				}
				return instance;
			}
		}

		public Player Player => PlayerManager.Instance.Player;

		public Transform SkeletonOfCharacter => skeletonOfCharacter;

		public Transform CharacterHips => characterHips;

		public Transform SkeletonInVehicle => skeletonInVehicle;

		public Transform DriverHips => driverHips;

		private AnimationController animationController => PlayerManager.Instance.AnimationController;

		public bool IsPlayerInMetro => MetroManager.InstanceExists && MetroManager.Instance.InMetro;

		[HideInInspector]
		public bool InteractionsAllowed => !GameEventManager.Instance.MassacreTaskActive && !PlayerManager.Instance.WeaponIsRecharging;

		private void Awake()
		{
			if (!instance)
			{
				instance = this;
			}
		}

		private void Start()
		{
			slowUpdateProc = new SlowUpdateProc(SlowUpdate, 0.1f);
			defoultTic = TweakingTic;
		}

		private void SlowUpdate()
		{
			if (!inVehicle)
			{
				ChooseNearestVehicle();
			}
		}

		private void Update()
		{
			MovePlayerToVehicleFunction();
		}

		private void FixedUpdate()
		{
			slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void LateUpdate()
		{
			if (animationController.tweakStart)
			{
				animationController.MainAnimator.enabled = false;
				if (tweakTimer > tweakTimeOut)
				{
					animationController.tweakStart = false;
					tweakTimer = 0f;
					sitInVehicle = true;
					SetEnabled(isEnable: true);
				}
				else
				{
					tweakTimer += Time.deltaTime;
				}
				TweakingTic = LastDrivableVehicle.ToSitPositionLerpTic;
				MoveCharacterToSitPosition(characterHips, driverHips, TweakingTic);
				TweakingSkeleton(characterHips, driverHips, TweakingTic);
			}
		}

		private void SetEnabled(bool isEnable)
		{
			if (LastDrivableVehicle.controller == null) return;
			//Debug.LogError(LastDrivableVehicle +" "+LastDrivableVehicle.controller + " "+LastDrivableVehicle.controller.VehicleSpecific);
			var motorcycleSpecific = LastDrivableVehicle.controller.VehicleSpecific as MotorcycleSpecific;
			if (motorcycleSpecific == null) return;
			motorcycleSpecific.HandsIKController.Limbs[0].IsEnabled = isEnable;
			motorcycleSpecific.HandsIKController.Limbs[1].IsEnabled = isEnable;
		}

		public void MoveCharacterToSitPosition(Transform character, Transform sample, float tic = 0.2f)
		{
			character.parent = sample.parent;
			if (tic >= 1f)
			{
				character.position = sample.position;
				character.rotation = sample.rotation;
			}
			else
			{
				character.position = Vector3.Lerp(character.position, sample.position, tic);
				character.rotation = Quaternion.Slerp(character.rotation, sample.rotation, tic);
			}
		}

		public void ResetCharacterHipsParent()
		{
			characterHips.parent = skeletonOfCharacter;
		}

		public void TweakingSkeleton(Transform character, Transform sample, float tic = 0.2f)
		{
			character.rotation = ((!(tic >= 1f)) ? Quaternion.Slerp(character.rotation, sample.rotation, tic) : sample.rotation);
			for (int i = 0; i < character.childCount; i++)
			{
				Transform child = character.GetChild(i);
				Transform transform = sample.Find(child.name);
				if (transform != null)
				{
					TweakingSkeleton(child, transform, tic);
				}
			}
			if (LastDrivableVehicle.GetVehicleType() != VehicleType.Bicycle && LastDrivableVehicle.GetVehicleType() != VehicleType.Motorbike)
			{
				sitInVehicle = true;
			}
		}

		public void StopTweakingSkeleton()
		{
			animationController.tweakStart = false;
		}

		public void GetIntoVehicle()
		{
			if (!LastDrivableVehicle || animationController.tweakStart || inVehicle || MovePlayerToVehicle || !LastDrivableVehicle.IsAbleToEnter() || SuperKick.isInKickState || !animationController.AnimOnGround || !Player.isActiveAndEnabled)
			{
				return;
			}
			switch (LastDrivableVehicle.GetVehicleType())
			{
			case VehicleType.Any:
			case VehicleType.Boat:
			case VehicleType.Plane:
				break;
			case VehicleType.Car:
				GetInOutCar(isIn: true);
				AddObstacle(LastDrivableVehicle);
				break;
			case VehicleType.Bicycle:
				GetInOutBike(isIn: true);
				AddObstacle(LastDrivableVehicle);
				break;
			case VehicleType.Tank:
				GetInOutTank(isIn: true);
				break;
			case VehicleType.Motorbike:
				GetInOutMoto(isIn: true);
				AddObstacle(LastDrivableVehicle);
				break;
			case VehicleType.Copter:
				if (!LastDrivableVehicle.WaterSensor.InWater)
				{
					GetInOutHelicopter(isIn: true);
					AddObstacle(LastDrivableVehicle);
				}
				break;
			case VehicleType.Mech:
				GetInOutMech(isIn: true);
				break;
			}
		}

		public bool IsAbleToInteractWithVehicle()
		{
			return LastDrivableVehicle.IsAbleToEnter();
		}

		public void InteractionWithVehicle()
		{
			if (InteractionsAllowed)
			{
				RemoveObstacles();
				DummyDriver componentInChildren = LastDrivableVehicle.GetComponentInChildren<DummyDriver>();
				if (componentInChildren != null && !componentInChildren.DriverDead)
				{
					forceEnter = componentInChildren.HaveDriver;
				}
				else
				{
					forceEnter = false;
				}
				if (LastDrivableVehicle.VehicleSpecificPrefab.HasRadio)
				{
					RadioManager.Instance.EnableRadio();
				}
				MovePlayerToVehicle = true;
				LastDrivableVehicle.Drive(Player);
				skeletonOfCharacter = Player.GetMetarig();
				characterHips = Player.GetHips();
				driverHips = GetDriverHips(LastDrivableVehicle);
				Player.transform.parent = LastDrivableVehicle.transform;
				Player.GetInOutVehicle(true, forceEnter, LastDrivableVehicle);
				LastDrivableVehicle.controller.Animate(LastDrivableVehicle);
				float delayedActivateTime = (LastDrivableVehicle is DrivableMotorcycle && componentInChildren != null) ? 2 : 0;
				Player.ProceedDriverStatus(LastDrivableVehicle, true, delayedActivateTime);
				ClearNearestVehicle();
				SwitchInOutVehicleButtons(vehicleIn: false, vehicleOut: true, wait: true);
				if (componentInChildren != null)
				{
					componentInChildren.InitOutOfVehicle(forceEnter, Player, IsReplaceOnRagdollAfterAnimation());
				}
				InGameLogManager.Instance.RegisterNewMessage(MessageType.SitInCar, LastDrivableVehicle.VehicleSpecificPrefab.Name.ToString());
				Game.MiniMap.MiniMap.Instance.SetTarget(LastDrivableVehicle.gameObject);
				inVehicle = true;
				GameEventManager.Instance.RefreshQwestArrow();
			}
		}

		public bool IsReplaceOnRagdollAfterAnimation()
		{
			return LastDrivableVehicle != null && LastDrivableVehicle.GetVehicleType().Equals(VehicleType.Motorbike);
		}

		public bool IsPossibleGetOut()
		{
			if (!LastDrivableVehicle)
			{
				return false;
			}
			if (animationController.tweakStart)
			{
				return false;
			}
			if (!inVehicle)
			{
				return false;
			}
			if (MovePlayerToVehicle)
			{
				return false;
			}
			return true;
		}

		public void GetOutFromVehicle(bool isRaggdol = false)
		{
			if (!IsPossibleGetOut())
			{
				return;
			}
			if (LastDrivableVehicle.DeepInWater)
			{
				Player.transform.parent = null;
				ResetCharacterHipsParent();
				Player.rootModel.SetActive(value: true);
				Player.GetInOutVehicle(false, false, LastDrivableVehicle, true);
				Transform transform = Player.transform;
				Vector3 eulerAngles = Player.transform.eulerAngles;
				transform.eulerAngles = new Vector3(0f, eulerAngles.y, 0f);
			}
			else
			{
				//Debug.LogError("type "+LastDrivableVehicle.GetVehicleType());
				switch (LastDrivableVehicle.GetVehicleType())
				{
				case VehicleType.Car:
					GetInOutCar(false);
					break;
				case VehicleType.Bicycle:
					GetInOutBike(false);
					break;
				case VehicleType.Tank:
					GetInOutTank(false);
					break;
				case VehicleType.Motorbike:
					GetInOutMoto(false, isRaggdol);
					break;
				case VehicleType.Copter:
					GetInOutHelicopter(false);
					break;
				case VehicleType.Mech:
					GetInOutMech(false);
					break;
				}
			}
			if (LastDrivableVehicle.VehicleSpecificPrefab.HasRadio)
			{
				RadioManager.Instance.DisableRadio();
			}
			CameraManager.Instance.ResetCameraMode();
			CameraManager.Instance.GetCurrentCameraMode().SetCameraConfigMode("Defaul");
			if (!isRaggdol)
			{
				CameraManager.Instance.SetCameraTarget(Player.transform);
			}
			Controls.SetControlsSubPanel(ControlsType.Character);
			Game.MiniMap.MiniMap.Instance.ResetTarget();
			LastDrivableVehicle.GetOut();
			Player.ProceedDriverStatus(LastDrivableVehicle, false);
			if (LastDrivableVehicle.IsAbleToEnter())
			{
				SwitchInOutVehicleButtons(true, false, true);
			}
			outFromCar = true;
			inVehicle = false;
			GameEventManager.Instance.RefreshQwestArrow();
			sitInVehicle = false;
		}

		private void GetInOutCar(bool isIn)
		{
			if (isIn)
			{
				Vector3 _;
				if (LastDrivableVehicle.IsDoorBlockedOffset(Player.BlockedLayerMask, Player.transform, out _))
				{
					InteractionWithVehicle();
					return;
				}
				Player.MoveToCar = true;
				Player.CarToMove = LastDrivableVehicle.VehiclePoints.EnterFromPositions[0];
				skeletonOfCharacter = Player.GetMetarig();
				tweakTimeOut = 3f;
				return;
			}
			bool crash = true;
			if (LastDrivableVehicle.controller != null)
			{
				crash = !LastDrivableVehicle.controller.EnabledToExit();
			}
			float magnitude = LastDrivableVehicle.MainRigidbody.velocity.magnitude;
			if (magnitude >= 15f && LastDrivableVehicle.HasExitAnimation())
			{
				ResetCharacterHipsParent();
				Player.JumpOutFromVehicle(true, 1.05f, true, true, false, false, LastDrivableVehicle);
				return;
			}
			if (LastDrivableVehicle.controller != null)
			{
				LastDrivableVehicle.controller.StopVehicle(inMoment: true);
			}
			ResetCharacterHipsParent();
			Player.GetInOutVehicle(false, false, LastDrivableVehicle, crash);
			Transform transform = Player.transform;
			Vector3 eulerAngles = Player.transform.eulerAngles;
			transform.eulerAngles = new Vector3(0f, eulerAngles.y, 0f);
		}

		private void GetInOutBike(bool isIn)
		{
			if (isIn)
			{
				Transform transform = LastDrivableVehicle.transform;
				Vector3 eulerAngles = LastDrivableVehicle.transform.eulerAngles;
				float x = eulerAngles.x;
				Vector3 eulerAngles2 = LastDrivableVehicle.transform.eulerAngles;
				transform.eulerAngles = new Vector3(x, eulerAngles2.y, 0f);
				LastDrivableVehicle.MainRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
				Player.MoveToCar = true;
				Player.CarToMove = LastDrivableVehicle.VehiclePoints.EnterFromPositions[0];
				tweakTimeOut = .1f;
			}
			else
			{
				ResetCharacterHipsParent();
				SwitchSkeletons(on: false);
				Player.GetInOutVehicle(false, false, LastDrivableVehicle);
			}
		}

		private void GetInOutMoto(bool isIn, bool isRagdoll = false)
		{
			if (isIn)
			{
				if (Vector3.Dot(LastDrivableVehicle.transform.up, Vector3.up) <= 0.2f)
				{
					LastDrivableVehicle.transform.position += Vector3.up;
				}
				SimpleWheelController[] componentsInChildren = LastDrivableVehicle.GetComponentsInChildren<SimpleWheelController>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].ResetWheelCollider();
				}
				LastDrivableVehicle.transform.LookAt(LastDrivableVehicle.transform.position + LastDrivableVehicle.transform.forward * 2f);
				LastDrivableVehicle.MainRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
				LastDrivableVehicle.MainRigidbody.velocity = Vector3.zero;
				Player.MoveToCar = true;
				Player.CarToMove = ((!LastDrivableVehicle.PointOnTheLeft(Player.transform.position)) ? LastDrivableVehicle.VehiclePoints.EnterFromPositions[1] : LastDrivableVehicle.VehiclePoints.EnterFromPositions[0]);
				tweakTimeOut = .1f;
			}
			else
			{
				float magnitude = LastDrivableVehicle.MainRigidbody.velocity.magnitude;
				if (magnitude >= 7f && !isRagdoll)
				{
					Player.transform.parent = null;
					ResetCharacterHipsParent();
					Player.ResetRotation();
					Player.JumpOutFromVehicle(true, 0.2f, magnitude >= 20f, true, false, false, LastDrivableVehicle);
					LastDrivableVehicle.MainRigidbody.constraints = RigidbodyConstraints.None;
				}
				else
				{
					ResetCharacterHipsParent();
					Player.ResetRotation();
					LastDrivableVehicle.StopVehicle();
					Player.GetInOutVehicle(false, false, LastDrivableVehicle, isRagdoll);
				}
				SetEnabled(isEnable: false);
			}
		}

		private void GetInOutHelicopter(bool isIn)
		{
			if (isIn)
			{
				Player.MoveToCar = true;
				Transform transform = LastDrivableVehicle.transform;
				Vector3 eulerAngles = LastDrivableVehicle.transform.eulerAngles;
				float x = eulerAngles.x;
				Vector3 eulerAngles2 = LastDrivableVehicle.transform.eulerAngles;
				transform.eulerAngles = new Vector3(x, eulerAngles2.y, 0f);
				Player.CarToMove = LastDrivableVehicle.VehiclePoints.EnterFromPositions[0];
				tweakTimeOut = 3f;
			}
			else
			{
				DrivableHelicopter drivableHelicopter = LastDrivableVehicle as DrivableHelicopter;
				ResetCharacterHipsParent();
				if (drivableHelicopter.IsGrounded)
				{
					Player.GetInOutVehicle(false, false, LastDrivableVehicle);
				}
				else
				{
					Player.JumpOutFromVehicle(true, 2.7f, false, false, true, true, LastDrivableVehicle);
				}
			}
		}

		private void GetInOutTank(bool isIn)
		{
			if (isIn)
			{
				Player.rootModel.SetActive(value: false);
				InteractionWithVehicle();
			}
			else
			{
				ResetCharacterHipsParent();
				Player.rootModel.SetActive(value: true);
				Player.GetInOutVehicle(isIn, false, LastDrivableVehicle);
			}
		}

		private void GetInOutMech(bool isIn)
		{
			GetInOutTank(isIn);
			if (!isIn)
			{
				CameraManager.Instance.GetCurrentCameraMode().SetCameraConfigMode("Default");
			}
		}

		public void DieInCar()
		{
			if (!inVehicle)
			{
				return;
			}
			PlayerDieManager.Instance.dieInCar = true;
			PlayerDieManager.Instance.OnPlayerDie();
			CameraManager.Instance.SetMode(Type.Dead);
			if (MovePlayerToVehicle)
			{
				MovePlayerToVehicle = false;
			}
			StopTweakingSkeleton();
			if (Player.IsTransformer)
			{
				InstantExitVehicle();
				return;
			}
			GetOutFromVehicle();
			switch (LastDrivableVehicle.GetVehicleType())
			{
			case VehicleType.Any:
			case VehicleType.Tank:
				break;
			case VehicleType.Bicycle:
				SwitchSkeletons(on: false);
				Player.ReplaceOnRagdoll(canWakeUp: false);
				break;
			case VehicleType.Motorbike:
				Player.transform.parent = null;
				ResetCharacterHipsParent();
				Player.ReplaceOnRagdoll(canWakeUp: false);
				break;
			case VehicleType.Car:
			{
				ResetCharacterHipsParent();
				float animationTimeLength = (!LastDrivableVehicle.HasExitAnimation()) ? 0f : 1.05f;
				Player.JumpOutFromVehicle(false, animationTimeLength, true, false, false, false, LastDrivableVehicle);
				break;
			}
			case VehicleType.Copter:
				Player.transform.parent = null;
				ResetCharacterHipsParent();
				Player.ReplaceOnRagdoll(canWakeUp: false);
				break;
			}
		}

		public void ResetGetInOutButtons()
		{
			SwitchInOutVehicleButtons(vehicleIn: true, vehicleOut: false, wait: true);
			outFromCar = true;
			inVehicle = false;
			sitInVehicle = false;
		}

		public void SwitchSkeletons(bool on, bool isAnimatorRebind = true)
		{
			if (on)
			{
				skeletonOfCharacter.parent = skeletonInVehicle.parent;
				skeletonInVehicle.parent = base.transform;
				skeletonInVehicle.parent = skeletonOfCharacter.parent;
				if (isAnimatorRebind)
				{
					((DrivableBike)LastDrivableVehicle).animator.Rebind();
				}
			}
			else
			{
				skeletonOfCharacter.parent = Player.rootModel.transform;
				if (isAnimatorRebind)
				{
					Player.GetComponent<Animator>().Rebind();
					skeletonOfCharacter.localPosition = Vector3.zero;
					skeletonOfCharacter.localRotation = Quaternion.identity;
				}
			}
		}

		public void SwitchInOutVehicleButtons(bool vehicleIn, bool vehicleOut, bool wait = false)
		{
			if (Player.IsTransformer)
			{
				return;
			}
			if (!GetInVehicleButton || !GetOutVehicleButton)
			{
				UnityEngine.Debug.LogWarning("GetOutButton or GetInButton is not set");
				return;
			}
			currentStateForGetInButton = vehicleIn;
			if ((bool)LastDrivableVehicle)
			{
				SwitchSpriteOnGetInButton(LastDrivableVehicle.GetVehicleType());
			}
			if (!wait && !outFromCar)
			{
				GetInVehicleButton.gameObject.SetActive(vehicleIn);
				GetOutVehicleButton.gameObject.SetActive(vehicleOut);
			}
			else
			{
				StartCoroutine(InVehicleButton());
				StartCoroutine(OutVehicleButton(vehicleOut));
			}
		}

		public bool IsDrivingAVehicle()
		{
			return LastDrivableVehicle != null && LastDrivableVehicle.controller != null;
		}

		public void NewNearestVehicle(DrivableVehicle vehicle)
		{
			if (!nearestVehicles.Contains(vehicle))
			{
				nearestVehicles.Add(vehicle);
			}
		}

		public void AddObstacle(DrivableVehicle vehicle)
		{
			if (vehicle.AddObstacle())
			{
				vehiclesWithObstacles.Add(vehicle);
			}
		}

		public void RemoveObstacles()
		{
			foreach (DrivableVehicle vehiclesWithObstacle in vehiclesWithObstacles)
			{
				vehiclesWithObstacle.RemoveObstacle();
			}
			vehiclesWithObstacles.Clear();
		}

		private void ChooseNearestVehicle()
		{
			if (nearestVehicles.Count.Equals(0) || !InteractionsAllowed)
			{
				SwitchInOutVehicleButtons(vehicleIn: false, vehicleOut: false);
			}
			else if (nearestVehicles.Count.Equals(1) && nearestVehicles[0].IsAbleToEnter())
			{
				float num = Vector3.Distance(Player.transform.position, nearestVehicles[0].transform.position);
				if (num >= 6f)
				{
					nearestVehicles.RemoveAt(0);
					SwitchInOutVehicleButtons(vehicleIn: false, vehicleOut: false);
				}
				else
				{
					LastDrivableVehicle = nearestVehicles[0];
					SwitchInOutVehicleButtons(vehicleIn: true, vehicleOut: false);
				}
			}
			else if (!Player.MoveToCar)
			{
				float num2 = Vector3.Distance(Player.transform.position, nearestVehicles[0].transform.position);
				List<DrivableVehicle> list = new List<DrivableVehicle>();
				foreach (DrivableVehicle nearestVehicle in nearestVehicles)
				{
					float num3 = Vector3.Distance(Player.transform.position, nearestVehicle.transform.position);
					if (num3 >= 6f)
					{
						list.Add(nearestVehicle);
					}
					else if (num3 <= num2 && nearestVehicle.IsAbleToEnter())
					{
						LastDrivableVehicle = nearestVehicle;
						SwitchInOutVehicleButtons(vehicleIn: true, vehicleOut: false);
					}
				}
				List<DrivableVehicle> list2 = nearestVehicles;
				List<DrivableVehicle> list3 = list;
				list2.RemoveAll(list3.Contains);
			}
		}

		public void ClearNearestVehicle()
		{
			nearestVehicles.Clear();
		}

		private void MovePlayerToVehicleFunction()
		{
			if (!MovePlayerToVehicle)
			{
				return;
			}
			if (LastDrivableVehicle == null)
			{
				MovePlayerToVehicle = false;
				return;
			}
			int num = (!LastDrivableVehicle.PointOnTheLeft(Player.transform.position)) ? 1 : 0;
			if (!LastDrivableVehicle.GetVehicleType().Equals(VehicleType.Motorbike))
			{
				num = 0;
			}
			Transform transform = (!forceEnter || LastDrivableVehicle.GetVehicleType().Equals(VehicleType.Motorbike)) ? LastDrivableVehicle.VehiclePoints.EnterFromPositions[num] : LastDrivableVehicle.VehiclePoints.EnterFromPositions[3];
			Player.transform.position = Vector3.Lerp(Player.transform.position, transform.position, Time.deltaTime * MovePlayerToVehicleSpeed);
			Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, transform.rotation, RotatePlayerToVehicleSpeed);
			float sqrMagnitude = (Player.transform.position - transform.position).sqrMagnitude;
			if (sqrMagnitude < 0.1f)
			{
				MovePlayerToVehicle = false;
				Player.transform.position = transform.position;
				Player.transform.rotation = transform.rotation;
				Player.agent.enabled = false;
			}
		}

		private IEnumerator InVehicleButton()
		{
			yield return new WaitForSeconds(5f);
			GetInVehicleButton.gameObject.SetActive(currentStateForGetInButton);
		}

		private IEnumerator OutVehicleButton(bool key)
		{
			yield return new WaitForSeconds(5f);
			GetOutVehicleButton.gameObject.SetActive(key);
			outFromCar = false;
		}

		public void InstantExitVehicle()
		{
			if (!(Player.transform.parent == null))
			{
				DrivableVehicle drivableVehicle = LastDrivableVehicle = Player.transform.parent.GetComponent<DrivableVehicle>();
				drivableVehicle.controller.StopVehicle(inMoment: true);
				ResetCharacterHipsParent();
				Player.transform.parent = null;
				SwitchSkeletons(on: false);
				Player.GetInOutVehicle(false, false, drivableVehicle);
				Player.ResetRotation();
				if (drivableVehicle.VehicleSpecificPrefab.HasRadio)
				{
					RadioManager.Instance.DisableRadio();
				}
				CameraManager.Instance.SetMode(CameraManager.Instance.ActivateModeOnStart);
				CameraManager.Instance.SetCameraTarget(Player.transform);
				Controls.SetControlsByType(ControlsType.Character);
				Game.MiniMap.MiniMap.Instance.ResetTarget();
				drivableVehicle.GetOut();
				Player.ProceedDriverStatus(drivableVehicle, false);
				outFromCar = true;
				inVehicle = false;
				GameEventManager.Instance.RefreshQwestArrow();
				sitInVehicle = false;
			}
		}

		public void InstantEnterVehicle(DrivableVehicle vehicle, bool isTransformer = true)
		{
			LastDrivableVehicle = vehicle;
			if (vehicle.VehicleSpecificPrefab.HasRadio)
			{
				RadioManager.Instance.EnableRadio();
			}
			skeletonOfCharacter = Player.GetMetarig();
			characterHips = Player.GetHips();
			if (!isTransformer)
			{
				driverHips = GetDriverHips(vehicle);
				Player.transform.parent = vehicle.transform;
				MoveCharacterToSitPosition(characterHips, driverHips, 1f);
				MovePlayerToVehicle = true;
			}
			Player.GetInOutVehicle(true, forceEnter, vehicle);
			vehicle.Drive(Player);
			Player.ProceedDriverStatus(vehicle, true);
			ClearNearestVehicle();
			SwitchInOutVehicleButtons(false, true, true);
			InGameLogManager.Instance.RegisterNewMessage(MessageType.SitInCar, vehicle.VehicleSpecificPrefab.Name.ToString());
			Game.MiniMap.MiniMap.Instance.SetTarget(vehicle.gameObject);
			inVehicle = true;
			GameEventManager.Instance.RefreshQwestArrow();
		}

		private void SwitchSpriteOnGetInButton(VehicleType vehicleType)
		{
			switch (vehicleType)
			{
			case VehicleType.Any:
			case VehicleType.Plane:
				break;
			case VehicleType.Car:
				GetInVehicleButton.image.sprite = GetInSprites.Car;
				break;
			case VehicleType.Motorbike:
				GetInVehicleButton.image.sprite = GetInSprites.Motorbike;
				break;
			case VehicleType.Bicycle:
				GetInVehicleButton.image.sprite = GetInSprites.Bicycle;
				break;
			case VehicleType.Tank:
				GetInVehicleButton.image.sprite = GetInSprites.Tank;
				break;
			case VehicleType.Copter:
				GetInVehicleButton.image.sprite = GetInSprites.Copter;
				break;
			case VehicleType.Mech:
				GetInVehicleButton.image.sprite = GetInSprites.Mech;
				break;
			case VehicleType.Boat:
				GetInVehicleButton.image.sprite = GetInSprites.Boat;
				break;
			}
		}

		public Transform GetDriverHips(DrivableVehicle vehicle)
		{
			if (vehicle is DrivableBike)
			{
				skeletonInVehicle = (vehicle as DrivableBike).metarig;
			}
			else
			{
				skeletonInVehicle = vehicle.controller.VehicleSpecific.PlayerSkeleton.Find("metarig");
			}
			return skeletonInVehicle.Find("hips");
		}

		private IEnumerator PlayerToPosition(Vector3 position, Quaternion rotation)
		{
			Transform playerTransform = Player.transform;
			Player player = Player;
			while (inVehicle || PlayerManager.Instance.IsGettingInOrOut || (bool)Player.CurrentRagdoll)
			{
				yield return new WaitForSeconds(0.25f);
			}
			player.gameObject.SetActive(value: false);
			playerTransform.position = position;
			playerTransform.rotation = rotation;
			player.gameObject.SetActive(value: true);
			yield return null;
		}

		public bool TeleportPlayerToPosition(Vector3 position, Quaternion rotation)
		{
			if (Player == null)
			{
				return false;
			}
			if (m_TeleportCoroutine != null)
			{
				StopCoroutine(m_TeleportCoroutine);
			}
			m_TeleportCoroutine = StartCoroutine(PlayerToPosition(position, rotation));
			return true;
		}

		public bool TeleportPlayerToPosition(Vector3 position)
		{
			return TeleportPlayerToPosition(position, Quaternion.identity);
		}

		internal bool TeleportPlayerToPosition(Transform targetTransform)
		{
			return TeleportPlayerToPosition(targetTransform.position, targetTransform.rotation);
		}

		internal Vector3 GetPlayerPosition()
		{
			if (IsDrivingAVehicle())
			{
				return LastDrivableVehicle.transform.position;
			}
			Transform transform;
			if ((transform = Player.GetRagdollHips()) == null)
			{
				transform = Player.transform;
			}
			return transform.position;
		}
	}
}
