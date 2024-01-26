using Game.Character.CharacterController;
using Game.Character.CharacterController.Enums;
using UnityEngine;

namespace Game.Weapons
{
	public class EnergyAmmoRangedWeapon : RangedWeapon
	{
		public float EnergyPerShot = 1f;

		public override int AmmoInCartridgeCount
		{
			get
			{
				if (base.WeaponOwner != null)
				{
					Player player = base.WeaponOwner as Player;
					if ((bool)player)
					{
						return (int)(player.stats.stamina.Current / EnergyPerShot);
					}
				}
				return ammoInCartridgeCount;
			}
			set
			{
				ammoInCartridgeCount = value;
			}
		}

		public override int AmmoCount => AmmoInCartridgeCount;

		public override string AmmoCountText => AmmoInCartridgeCount.ToString();

		public override void DeInit()
		{
		}

		public override RangedAttackState GetRangedAttackState()
		{
			LastGetStateTime = Time.time;
			if (base.WeaponOwner != null)
			{
				bool flag = false;
				RaycastHit hitInfo;
				Ray ray = (base.WeaponOwner as Player) ? Camera.main.ScreenPointToRay(TargetManager.Instance.CrosshairStart.position) : new Ray(Muzzle.transform.position, base.WeaponOwner.transform.forward);
				if (Physics.Raycast(ray, out hitInfo, AttackDistance, RangedWeapon.characterLayerMask))
				{
					Human component = hitInfo.collider.GetComponent<Human>();
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
			if (base.IsCartridgeEmpty)
			{
				return RangedAttackState.Idle;
			}
			return RangedAttackState.Shoot;
		}

		protected override void ChangeAmmo(int amount)
		{
			if (base.WeaponOwner != null && amount < 0)
			{
				Player player = base.WeaponOwner as Player;
				if ((bool)player)
				{
					player.stats.stamina.Change(0f - EnergyPerShot);
				}
			}
			else
			{
				AmmoInCartridgeCount += amount;
			}
			if (AmmoChangedEvent != null)
			{
				AmmoChangedEvent(this);
			}
		}
	}
}
