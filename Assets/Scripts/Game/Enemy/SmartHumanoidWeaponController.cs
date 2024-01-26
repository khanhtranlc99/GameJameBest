using Game.Character.CharacterController;
using Game.Character.CharacterController.Enums;
using Game.GlobalComponent;
using Game.Weapons;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Enemy
{
	public class SmartHumanoidWeaponController : MonoBehaviour
	{
		public bool DebugLog;

		public AudioSource WeaponAudio;

		public List<Weapon> Weapons = new List<Weapon>();

		private HitEntity controllerOwner;

		private WeaponForSmartHumanoidNPC usingWeapons;

		private WeaponForSmartHumanoidNPC currentInitedWeapons;

		private Weapon currentWeapon;

		private readonly AttackTargetParameters attackTargetParameters = new AttackTargetParameters();

		private AttackState attackState = new AttackState();

		public Weapon CurrentWeapon => currentWeapon;

		public WeaponArchetype CurrentArchetype => currentWeapon.Archetype;

		private void Update()
		{
			if (DebugLog)
			{
				UnityEngine.Debug.Log(CurrentWeapon.WeaponNameInList);
			}
		}

		public void Init(BaseNPC controlledNPC)
		{
			controllerOwner = controlledNPC.StatusNpc;
			usingWeapons = controlledNPC.SpecificNpcLinks.UsingWeapons;
			currentInitedWeapons = PoolManager.Instance.GetFromPool(usingWeapons);
			currentInitedWeapons.transform.parent = base.transform;
			currentInitedWeapons.transform.localPosition = Vector3.zero;
			currentInitedWeapons.transform.localEulerAngles = Vector3.zero;
			currentInitedWeapons.Init(controlledNPC, this);
			if (Weapons.Count == 1)
			{
				ChangeWeapon(Weapons[0]);
			}
			else if (!ActivateWeaponByType(WeaponArchetype.Ranged))
			{
				ActivateWeaponByType(WeaponArchetype.Melee);
			}
		}

		public void DeInit()
		{
			controllerOwner = null;
			DeInitWeapon();
			currentInitedWeapons.DeInit();
			PoolManager.Instance.ReturnToPool(currentInitedWeapons);
			currentInitedWeapons = null;
		}

		public void Attack(HitEntity entity)
		{
			attackTargetParameters.AttackMethod = AttackMethod.AttackEntity;
			attackTargetParameters.Victim = entity;
		}

		public void MeleeWeaponAttack(int attackState)
		{
			MeleeWeapon meleeWeapon = currentWeapon as MeleeWeapon;
			if (meleeWeapon != null)
			{
				meleeWeapon.MeleeAttack(attackState);
			}
		}

		public void AttackWithWeapon()
		{
			Invoke("InvokedAttack", 0f);
		}

		public bool ActivateWeaponByType(WeaponArchetype newWeaponArchetype)
		{
			if (currentWeapon != null && CurrentArchetype == newWeaponArchetype)
			{
				return true;
			}
			if (Weapons.Count <= 1)
			{
				return false;
			}
			int num = -1;
			for (int i = 0; i < Weapons.Count; i++)
			{
				if (Weapons[i].Archetype == newWeaponArchetype)
				{
					num = i;
					RangedWeapon rangedWeapon = Weapons[i] as RangedWeapon;
					if (!(rangedWeapon != null) || rangedWeapon.AmmoCount > 0)
					{
						break;
					}
					num = -1;
				}
			}
			if (num == -1)
			{
				return false;
			}
			ChangeWeapon(Weapons[num]);
			return true;
		}

		public bool CheckCurrentWeaponAmmoExist()
		{
			RangedWeapon rangedWeapon = currentWeapon as RangedWeapon;
			if (rangedWeapon != null)
			{
				return rangedWeapon.AmmoCount > 0;
			}
			return true;
		}

		public AttackState UpdateAttackState(bool attack)
		{
			attackState.MeleeAttackState = MeleeAttackState.None;
			attackState.RangedAttackState = RangedAttackState.None;
			attackState.RangedWeaponType = RangedWeaponType.None;
			attackState.MeleeWeaponType = MeleeWeapon.MeleeWeaponType.Hand;
			attackState.Aim = false;
			attackState.CanAttack = false;
			RangedWeapon rangedWeapon = currentWeapon as RangedWeapon;
			if ((bool)rangedWeapon)
			{
				attackState.RangedWeaponType = rangedWeapon.RangedWeaponType;
			}
			if (!attack)
			{
				return attackState;
			}
			MeleeWeapon meleeWeapon = currentWeapon as MeleeWeapon;
			if ((bool)rangedWeapon)
			{
				attackState.Aim = true;
				attackState.RangedWeaponType = rangedWeapon.RangedWeaponType;
				attackState.RangedAttackState = rangedWeapon.GetRangedAttackState();
				if (attackState.RangedAttackState == RangedAttackState.Shoot)
				{
					attackState.CanAttack = true;
				}
			}
			else if ((bool)meleeWeapon)
			{
				attackState.MeleeWeaponType = meleeWeapon.MeleeType;
				attackState.Aim = false;
				if (meleeWeapon.IsOnCooldown)
				{
					attackState.MeleeAttackState = MeleeAttackState.Idle;
				}
				else
				{
					attackState.MeleeAttackState = meleeWeapon.GetMeleeAttackState();
					if (attackState.MeleeAttackState != MeleeAttackState.None && attackState.MeleeAttackState != MeleeAttackState.Idle)
					{
						attackState.CanAttack = true;
					}
				}
			}
			return attackState;
		}

		public void HideWeapon()
		{
			if ((bool)currentWeapon)
			{
				currentWeapon.gameObject.SetActive(value: false);
			}
		}

		public void ShowWeapon()
		{
			if ((bool)currentWeapon)
			{
				currentWeapon.gameObject.SetActive(value: true);
			}
		}

		private void InvokedAttack()
		{
			if ((bool)currentWeapon)
			{
				currentWeapon.Attack(controllerOwner, attackTargetParameters.Victim);
			}
		}

		private void OnEnable()
		{
			CheckReloadOnWakeUp();
		}

		private void CheckReloadOnWakeUp()
		{
			RangedWeapon rangedWeapon = currentWeapon as RangedWeapon;
			if (rangedWeapon != null && rangedWeapon.AmmoInCartridgeCount == 0)
			{
				rangedWeapon.RechargeFinish();
			}
		}

		private void ChangeWeapon(Weapon newWeapon)
		{
			if (!(currentWeapon == newWeapon))
			{
				DeInitWeapon();
				InitWeapon(newWeapon);
			}
		}

		private void InitWeapon(Weapon newWeapon)
		{
			currentWeapon = newWeapon;
			currentWeapon.gameObject.SetActive(value: true);
			currentWeapon.Init();
			currentWeapon.PerformAttackEvent = PerformAttackEvent;
			currentWeapon.InflictDamageEvent = InflictDamageEvent;
			RangedWeapon rangedWeapon = currentWeapon as RangedWeapon;
			if (rangedWeapon != null)
			{
				rangedWeapon.RechargeStartedEvent = RechargeStartedEvent;
			}
		}

		private void DeInitWeapon()
		{
			if (!(currentWeapon == null))
			{
				currentWeapon.PerformAttackEvent = null;
				currentWeapon.InflictDamageEvent = null;
				currentWeapon.DeInit();
				RangedWeapon rangedWeapon = currentWeapon as RangedWeapon;
				if ((bool)rangedWeapon)
				{
					rangedWeapon.RechargeStartedEvent = null;
				}
				currentWeapon.gameObject.SetActive(value: false);
				currentWeapon = null;
			}
		}

		private void PerformAttackEvent(Weapon weapon)
		{
			weapon.PlayAttackSound(WeaponAudio);
			RangedWeapon rangedWeapon = weapon as RangedWeapon;
			if (rangedWeapon != null)
			{
				MakeShotFlash(rangedWeapon.Muzzle, rangedWeapon.ShotSfx);
			}
		}

		private void MakeShotFlash(Transform muzzle, ShotSFXType type)
		{
			if ((bool)WeaponManager.Instance && (bool)muzzle)
			{
				WeaponManager.Instance.StartShootSFX(muzzle, type);
			}
		}

		private void InflictDamageEvent(Weapon weapon, HitEntity owner, HitEntity victim, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			victim.OnHit(weapon.WeaponDamageType, owner, weapon.Damage, hitPos, hitVector, defenceReduction);
		}

		private void RechargeStartedEvent(Weapon weapon)
		{
			RangedWeapon rangedWeapon = weapon as RangedWeapon;
			if (rangedWeapon != null)
			{
				rangedWeapon.PlayRechargeSound(WeaponAudio);
			}
		}
	}
}
