using Game.Character;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Enemy
{
	public class RagdollWakeUper : MonoBehaviour
	{
		public class BodyPart
		{
			public Transform Transform;

			public Vector3 StoredPosition;

			public Quaternion StoredRotation;
		}

		private const float getUpTransTime = 0.05f;

		private const int ForceWakeupTime = 10;

		private const int PushRigidbodiesRadius = 10;

		private const int PushForce = 500;

		private const int PeriodicDamage = 10;

		private const int ResistTimeout = 3;

		private static int BigDynamicLayerNumber = -1;

		public float BlendTime = 0.5f;

		public LayerMask RayLayerMask;

		public Animator WakeUpAnimController;

		public float WakeUpFromChestAnimLength;

		public float WakeUpFromBackAnimationLength;

		[HideInInspector]
		public RagdollState CurrentState = RagdollState.Ragdolled;

		private float rdEndTime = -100f;

		private Vector3 rdHipPosition;

		private Vector3 rdHeadPosition;

		private Vector3 rdFeetPosition;

		private readonly List<BodyPart> bodyParts = new List<BodyPart>();

		private Animator anim;

		private Transform rootTransform;

		private GameObject mainObject;

		private HitEntity mainHitEntity;

		private Human mainHuman;

		private HumanoidStatusNPC humanoidNPC;

		private float currentReplaceTime;

		private RagdollStatus rdStatus;

		private float ragDollDestroyTime;

		private float knockdownTime;

		private float resistTime;

		public HitEntity OriginHitEntity => mainHitEntity;

		public bool IsPlayer()
		{
			return mainHuman;
		}

		public void SetRagdollWakeUpStatus(bool wakeUp)
		{
			if (wakeUp)
			{
				var parentPost = transform.parent.position;
				parentPost.y = anim.transform.position.y;
				anim.transform.position = parentPost;
				transform.parent.localPosition = Vector3.zero;
				anim.Rebind();
				if (CurrentState == RagdollState.Ragdolled)
				{
					rdEndTime = Time.time;
					anim.enabled = true;
					CurrentState = RagdollState.BlendToAnim;
					foreach (BodyPart bodyPart in bodyParts)
					{
						if (!(bodyPart.Transform == null))
						{
							bodyPart.StoredRotation = bodyPart.Transform.rotation;
							bodyPart.StoredPosition = bodyPart.Transform.position;
						}
					}
					rdFeetPosition = 0.5f * (anim.GetBoneTransform(HumanBodyBones.LeftToes).position + anim.GetBoneTransform(HumanBodyBones.RightToes).position);
					rdHeadPosition = anim.GetBoneTransform(HumanBodyBones.Head).position;
					rdHipPosition = anim.GetBoneTransform(HumanBodyBones.Hips).position;
					Vector3 forward = anim.GetBoneTransform(HumanBodyBones.Hips).forward;
					if (forward.y > 0f)
					{
						currentReplaceTime = WakeUpFromBackAnimationLength;
						anim.SetBool("GetUpFromBack", value: true);
					}
					else
					{
						currentReplaceTime = WakeUpFromChestAnimLength;
						anim.SetBool("GetUpFromChest", value: true);
					}
					knockdownTime = Time.time;

				}
			}
			else if (CurrentState == RagdollState.Animated)
			{
				StopAllCoroutines();
				rdStatus.CheckWakeUpAbility();
				anim.enabled = false;
				CurrentState = RagdollState.Ragdolled;
			}
		}

		private IEnumerator ReplaceOnNpcWithDelay(float time)
		{
			yield return new WaitForSeconds(time);
			DeInitRagdoll(mainObjectDead: false);
		}

		public void Init(GameObject rootObject, float maxHp, float currentHp, Defence newDefence, Faction newFaction)
		{
			anim = GetComponentInParent<Animator>();
			anim.runtimeAnimatorController = WakeUpAnimController.runtimeAnimatorController;
			anim.Rebind();
			anim.enabled = false;
			rootTransform = anim.gameObject.transform;
			mainObject = rootObject;
			mainHitEntity = rootObject.GetComponent<HitEntity>();
			mainHuman = (mainHitEntity as Human);
			humanoidNPC = (mainHitEntity as HumanoidStatusNPC);
			ragDollDestroyTime = ((!mainHuman) ? humanoidNPC.RagdollDestroyTime : mainHuman.DestroyTime);
			CurrentState = RagdollState.Ragdolled;
			rdStatus = GetComponent<RagdollStatus>();
			rdStatus.Init(maxHp, currentHp, newDefence, rootTransform.gameObject, newFaction);
			rdStatus.CheckWakeUpAbility();
			knockdownTime = Time.time;
			Transform[] componentsInChildren = rootTransform.GetComponentsInChildren<Transform>();
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				if (!transform.gameObject.CompareTag("Untweakable"))
				{
					BodyPart bodyPart = new BodyPart();
					bodyPart.Transform = transform;
					bodyParts.Add(bodyPart);
				}
			}
		}

		public void OnHealthChange(float amount)
		{
			if ((bool)(mainHitEntity as Player))
			{
				mainHitEntity.Health.Change(0f - amount);
			}
			else
			{
				SetRagdollWakeUpStatus(wakeUp: false);
			}
		}

		public void DeInitRagdoll(bool mainObjectDead, bool callOnDieEvent = true, bool instantly = false)
		{
			if (!mainObject)
			{
				return;
			}
			Vector3 position = (!mainObjectDead && !instantly) ? rootTransform.position : anim.GetBoneTransform(HumanBodyBones.Hips).position;
			position.y += .1f;
			mainObject.transform.position = position;
			mainObject.transform.rotation = rootTransform.rotation;
			anim.runtimeAnimatorController = null;
			anim.enabled = false;
			rdStatus.DeInit();
			Player player = mainHuman as Player;
			if (!mainObjectDead)
			{
				mainHitEntity.Health.Current = rdStatus.Health.Current;
				if ((bool)mainHuman)
				{
					mainHuman.ClearCurrentRagdoll();
					if (player != null)
					{
						if (!CameraManager.Instance.GetCurrentCameraMode().Equals(CameraManager.Instance.ActivateModeOnStart))
						{
							CameraManager.Instance.ResetCameraMode();
						}
						CameraManager.Instance.SetCameraTarget(player.gameObject.transform);
						CameraManager.Instance.GetCurrentCameraMode().SetCameraConfigMode("Default");
						player.ResetRotation();
						player.enabled = true;
						PlayerManager.Instance.AnimationController.ResetCollisionNormal();
					}
					mainHuman.CheckReloadOnWakeUp();
				}
				else
				{
					PoolManager.Instance.ReturnToPool(rootTransform.gameObject);
				}
				mainObject.SetActive(value: true);
			}
			else
			{
				if (callOnDieEvent)
				{
					mainHitEntity.OnDieEventCaller();
				}
				if (ragDollDestroyTime > 0f)
				{
					if (player != null)
					{
						player.DestroyRagdollTime = Time.time + ragDollDestroyTime;
					}
					else
					{
						PoolManager.Instance.ReturnToPoolWithDelay(rootTransform.gameObject, ragDollDestroyTime);
					}
				}
			}
			mainObject = null;
			SetupRagdollMark(base.transform.parent.gameObject);
			PoolManager.Instance.ReturnToPool(base.gameObject);
		}

		public void TryWakeup()
		{
			if (!IsPlayer() || !(Time.time > knockdownTime + 10f))
			{
				return;
			}
			LayerMask mask = 1 << BigDynamicLayerNumber;
			Collider[] array = Physics.OverlapSphere(base.transform.position, 10f, mask);
			foreach (Collider collider in array)
			{
				Rigidbody rigidbody = collider.GetComponent<Rigidbody>();
				if (rigidbody == null)
				{
					rigidbody = collider.GetComponentInParent<Rigidbody>();
				}
				if (rigidbody != null)
				{
					rigidbody.AddExplosionForce(500f, base.transform.position, 10f);
				}
			}
			rdStatus.OnHit(DamageType.Instant, null, 10f, base.transform.position, Vector3.zero, 0f);
			if (!(Time.time > resistTime + 3f))
			{
				return;
			}
			for (int j = 0; j < bodyParts.Count; j++)
			{
				BodyPart bodyPart = bodyParts[j];
				if ((bool)bodyPart.Transform.GetComponent<Rigidbody>())
				{
					bodyPart.Transform.localRotation = Quaternion.Slerp(bodyPart.Transform.localRotation, Quaternion.identity, 0.5f);
				}
				resistTime = Time.time;
			}
		}

		public void Drowning(float waterDepth)
		{
			if (humanoidNPC == null)
			{
				return;
			}
			Transform boneTransform = anim.GetBoneTransform(HumanBodyBones.Hips);
			SetRagdollWakeUpStatus(wakeUp: false);
			if ((bool)boneTransform)
			{
				RagdollDrowning ragdollDrowning = boneTransform.GetComponent<RagdollDrowning>() ?? boneTransform.gameObject.AddComponent<RagdollDrowning>();
				PoolManager.Instance.AddBeforeReturnEvent(this, delegate
				{
					UnityEngine.Object.Destroy(ragdollDrowning);
				});
				if ((bool)ragdollDrowning)
				{
					ragdollDrowning.Init(boneTransform, waterDepth);
				}
			}
		}

		private void Awake()
		{
			BigDynamicLayerNumber = LayerMask.NameToLayer("BigDynamic");
		}

		private void LateUpdate()
		{
			if (CurrentState != RagdollState.BlendToAnim)
			{
				return;
			}
			if (Time.time <= rdEndTime + 0.05f)
			{
				Vector3 b = rdHipPosition - anim.GetBoneTransform(HumanBodyBones.Hips).position;
				Vector3 vector = rootTransform.position + b;
				vector.y += 0.2f;
				RaycastHit[] array = Physics.RaycastAll(new Ray(vector, Vector3.down * 0.5f), (int)RayLayerMask);
				vector.y = -1000f;
				RaycastHit[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					RaycastHit raycastHit = array2[i];
					if (!raycastHit.transform.IsChildOf(rootTransform.transform))
					{
						float y = vector.y;
						Vector3 point = raycastHit.point;
						vector.y = Mathf.Max(y, point.y);
					}
				}
				rootTransform.position = vector;
				Vector3 vector2 = rdHeadPosition - rdFeetPosition;
				vector2.y = 0f;
				Vector3 b2 = 0.5f * (anim.GetBoneTransform(HumanBodyBones.LeftFoot).position + anim.GetBoneTransform(HumanBodyBones.RightFoot).position);
				Vector3 vector3 = anim.GetBoneTransform(HumanBodyBones.Head).position - b2;
				vector3.y = 0f;
				rootTransform.rotation *= Quaternion.FromToRotation(vector3.normalized, vector2.normalized);
			}
			float value = 1f - (Time.time - rdEndTime - 0.05f) / BlendTime;
			value = Mathf.Clamp01(value);
			foreach (BodyPart bodyPart in bodyParts)
			{
				if (!(bodyPart.Transform == null) && bodyPart.Transform != rootTransform)
				{
					if (bodyPart.Transform == anim.GetBoneTransform(HumanBodyBones.Hips))
					{
						bodyPart.Transform.position = Vector3.Lerp(bodyPart.Transform.position, bodyPart.StoredPosition, value);
					}
					bodyPart.Transform.rotation = Quaternion.Slerp(bodyPart.Transform.rotation, bodyPart.StoredRotation, value);
				}
			}
			if (value == 0f)
			{
				Transform transform = rootTransform;
				Vector3 eulerAngles = rootTransform.eulerAngles;
				transform.eulerAngles = new Vector3(0f, eulerAngles.y, 0f);
				anim.SetBool("GetUpFromBack", value: false);
				anim.SetBool("GetUpFromChest", value: false);
				CurrentState = RagdollState.Animated;
				StartCoroutine(ReplaceOnNpcWithDelay(currentReplaceTime));
			}
		}

		public void SetupRagdollMark(GameObject objectForAttaching)
		{
			RagdollMark component = objectForAttaching.GetComponent<RagdollMark>();
			if (!component)
			{
				objectForAttaching.AddComponent<RagdollMark>();
			}
		}
	}
}
