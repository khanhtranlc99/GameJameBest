using Game.Character;
using Game.Character.CharacterController;
using Game.Character.CollisionSystem;
using Game.Character.Extras;
using Game.Effects;
using Game.GlobalComponent;
using Game.GlobalComponent.HelpfulAds;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Vehicle
{
	public class VehicleStatus : HitEntity
	{
		public enum SmokeState
		{
			None,
			Low,
			Medium,
			Critical
		}

		[Serializable]
		public class Smoke
		{
			public SmokeState SmokeState;

			public GameObject SmokeParticle;
		}

		private const float DeadVelocity = 250000f;

		private const int CorpseVelocityPower = 10;

		private static readonly IDictionary<float, SmokeState> SmokeRates = new Dictionary<float, SmokeState>
		{
			{
				0.7f,
				SmokeState.None
			},
			{
				0.5f,
				SmokeState.Low
			},
			{
				0.2f,
				SmokeState.Medium
			},
			{
				0f,
				SmokeState.Critical
			}
		};

		[Separator("Vehicle specific variables")]
		public bool IsArmored;

		public Collider[] AdditionalColliders;

		public GameObject DestroyReplace;

		public Vector3 DestroyOffset;

		public GameObject SmokePoint;

		public Smoke[] CarSmokes;

		public GameObject Explosion;

		public float ReplacementDestroyTime = 5f;

		[HideInInspector]
		public DrivableVehicle rootDrivableVehicle;

		private SmokeState currentSmokeState;

		private GameObject rootObject;

		private GameObject destroyedObject;

		private GameObject currentSmoke;

		private ParticleSystem[] currentSmokeParticleSystems;

		private bool burned;

		private List<IgnoreCollision> ignoreCollisionsInstance = new List<IgnoreCollision>();

		public float HealthProcent => (float)Math.Round(Health.Current / Health.Max, 1);

		protected override void Start()
		{
			Initialization(setUpHealth: false);
		}

		private void OnEnable()
		{
			if (!rootObject)
			{
				rootDrivableVehicle = GetComponentInParent<DrivableVehicle>();
				rootObject = rootDrivableVehicle.gameObject;
			}
			PoolManager.Instance.AddBeforeReturnEvent(rootObject, delegate
			{
				Dead = false;
				burned = false;
				Health.Setup();
				EntityManager.Instance.Register(this);
				UpdateSmokeState();
			});
		}

		public override void Initialization(bool setUpHealth = true)
		{
			base.Initialization(setUpHealth);
			if (setUpHealth)
			{
				Health.Current = Health.Max;
				UpdateSmokeState();
			}
			Dead = false;
			if (MainCollider == null)
			{
				MainCollider = GetComponent<Collider>();
			}
		}

		public HitEntity GetVehicleDriver()
		{
			return rootDrivableVehicle.CurrentDriver;
		}

		public override void OnHit(DamageType damageType, HitEntity owner, float damage, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			bool flag = PlayerInteractionsManager.Instance.inVehicle && rootDrivableVehicle == PlayerInteractionsManager.Instance.LastDrivableVehicle;
			bool flag2 = CalculateSmokeState((float)Math.Max(Math.Round((Health.Current - damage) / Health.Max, 1), 0.10000000149011612)) == SmokeState.Critical;
			if (flag)
			{
				if (!PlayerManager.Instance.Player.IsTransformer && HelpfullAdsManager.Instance != null && flag2)
				{
					Action<bool> helpCallback = delegate(bool helpAccepted)
					{
						Immortal = false;
						if (!helpAccepted)
						{
							OnDie();
						}
					};
					Immortal = true;
					HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.VehicleRepair, helpCallback);
				}
				base.OnHit(damageType, owner, damage, hitPos, hitVector, defenceReduction);
				if (!PlayerManager.Instance.Player.IsTransformer)
				{
					UpdateSmokeState();
					if (currentSmokeState == SmokeState.Critical)
					{
						IgniteCar();
					}
				}
			}
			else
			{
				base.OnHit(damageType, owner, damage, hitPos, hitVector, defenceReduction);
				UpdateSmokeState();
				if (currentSmokeState == SmokeState.Critical)
				{
					IgniteCar();
				}
				if ((bool)(owner as Player))
				{
					WorsenRelations();
				}
			}
		}

		public void Repair(float amount)
		{
			Health.Change(amount);
			UpdateSmokeState();
			if (burned)
			{
				VehicleController componentInChildren = rootObject.GetComponentInChildren<VehicleController>();
				if (componentInChildren != null)
				{
					componentInChildren.EnableEngine();
				}
				StopAllCoroutines();
				burned = false;
			}
		}

		public void SetCameraIgnoreCollision()
		{
			IgnoreCollision item = base.gameObject.AddComponent<IgnoreCollision>();
			ignoreCollisionsInstance.Add(item);
			Collider[] additionalColliders = AdditionalColliders;
			foreach (Collider collider in additionalColliders)
			{
				item = collider.gameObject.AddComponent<IgnoreCollision>();
				ignoreCollisionsInstance.Add(item);
			}
			PoolManager.Instance.AddBeforeReturnEvent(rootObject, delegate
			{
				RemoveCameraIgnoreCollision();
			});
		}

		public void RemoveCameraIgnoreCollision()
		{
			foreach (IgnoreCollision item in ignoreCollisionsInstance)
			{
				UnityEngine.Object.Destroy(item);
			}
			ignoreCollisionsInstance.Clear();
		}

		private void IgniteCar()
		{
			if (base.gameObject.activeInHierarchy && !burned)
			{
				burned = true;
				VehicleController componentInChildren = rootObject.GetComponentInChildren<VehicleController>();
				if (componentInChildren != null)
				{
					componentInChildren.DisableEngine();
				}
				StartCoroutine(CarBurned());
			}
		}

		private IEnumerator CarBurned()
		{
			WaitForSeconds waitForSeconds = new WaitForSeconds(1f);
			while (true)
			{
				OnHit(DamageType.Instant, null, 10f, base.transform.position, base.transform.up, 0f);
				yield return waitForSeconds;
			}
		}

		private void WorsenRelations()
		{
			if (GetVehicleDriver() != null)
			{
				FactionsManager.Instance.PlayerAttackHuman(GetVehicleDriver());
			}
		}

		private void DeInitOnExplosion()
		{
			rootObject.GetComponent<DrivableVehicle>().GetOut();
			rootObject.GetComponent<DrivableVehicle>().DeInit();
			CarSpecific componentInChildren = rootObject.GetComponentInChildren<CarSpecific>();
			if (componentInChildren != null)
			{
				PoolManager.Instance.ReturnToPool(componentInChildren.gameObject);
			}
			if ((bool)currentSmoke)
			{
				PoolManager.Instance.ReturnToPool(currentSmoke);
			}
			Player componentInChildren2 = rootObject.GetComponentInChildren<Player>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.transform.parent = null;
				componentInChildren2.transform.position += -rootObject.transform.right;
				componentInChildren2.Die();
				if (!componentInChildren2.IsTransformer)
				{
					GameObject currentRagdoll = componentInChildren2.GetCurrentRagdoll();
					if (currentRagdoll != null)
					{
						Rigidbody[] componentsInChildren = currentRagdoll.GetComponentsInChildren<Rigidbody>();
						Rigidbody[] array = componentsInChildren;
						foreach (Rigidbody rigidbody in array)
						{
							rigidbody.AddForce(-rootObject.transform.right * 10f);
						}
					}
				}
				PlayerInteractionsManager.Instance.ResetGetInOutButtons();
			}
			PoolManager.Instance.ReturnToPool(rootObject);
		}

		private SmokeState CalculateSmokeState(float currentHpProcent)
		{
			foreach (KeyValuePair<float, SmokeState> smokeRate in SmokeRates)
			{
				if (currentHpProcent > smokeRate.Key)
				{
					return smokeRate.Value;
				}
			}
			return currentSmokeState;
		}

		private GameObject GetCurrentSmoke(SmokeState newSmokeState)
		{
			Smoke[] carSmokes = CarSmokes;
			foreach (Smoke smoke in carSmokes)
			{
				if (smoke.SmokeState == newSmokeState)
				{
					return smoke.SmokeParticle;
				}
			}
			return null;
		}

		protected void UpdateSmokeState()
		{
			if (!SmokePoint)
			{
				return;
			}
			SmokeState smokeState = currentSmokeState;
			currentSmokeState = CalculateSmokeState(HealthProcent);
			if (currentSmokeState == smokeState)
			{
				return;
			}
			if (currentSmoke != null)
			{
				PoolManager.Instance.ReturnToPool(currentSmoke);
				currentSmoke = null;
				ParticleSystem[] array = currentSmokeParticleSystems;
				foreach (ParticleSystem particleSystem in array)
				{
					particleSystem.Stop();
				}
			}
			GameObject gameObject = GetCurrentSmoke(currentSmokeState);
			if (gameObject != null)
			{
				currentSmoke = PoolManager.Instance.GetFromPool(gameObject);
				currentSmoke.transform.parent = SmokePoint.transform;
				currentSmoke.transform.localPosition = Vector3.zero;
				currentSmoke.transform.localEulerAngles = Vector3.zero;
				currentSmokeParticleSystems = currentSmoke.GetComponentsInChildren<ParticleSystem>();
				ParticleSystem[] array2 = currentSmokeParticleSystems;
				foreach (ParticleSystem particleSystem2 in array2)
				{
					particleSystem2.Play();
				}
			}
		}

		protected override void SpecifficEffect(Vector3 hitPos)
		{
			if ((bool)SparksHitEffect.Instance)
			{
				SparksHitEffect.Instance.Emit(hitPos);
			}
		}

		protected override void OnDie()
		{
			bool flag = PlayerInteractionsManager.Instance.inVehicle && rootDrivableVehicle == PlayerInteractionsManager.Instance.LastDrivableVehicle;
			if (HelpfullAdsManager.Instance != null && PlayerInteractionsManager.Instance.Player.IsTransformer && flag)
			{
				HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.Heal, OnDieAction);
			}
			else
			{
				OnDieAction(notDie: false);
			}
		}

		protected void OnDieAction(bool notDie)
		{
			if (notDie)
			{
				return;
			}
			if ((bool)GetVehicleDriver())
			{
				GetVehicleDriver().LastHitOwner = LastHitOwner;
				if ((bool)(LastHitOwner as Player))
				{
					FactionsManager.Instance.CommitedACrime();
				}
				GetVehicleDriver().Die(LastHitOwner);
			}
			base.OnDie();
			if ((bool)DestroyReplace && (bool)Explosion)
			{
				GameObject fromPool = PoolManager.Instance.GetFromPool(DestroyReplace);
				fromPool.transform.position = rootObject.transform.position + DestroyOffset;
				fromPool.transform.rotation = rootObject.transform.rotation;
				Rigidbody component = fromPool.GetComponent<Rigidbody>();
				if ((bool)component)
				{
					component.AddForce(Vector3.up * 250000f);
					component.AddForce(LastHitVector * LastDamage);
					component.AddTorque(new Vector3(UnityEngine.Random.Range(-250000f, 250000f), UnityEngine.Random.Range(-250000f, 250000f), UnityEngine.Random.Range(-250000f, 250000f)));
				}
				destroyedObject = fromPool;
				DestroyedCar component2 = destroyedObject.GetComponent<DestroyedCar>();
				if (component2 != null)
				{
					component2.Init();
					component2.DeInitWithDelay(ReplacementDestroyTime);
				}
				else
				{
					PoolManager.Instance.ReturnToPoolWithDelay(destroyedObject, ReplacementDestroyTime);
				}
				GameObject fromPool2 = PoolManager.Instance.GetFromPool(Explosion);
				fromPool2.transform.position = rootObject.transform.position;
				fromPool2.GetComponent<Explosion>().Init(LastHitOwner, new GameObject[2]
				{
					destroyedObject,
					base.gameObject
				});
				DeInitOnExplosion();
			}
		}
	}
}
