using Game.Character;
using Game.Character.CharacterController;
using Game.Character.CharacterController.Enums;
using Game.Character.Extras;
using Game.Character.Stats;
using Game.GlobalComponent.HelpfulAds;
using System;
using System.Collections;
using UnityEngine;

namespace Game.Weapons
{
	public class RangedWeapon : Weapon
	{
		private const float SaveAmmoTime = 5f;

		private const int TimeToDisableBigGunModel = 1;

		public const string MuzzleName = "Muzzle";

		public RangedWeaponType RangedWeaponType;

		public bool BigGun;

		public ShotSFXType ShotSfx;

		public GameObject Tracer;

		public AudioClip RechargeSound;

		public int ShotgunBulletsCount = 8;

		public float RechargeDelay = 1f;

		[Tooltip("Тангенс угла разброса")]
		public float ScatterAngle;

		[Tooltip("Сила отдачи")]
		public float FireKickPower;

		public bool IsFiniteAmmo = true;

		public float ShootAlarmRange = 10f;

		public Transform Muzzle;

		public bool IgnoreMuzzleDirection;

		public bool IsRecharging;

		[Tooltip("Если указан общий контейнер для патронов, то патроны будут тратиться из контейнера.")]
		public JointAmmoContainer AmmoContainer;

		[Header("ForNpcOnly")]
		public float RangedFireDistanceNPC = 15f;

		public int AmmoCartridgeCapacity = 1;

		public int AmmoOutOfCartridgeCount;

		protected int ammoInCartridgeCount;

		[Tooltip("Only for main character")]
		[Range(0f, float.PositiveInfinity)]
		[Separator("Customization")]
		public float EnergyCost;

		[HideInInspector]
		public Vector3 LastHitDirectionVector;

		[HideInInspector]
		public Vector3 LastHitPosition;

		protected float LastGetStateTime;

		protected static LayerMask characterLayerMask;

		private static int charactetLayerNumber = -1;

		private float rechargeStartTime;

		private float saveAmmoTimer = 5f;

		private bool scatterCalculateFromMuzzle;

		private Player player;

		private float maxScatterRadius;

		public AttackEvent AmmoChangedEvent;

		public AttackEvent RechargeStartedEvent;

		public AttackEvent RechargeSuccessfullEvent;

		public AttackEvent AfterAttackEvent;

		public virtual int AmmoInCartridgeCount
		{
			get
			{
				return GetAmmoInCartrige();
			}
			set
			{
				ammoInCartridgeCount = value;
			}
		}

		public virtual int AmmoCount
		{
			get
			{
				int num = (!AmmoContainer) ? AmmoOutOfCartridgeCount : AmmoContainer.AmmoCount;
				return ammoInCartridgeCount + num;
			}
		}

		public virtual string AmmoCountText
		{
			get
			{
				int num = (!AmmoContainer) ? AmmoOutOfCartridgeCount : AmmoContainer.AmmoCount;
				if (IsFiniteAmmo)
				{
					return ammoInCartridgeCount + " / " + num;
				}
				return ammoInCartridgeCount.ToString();
			}
		}

		public bool IsCartridgeEmpty => AmmoInCartridgeCount <= 0;

		public Vector3 ScatterVector => GetScatterVector();

		public override void Init()
		{
			base.Init();
			if (charactetLayerNumber == -1)
			{
				charactetLayerNumber = LayerMask.NameToLayer("Character");
				characterLayerMask = 1 << charactetLayerNumber;
			}
			player = (base.WeaponOwner as Player);
			if ((bool)player)
			{
				AmmoContainer = AmmoManager.Instance.GetContainer(AmmoType);
			}
			if (IsCartridgeEmpty)
			{
				RechargeFinish();
			}
		}

		public override void DeInit()
		{
			AmmoChangedEvent = null;
			RechargeStartedEvent = null;
			Discharge();
			base.DeInit();
		}

		protected override void Start()
		{
			base.Start();
			maxScatterRadius = Mathf.Tan(ScatterAngle * ((float)Math.PI / 180f));
			if (DebugLog)
			{
				UnityEngine.Debug.Log(maxScatterRadius);
			}
			if (Muzzle == null)
			{
				Muzzle = base.transform.Find("Muzzle");
			}
			if (Muzzle == null)
			{
				UnityEngine.Debug.LogError("Can't find Muzzle");
			}
			Archetype = WeaponArchetype.Ranged;
			if (WeaponObjects == null || WeaponObjects.Length <= 0)
			{
				WeaponObjects = new GameObject[1]
				{
					base.gameObject
				};
			}
		}

		private void Update()
		{
			if (!BigGun || IsRecharging)
			{
				return;
			}
			if (Time.time > LastGetStateTime + 1f)
			{
				foreach (GameObject gameObject in WeaponObjects)
				{
					gameObject.SetActive(value: false);
				}
			}
			else
			{
				foreach (GameObject gameObject2 in WeaponObjects)
				{
					gameObject2.SetActive(value: true);
				}
			}
		}

		public int GetAmmoInCartrige()
		{
			if ((bool)player && EnergyCost > 0f)
			{
				return (int)(player.stats.stamina.Current / EnergyCost);
			}
			return ammoInCartridgeCount;
		}

		public override void Attack(HitEntity owner)
		{
			if (!PrepareAttack() || !TargetManager.Instance)
			{
				return;
			}
			int num = (RangedWeaponType != RangedWeaponType.Shotgun) ? 1 : ShotgunBulletsCount;
			for (int i = 1; i <= num; i++)
			{
				Vector3 shotDirVector;
				CastResult castResult = TargetManager.Instance.ShootFromCamera(owner, GetScatterVector(), out shotDirVector, AttackDistance, this, humanoidShoot: true);
				if (castResult.TargetType == TargetType.Enemy && (bool)castResult.HitEntity)
				{
					if (InflictDamageEvent != null)
					{
						InflictDamageEvent(this, owner, castResult.HitEntity, castResult.HitPosition, castResult.HitVector, DefenceIgnorence);
					}
					PlayHitSound(castResult.HitPosition);
				}
				MakeShootTrace(shotDirVector, castResult.RayLength);
				LastHitDirectionVector = shotDirVector;
				LastHitPosition = castResult.HitPosition;
				if (AfterAttackEvent != null)
				{
					AfterAttackEvent(this);
				}
				OnShootAlarm(owner, castResult.HitEntity);
			}
		}

		public override void Attack(HitEntity owner, Vector3 direction)
		{
			if (!PrepareAttack() || !TargetManager.Instance)
			{
				return;
			}
			int num = (RangedWeaponType != RangedWeaponType.Shotgun) ? 1 : ShotgunBulletsCount;
			for (int i = 1; i <= num; i++)
			{
				Vector3 vector = direction + GetScatterVector();
				CastResult castResult = (!Muzzle) ? TargetManager.Instance.ShootAt(owner, vector, AttackDistance) : TargetManager.Instance.ShootFromAt(owner, Muzzle.position, vector.normalized, AttackDistance);
				if (castResult.TargetType == TargetType.Enemy && (bool)castResult.HitEntity)
				{
					if (InflictDamageEvent != null)
					{
						InflictDamageEvent(this, owner, castResult.HitEntity, castResult.HitPosition, castResult.HitVector, DefenceIgnorence);
					}
					PlayHitSound(castResult.HitPosition);
				}
				MakeShootTrace(vector, castResult.RayLength);
				LastHitDirectionVector = vector;
				LastHitPosition = castResult.HitPosition;
				if (AfterAttackEvent != null)
				{
					AfterAttackEvent(this);
				}
				OnShootAlarm(owner, castResult.HitEntity);
			}
		}

		public override void Attack(HitEntity owner, HitEntity victim)
		{
			if (!(victim == null))
			{
				if ((bool)Muzzle)
				{
					Attack(owner, (victim.transform.position + victim.NPCShootVectorOffset - Muzzle.position).normalized);
				}
				else
				{
					Attack(owner, victim.transform.position - owner.transform.position);
				}
			}
		}

		public void SetScatterCalculateFromMuzzle()
		{
			scatterCalculateFromMuzzle = true;
		}

		protected Vector3 GetScatterVector()
		{
			if (ScatterAngle == 0f)
			{
				return Vector3.zero;
			}
			float num = UnityEngine.Random.Range(0f, 1f);
			float num2 = UnityEngine.Random.Range(0f, 360f);
			num = num * num * maxScatterRadius;
			if ((bool)player)
			{
				num *= player.stats.GetPlayerStat(StatsList.ScatterBullets);
			}
			Transform transform = (!(Muzzle != null)) ? base.WeaponOwner.transform : Muzzle;
			float d = num * Mathf.Cos(num2 * ((float)Math.PI / 180f));
			float d2 = num * Mathf.Sin(num2 * ((float)Math.PI / 180f));
			return transform.up * d2 + transform.right * d;
		}

		public void OnShootAlarm(HitEntity owner, HitEntity victim)
		{
			if ((bool)player && !(ShootAlarmRange <= 0f) && !(base.WeaponOwner == null))
			{
				Vector3 position = (!Muzzle) ? base.transform.position : Muzzle.transform.position;
				EntityManager.Instance.OverallAlarm(base.WeaponOwner, victim, position, ShootAlarmRange);
			}
		}

		public virtual RangedAttackState GetRangedAttackState()
		{
			LastGetStateTime = Time.time;
			if (base.WeaponOwner != null)
			{
				bool flag = false;
				RaycastHit hitInfo;
				Ray ray = player ? Camera.main.ScreenPointToRay(TargetManager.Instance.CrosshairStart.position) : new Ray(Muzzle.transform.position, base.WeaponOwner.transform.forward);
				if (Physics.Raycast(ray, out hitInfo, AttackDistance, characterLayerMask))
				{
					HitEntity component = hitInfo.collider.GetComponent<HitEntity>();
					if (component != null && FactionsManager.Instance.GetRelations(component.Faction, base.WeaponOwner.Faction) == Relations.Friendly)
					{
						flag = true;
					}
				}
				if (flag)
				{
					return RangedAttackState.ShootInFriendly;
				}
			}
			if (base.IsOnCooldown)
			{
				return RangedAttackState.Idle;
			}
			if (IsRecharging)
			{
				return RangedAttackState.Recharge;
			}
			if (IsCartridgeEmpty)
			{
				CheckRecharge();
				if (IsRecharging && EnergyCost == 0f)
				{
					return RangedAttackState.Recharge;
				}
				return RangedAttackState.Idle;
			}
			return RangedAttackState.Shoot;
		}

		protected bool PrepareAttack()
		{
			if (GetRangedAttackState() != RangedAttackState.Shoot)
			{
				return false;
			}
			lastAttackTime = Time.time;
			ChangeAmmo(-1);
			if (player != null)
				player.OnPlayerShootOne?.Invoke();
			if (PerformAttackEvent != null)
			{
				PerformAttackEvent(this);
			}
			return true;
		}

		protected virtual void ChangeAmmo(int amount)
		{
			if (amount > 0)
			{
				if ((bool)AmmoContainer)
				{
					AmmoContainer.AmmoCount += amount;
				}
				else
				{
					AmmoOutOfCartridgeCount += amount;
				}
			}
			else if (EnergyCost != 0f && (bool)player)
			{
				player.stats.stamina.Change(0f - EnergyCost);
			}
			else
			{
				ammoInCartridgeCount += amount;
			}
			if (AmmoChangedEvent != null)
			{
				AmmoChangedEvent(this);
			}
		}

		protected void CheckRecharge()
		{
			int num = (!AmmoContainer) ? AmmoOutOfCartridgeCount : AmmoContainer.AmmoCount;
			if (!IsRecharging && (num > 0 || !IsFiniteAmmo) && ammoInCartridgeCount < AmmoCartridgeCapacity)
			{
				StartCoroutine(Recharge());
			}
			if (IsFiniteAmmo && num <= 0 && PlayerInteractionsManager.Instance.Player.CheckIsPlayerWeapon(this) && HelpfullAdsManager.Instance != null)
			{
				if (EnergyCost != 0f)
				{
					HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.Stamina, null);
				}
				else
				{
					HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.Ammo, null);
				}
			}
		}

		public float GetRechargeStatus()
		{
			return (Time.time - rechargeStartTime) / RechargeDelay;
		}

		public void RechargeCall()
		{
			StartCoroutine(Recharge());
		}

		protected IEnumerator Recharge()
		{
			RechargeStart();
			yield return new WaitForSeconds(RechargeDelay);
			RechargeFinish();
		}

		private void RechargeStart()
		{
			IsRecharging = true;
			rechargeStartTime = Time.time;
			if (RechargeStartedEvent != null)
			{
				RechargeStartedEvent(this);
			}
			if (BigGun)
			{
				GameObject[] weaponObjects = WeaponObjects;
				foreach (GameObject gameObject in weaponObjects)
				{
					gameObject.SetActive(value: true);
				}
			}
		}

		public void RechargeFinish()
		{
			IsRecharging = false;
			if (IsFiniteAmmo)
			{
				int value = (!AmmoContainer) ? AmmoOutOfCartridgeCount : AmmoContainer.AmmoCount;
				int num = Mathf.Clamp(value, 0, AmmoCartridgeCapacity - ammoInCartridgeCount);
				ammoInCartridgeCount += num;
				if ((bool)AmmoContainer)
				{
					AmmoContainer.AmmoCount -= num;
				}
				else
				{
					AmmoOutOfCartridgeCount -= num;
				}
			}
			else
			{
				ammoInCartridgeCount = AmmoCartridgeCapacity;
			}
			if (AmmoChangedEvent != null)
			{
				AmmoChangedEvent(this);
			}
			if (RechargeSuccessfullEvent != null)
			{
				RechargeSuccessfullEvent(this);
			}
			if ((bool)player && player == PlayerManager.Instance.DefaulPlayer && EnergyCost <= 0f)
			{
				AmmoContainer.SaveAmmo();
			}
		}

		private void Discharge()
		{
			if (IsFiniteAmmo)
			{
				int num = ammoInCartridgeCount;
				if ((bool)AmmoContainer)
				{
					AmmoContainer.AmmoCount += num;
				}
				else
				{
					AmmoOutOfCartridgeCount += num;
				}
				if ((bool)player && player == PlayerManager.Instance.DefaulPlayer && EnergyCost <= 0f)
				{
					AmmoContainer.SaveAmmo();
				}
			}
			ammoInCartridgeCount = 0;
		}

		private void MakeShootTrace(Vector3 directionVector, float traceLength)
		{
			if (Tracer != null)
			{
				WeaponManager.Instance.StartTraceSfx(Muzzle, Tracer, directionVector, traceLength);
			}
		}

		public void PlayRechargeSound(AudioSource audioSource)
		{
			PlaySound(audioSource, RechargeSound);
		}
	}
}
