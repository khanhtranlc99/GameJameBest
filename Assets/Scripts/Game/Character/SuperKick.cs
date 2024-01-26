using Game.Character.CharacterController;
using Game.Enemy;
using Game.GlobalComponent;
using Game.Shop;
using Game.Vehicle;
using UnityEngine;

namespace Game.Character
{
	public class SuperKick : MonoBehaviour
	{
		private const string SuperKickAxisName = "SuperKick";

		private const float MaxKickDistance = 5f;

		public static bool isInKickState;

		public GameObject SuperKickButton;

		public Rigidbody MainRigidbody;

		public LayerMask GroundLayerMask;

		public float TimeOffsetForAnimation = 0.5f;

		public float kickDamage = 100f;

		public float HorizontalPushForce = 700f;

		public float VerticalPushForce = 250f;

		public bool IgnoreMass = true;

		public int staminaForKick = 50;

		public ForceMultipliers multipliers;

		public bool debug;

		private GameObject lastTarget;

		private Vector3 hitPosition;

		private float distance;

		private float currentMultipler;

		private float kickTimer;

		private bool isAbleToKick;

		private bool isGrounded;

		private bool isEnoughStamina;

		private bool isEnoughClose;

		private AnimationController animationController;

		private Player player;

		private bool KickConditions => isAbleToKick && isGrounded && isEnoughStamina && isEnoughClose && !player.MoveToCar && player.isActiveAndEnabled && kickTimer < 0f && (!player.IsTransformer || !player.Transformer.transformating);

		private void Start()
		{
			animationController = base.transform.GetComponentInParent<AnimationController>();
			player = PlayerInteractionsManager.Instance.Player;
		}

		private void OnTriggerStay(Collider other)
		{
			if (isAbleToKick) return;
			isAbleToKick = true;
			lastTarget = other.gameObject;
			hitPosition = other.transform.position;
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.gameObject == lastTarget)
			{
				isAbleToKick = false;
				isInKickState = false;
			}
		}

		public void Reset()
		{
			isAbleToKick = false;
			isInKickState = false;
			SuperKickButton.SetActive(value: false);
		}

		private void Update()
		{
			EnableButton();
			KickProcessing();
			StaminaProcessing();
		}

		private void FixedUpdate()
		{
			GroundCheck();
			DistanceCheck();
			kickTimer -= Time.deltaTime;
		}

		private void KickProcessing()
		{
			if (Controls.GetButtonDown("SuperKick") && KickConditions)
			{
				kickTimer = 3f;
				isInKickState = true;
				StartAnim();
				Invoke("Kick", TimeOffsetForAnimation);
				StaminaProcessing(spendStamina: true);
			}
		}

		private void StartAnim()
		{
			animationController.MainAnimator.SetTrigger("DoAbility");
		}

		private void EnableButton()
		{
			if (KickConditions)
			{
				SuperKickButton.SetActive(value: true);
			}
			else
			{
				SuperKickButton.SetActive(value: false);
			}
		}

		private void DistanceCheck()
		{
			if (lastTarget == null) return;
			distance = Vector3.Distance(lastTarget.transform.position, player.transform.position);
			isEnoughClose = (distance <= 5f);
			if (isEnoughClose) return;
			lastTarget = null;
			isAbleToKick = false;
		}

		private void GroundCheck()
		{
			isGrounded = (animationController.AnimOnGround && !player.IsSwiming);
		}

		private void Push(Rigidbody r, float multiplier)
		{
			Vector3 a = base.transform.forward * HorizontalPushForce + base.transform.up * VerticalPushForce;
			Vector3 torque = -base.transform.up;
			float d = (!IgnoreMass) ? 1f : r.mass;
			r.AddForceAtPosition(a * d * multiplier, base.transform.position + Vector3.up);
			r.AddTorque(torque);
			isInKickState = false;
		}

		private void Kick()
		{
			if (!lastTarget)
			{
				isInKickState = false;
				return;
			}
			hitPosition = lastTarget.transform.position;
			HitEntity hitEntity = lastTarget.GetComponent<HitEntity>();
			if (hitEntity != null && hitEntity.DeadByDamage(kickDamage))
			{
				hitEntity.KilledByAbillity = WeaponNameList.SuperKick;
			}
			Human human = lastTarget.GetComponent<Human>();
			Rigidbody componentInChildren;
			if (human!=null)
			{
				currentMultipler = multipliers.human;
				GameObject initRagdoll;
				human.ReplaceOnRagdoll(!human.DeadByDamage(kickDamage), out initRagdoll);
				componentInChildren = initRagdoll.GetComponentInChildren<Rigidbody>();
				human.OnHit(DamageType.MeleeHit, player, kickDamage, hitPosition, player.transform.forward, 0f);
				Push(componentInChildren, currentMultipler);
				PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.KickHumen);
				return;
			}
			BodyPartDamageReciever bodyPartDamageReciever = lastTarget.GetComponent<BodyPartDamageReciever>();
			if (bodyPartDamageReciever!=null)
			{
				HumanoidStatusNPC humanoidStatusNPC = bodyPartDamageReciever.RerouteEntity as HumanoidStatusNPC;
				if (humanoidStatusNPC !=null)
				{
					BaseControllerNPC controller;
					humanoidStatusNPC.BaseNPC.ChangeController(BaseNPC.NPCControllerType.Smart, out controller);
					SmartHumanoidController smartHumanoidController = controller as SmartHumanoidController;
					if (smartHumanoidController != null)
					{
						smartHumanoidController.AddTarget(player);
						smartHumanoidController.InitBackToDummyLogic();
					}
					if (humanoidStatusNPC.DeadByDamage(kickDamage))
					{
						humanoidStatusNPC.KilledByAbillity = WeaponNameList.Ability;
					}
					if (humanoidStatusNPC.Ragdollable)
					{
						GameObject _;
						humanoidStatusNPC.ReplaceOnRagdoll(humanoidStatusNPC.Health.Current - kickDamage, out _);
						componentInChildren = humanoidStatusNPC.GetRagdollHips().GetComponent<Rigidbody>();
						Push(componentInChildren, multipliers.human);
					}
					humanoidStatusNPC.OnHit(DamageType.MeleeHit, player, kickDamage, hitPosition, player.transform.forward, 0f);
					PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.KickHumen);
					return;
				}
			}
			componentInChildren = lastTarget.GetComponent<Rigidbody>();
			if (componentInChildren !=null)
			{
				RagdollDamageReciever ragdollDamageReciever = componentInChildren.transform.GetComponent<RagdollDamageReciever>();
				if (ragdollDamageReciever!=null)
				{
					RagdollWakeUper ragdollWakeUper = ragdollDamageReciever.rootParent.GetComponent<RagdollWakeUper>();
					if (ragdollWakeUper!=null && ragdollWakeUper.CurrentState != RagdollState.Ragdolled)
					{
						ragdollWakeUper.SetRagdollWakeUpStatus(wakeUp: false);
					}
					ragdollDamageReciever.OnHit(DamageType.MeleeHit, player, 1000f, hitPosition, player.transform.forward, 0f);
					PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.KickHumen);
					isInKickState = false;
					return;
				}
				Rigidbody[] componentsInParent = componentInChildren.GetComponentsInParent<Rigidbody>();
				Rigidbody[] array = componentsInParent;
				foreach (Rigidbody rigidbody in array)
				{
					if (rigidbody.name == "hips")
					{
						componentInChildren = rigidbody;
						RagdollWakeUper componentInChildren2 = componentInChildren.gameObject.GetComponentInChildren<RagdollWakeUper>();
						if (componentInChildren2 != null)
						{
							componentInChildren2.DeInitRagdoll(mainObjectDead: true);
						}
						PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.KickHumen);
						break;
					}
				}
				Push(componentInChildren, multipliers.ragdoll);
				return;
			}
			componentInChildren = lastTarget.GetComponentInParent<Rigidbody>();
			if (componentInChildren !=null)
			{
				DrivableVehicle drivableVehicle = componentInChildren.transform.GetComponent<DrivableVehicle>();
				DrivableMotorcycle drivableMotorcycle = componentInChildren.GetComponent<DrivableMotorcycle>();
				if (drivableVehicle!=null)
				{
					DrivableCar drivableCar = drivableVehicle as DrivableCar;
					if ((bool)drivableCar)
					{
						WheelCollider[] wheels = drivableCar.wheels;
						foreach (WheelCollider wheelCollider in wheels)
						{
							wheelCollider.brakeTorque = 0f;
						}
					}
					PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.KickCar);
					currentMultipler = multipliers.car;
				}
				if (drivableMotorcycle!=null)
				{
					if (drivableMotorcycle.DummyDriver !=null)
					{
						drivableMotorcycle.MainRigidbody.constraints = RigidbodyConstraints.None;
						drivableMotorcycle.DummyDriver.DropRagdoll(player, drivableMotorcycle.transform.up, false, false);
					}
					currentMultipler = multipliers.motorcycle;
				}
				PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.KickCar);
				Push(componentInChildren, currentMultipler);
			}
			else
			{
				PseudoDynamicObject pseudoDynamicObject = lastTarget.GetComponent<PseudoDynamicObject>();
				if (pseudoDynamicObject!=null)
				{
					pseudoDynamicObject.ReplaceOnDynamic();
					componentInChildren = pseudoDynamicObject.GetComponent<Rigidbody>();
					PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.KickObj);
					Push(componentInChildren, multipliers.PDO);
				}
				else
				{
					isInKickState = false;
				}
			}
		}

		private void StaminaProcessing(bool spendStamina = false)
		{
			if (player.stats.stamina.Current >= (float)staminaForKick)
			{
				isEnoughStamina = true;
				if (spendStamina)
				{
					player.stats.stamina.SetAmount(-staminaForKick);
				}
			}
			else
			{
				isEnoughStamina = false;
			}
		}
	}
}
