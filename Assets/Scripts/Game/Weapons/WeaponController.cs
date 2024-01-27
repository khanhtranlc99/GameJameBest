using Game.Character.CameraEffects;
using Game.Character.CharacterController;
using Game.Character.Stats;
using Game.GlobalComponent;
using Game.Items;
using Game.Shop;
using Game.UI;
using System;
using System.Collections.Generic;
using Game.Managers;
using RopeHeroViceCity.Scripts.Gampelay;
using RopeHeroViceCity.UI_Features.UI_InGame.Scripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Weapons
{
	public class WeaponController : MonoBehaviour
	{
		public bool DebugLog;

		[Space(10f)]
		public AudioSource AudioSource;

		public HitEntity Owner;

		public Image WeaponImage;

		public List<Weapon> Weapons = new List<Weapon>();

		[Space(10f)]
		public Weapon StartWeapon;

		public WeaponSet WeaponSet;

		[SerializeField]
		private Weapon currentWeapon;

		private int currentSlotIndex;

		[SerializeField]
		private int currentWeaponIndex;

		private AttackTargetParameters attackTargetParameters = new AttackTargetParameters();

		private PlayerStoreProfile playerProfile;

		private Player player;

		private float meleeWeaponDamageMultipler;

		public Weapon CurrentWeapon => currentWeapon;

		public void Initialization()
		{
			playerProfile = GetComponent<PlayerStoreProfile>();
			player = GetComponent<Player>();
			ActivateFists();
			Owner = GetComponent<HitEntity>();
			if (Owner == null)
			{
				UnityEngine.Debug.LogError("Can't find owner!");
			}
			BaseSoundController component = GetComponent<BaseSoundController>();
			if ((bool)component)
			{
				component.IsInGameSound = true;
			}
			attackTargetParameters.AttackMethod = AttackMethod.None;
			// UIGame instance = UIGame.Instance;
			// instance.OnExitInMenu = (Action)Delegate.Combine(instance.OnExitInMenu, new Action(OnExitToMenu));
			EventManager.Instance.OnExitInMenu.AddListener(OnExitToMenu);
			//UI_InGame.OnExitInMenu.AddListener(OnExitToMenu);
		}

		public void Deinitialization()
		{
			if (currentWeapon != null)
			{
				CurrentWeapon.DeInit();
			}
		}

		private void OnExitToMenu()
		{
			if (PlayerManager.Instance.DefaultWeaponController == this)
			{
				Deinitialization();
			}
		}

		public void UpdatePlayerStats()
		{
			if ((bool)player && !player.IsTransformer)
			{
				meleeWeaponDamageMultipler = player.stats.GetPlayerStat(StatsList.MeleeWeaponDamage);
				if ((bool)WeaponSet.Slots[1].WeaponInstance)
				{
					MeleeWeapon meleeWeapon = (MeleeWeapon)WeaponSet.Slots[1].WeaponInstance;
					meleeWeapon.UpdateStats(player);
				}
			}
		}

		public void SwitchWeapon(bool right)
		{
			if (Weapons == null || Weapons.Count <= 1)
			{
				return;
			}
			if (right)
			{
				currentWeaponIndex++;
			}
			else
			{
				currentWeaponIndex--;
				while (currentWeaponIndex < 0)
				{
					currentWeaponIndex += Weapons.Count;
				}
			}
			InitWeapon(currentWeaponIndex);
		}

		private void GetWeapon(string name)
		{
			if (Weapons == null || Weapons.Count <= 0)
			{
				return;
			}
			int i;
			for (i = 0; i < Weapons.Count; i++)
			{
				if (Weapons[i].gameObject.name.Equals(name))
				{
					currentWeaponIndex = i;
					break;
				}
			}
			if (i >= Weapons.Count)
			{
				currentWeaponIndex = 0;
			}
			currentWeapon = Weapons[currentWeaponIndex];
			if (CurrentWeapon == null)
			{
				currentWeapon = Weapons[0];
			}
		}

		private void GetWeapon(int index)
		{
			if (Weapons != null && Weapons.Count > 0)
			{
				index %= Weapons.Count;
				currentWeaponIndex = index;
				currentWeapon = Weapons[currentWeaponIndex];
			}
		}

		public void HideWeapon()
		{
			currentWeapon.gameObject.SetActive(value: false);
		}

		public void ShowWeapon()
		{
			currentWeapon.gameObject.SetActive(value: true);
		}

		public bool ActivateWeaponByType(WeaponArchetype weapArchetype)
		{
			if (Weapons != null && Weapons.Count > 1)
			{
				int num = -1;
				for (int i = 0; i < Weapons.Count; i++)
				{
					if (Weapons[i].Archetype == weapArchetype)
					{
						num = i;
						if (weapArchetype != 0 || Weapons[i].GetComponent<RangedWeapon>().AmmoCount > 0)
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
				InitWeapon(num);
				return true;
			}
			return false;
		}

		private void InitWeapon()
		{
			if (CurrentWeapon == null)
			{
				return;
			}
			if (WeaponImage)
			{
				WeaponImage.sprite = CurrentWeapon.image;
			}
			CurrentWeapon.gameObject.SetActive(value: true);
			CurrentWeapon.Init();
			CurrentWeapon.PerformAttackEvent = PerformAttackEvent;
			CurrentWeapon.InflictDamageEvent = InflictDamageEvent;
			RangedWeapon rangedWeapon = CurrentWeapon as RangedWeapon;
			if ((bool)rangedWeapon && rangedWeapon.EnergyCost == 0f)
			{
				rangedWeapon.AmmoChangedEvent = AmmoChanged;
				rangedWeapon.RechargeStartedEvent = RechargeStarted;
				UpdateAmmoText(rangedWeapon.AmmoCountText);
			}
			else
			{
				UpdateAmmoText(null);
			}
		}

		private void DeInitWeapon(Weapon weapon)
		{
			if (!(weapon == null))
			{
				weapon.PerformAttackEvent = null;
				weapon.InflictDamageEvent = null;
				weapon.DeInit();
				UpdateAmmoText(null);
				weapon.gameObject.SetActive(value: false);
			}
		}

		public void InitWeapon(int index)
		{
			DeInitWeapon(CurrentWeapon);
			GetWeapon(index);
			InitWeapon();
		}

		private void RechargeStarted(Weapon weapon)
		{
			RangedWeapon rangedWeapon = weapon as RangedWeapon;
			if (rangedWeapon)
			{
				rangedWeapon.PlayRechargeSound(AudioSource);
			}
		}

		private void AmmoChanged(Weapon weapon)
		{
			RangedWeapon rangedWeapon = weapon as RangedWeapon;
			if (rangedWeapon)
			{
				UpdateAmmoText(rangedWeapon.AmmoCountText);
			}
		}

		private void InflictDamageEvent(Weapon weapon, HitEntity owner, HitEntity victim, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			float num = 1f;
			if ((bool)player && weapon.Archetype == WeaponArchetype.Melee)
			{
				num = meleeWeaponDamageMultipler;
			}
			float damage = weapon.Damage * num;
			victim.OnHit(weapon.WeaponDamageType, owner, damage, hitPos, hitVector, defenceReduction);
		}

		private void PerformAttackEvent(Weapon weapon)
		{
			weapon.PlayAttackSound(AudioSource);
			RangedWeapon rangedWeapon = weapon as RangedWeapon;
			if ((bool)rangedWeapon)
			{
				MakeShotFlash(rangedWeapon.Muzzle, rangedWeapon.ShotSfx);
				MakeShotFireKick(rangedWeapon.FireKickPower);
			}
		}

		private void MakeShotFlash(Transform muzzle, ShotSFXType type)
		{
			if ((bool)WeaponManager.Instance && (bool)muzzle)
			{
				WeaponManager.Instance.StartShootSFX(muzzle, type);
			}
		}

		private void MakeShotFireKick(float kickPower)
		{
			if (Owner is Player)
			{
				FireKick fireKick = EffectManager.Instance.Create<FireKick>();
				fireKick.KickAngle = kickPower;
				fireKick.Play();
			}
		}

		public void UpdateAmmoText()
		{
			RangedWeapon rangedWeapon = currentWeapon as RangedWeapon;
			if (rangedWeapon != null)
			{
				UpdateAmmoText(rangedWeapon.AmmoCountText);
			}
		}

		public void UpdateAmmoText(string  ammoText)
		{
			if (Owner is Player)
			{
				EventManager.Instance.UpdateAmmoPlayer(ammoText);
			}
		}

		public void Aim()
		{
		}

		public void Hold()
		{
		}

		public void Attack()
		{
			if ((bool)CurrentWeapon)
			{
				attackTargetParameters.AttackMethod = AttackMethod.Attack;
			}
		}

		public void Attack(HitEntity entity)
		{
			if ((bool)CurrentWeapon)
			{
				attackTargetParameters.AttackMethod = AttackMethod.AttackEntity;
				attackTargetParameters.Victim = entity;
			}
		}

		public void Attack(Vector3 direction)
		{
			if ((bool)CurrentWeapon)
			{
				attackTargetParameters.AttackMethod = AttackMethod.AttackByDirection;
				attackTargetParameters.Direction = direction;
			}
		}

		public void AttackWithWeapon()
		{
			Invoke("InvokedAttack", 0f);
		}

		private void InvokedAttack()
		{
			switch (attackTargetParameters.AttackMethod)
			{
			case AttackMethod.Attack:
				CurrentWeapon.Attack(Owner);
				break;
			case AttackMethod.AttackByDirection:
				CurrentWeapon.Attack(Owner, attackTargetParameters.Direction);
				break;
			case AttackMethod.AttackEntity:
				CurrentWeapon.Attack(Owner, attackTargetParameters.Victim);
				break;
			default:
				UnityEngine.Debug.LogError("Unsupported attack method!");
				break;
			}
		}

		public void MeleeWeaponAttack(int attackState)
		{
			MeleeWeapon meleeWeapon = currentWeapon as MeleeWeapon;
			if (meleeWeapon != null)
			{
				meleeWeapon.MeleeAttack(attackState);
			}
		}

		public void AddWeaponInList(Weapon newWeapon)
		{
			if (!Weapons.Contains(newWeapon))
			{
				Weapons.Add(newWeapon);
			}
		}

		public void RemoveWeaponFromList(Weapon oldWeapon)
		{
			if (Weapons.Contains(oldWeapon))
			{
				ActivateWeaponByType(WeaponArchetype.Melee);
				Weapons.Remove(oldWeapon);
			}
		}

		public bool CheckIsThisWeaponControllerWeapon(Weapon weapon)
		{
			return currentWeapon == weapon;
		}

		public void CheckReloadOnWakeUp()
		{
			RangedWeapon rangedWeapon = currentWeapon as RangedWeapon;
			if (rangedWeapon != null && rangedWeapon.AmmoInCartridgeCount <= 0)
			{
				rangedWeapon.RechargeFinish();
			}
		}

		public void LostCurrentWeapon()
		{
		}

		public WeaponSlotTypes GetTargetSlot(Weapon incWeapon)
		{
			if (incWeapon.Type == WeaponTypes.None)
			{
				return WeaponSlotTypes.None;
			}
			if (incWeapon.Type == WeaponTypes.Melee)
			{
				return WeaponSlotTypes.Melee;
			}
			if (incWeapon.Type == WeaponTypes.Pistol || incWeapon.Type == WeaponTypes.SMG)
			{
				return WeaponSlotTypes.Additional;
			}
			if (incWeapon.Type == WeaponTypes.Rifle || incWeapon.Type == WeaponTypes.Shotgun)
			{
				return WeaponSlotTypes.Main;
			}
			if (incWeapon.Type == WeaponTypes.Heavy)
			{
				return WeaponSlotTypes.Heavy;
			}
			return WeaponSlotTypes.Universal;
		}

		public bool WeaponSlotIsEmpty(int targetSlotIndex)
		{
			return WeaponSet.Slots[targetSlotIndex].WeaponPrefab == null;
		}

		public int EquipWeapon(GameItemWeapon weaponItem, int slotIndex, bool temporary = false)
		{
			if (WeaponSet.Locked || slotIndex >= WeaponSet.Slots.Length)
			{
				return -1;
			}
			Loadout currentLoadout = PlayerStoreProfile.CurrentLoadout;
			string key = Loadout.KeyFromWeaponSlot(slotIndex);
			int num = 0;
			if (currentSlotIndex == slotIndex)
			{
				ActivateFists();
			}
			num = (currentLoadout.Weapons.ContainsKey(key) ? currentLoadout.Weapons[key] : 0);
			if (currentLoadout.Weapons.ContainsKey(key))
			{
				currentLoadout.Weapons[key] = weaponItem.ID;
			}
			else
			{
				currentLoadout.Weapons.Add(key, weaponItem.ID);
			}
			WeaponSet.Slots[slotIndex].WeaponPrefab = weaponItem.Weapon;
			UpdateWeaponSlotPlaceholder(slotIndex);
			RangedWeapon rangedWeapon = WeaponSet.Slots[slotIndex].WeaponInstance as RangedWeapon;
			if (rangedWeapon != null)
			{
				rangedWeapon.IsFiniteAmmo = !temporary;
			}
			MeleeWeapon x = WeaponSet.Slots[slotIndex].WeaponInstance as MeleeWeapon;
			if (x != null)
			{
				UpdatePlayerStats();
			}
			if (!temporary)
			{
				PlayerStoreProfile.SaveLoadout();
			}
			return num;
		}

		public void UnEquipWeapon(int slotIndex)
		{
			if (!WeaponSet.Locked && slotIndex < WeaponSet.Slots.Length)
			{
				if (currentSlotIndex == slotIndex)
				{
					ActivateFists();
				}
				Loadout currentLoadout = PlayerStoreProfile.CurrentLoadout;
				string key = "WeaponSlot" + slotIndex;
				if (currentLoadout.Weapons.ContainsKey(key))
				{
					currentLoadout.Weapons[key] = 0;
				}
				else
				{
					currentLoadout.Weapons.Add(key, 0);
				}
				WeaponSet.Slots[slotIndex].WeaponPrefab = null;
				if ((bool)WeaponSet.Slots[slotIndex].WeaponInstance)
				{
					PoolManager.Instance.ReturnToPool(WeaponSet.Slots[slotIndex].WeaponInstance);
				}
				WeaponSet.Slots[slotIndex].WeaponInstance = null;
				PlayerStoreProfile.SaveLoadout();
			}
		}

		public void ChooseSlot(int slotIndex)
		{
			if (!WeaponSet.Locked)
			{
				DeInitWeapon(currentWeapon);
				currentWeapon = WeaponSet.Slots[slotIndex].WeaponInstance;
				InitWeapon();
				currentSlotIndex = slotIndex;
				player.OnPlayerChooseOtherWeapon?.Invoke();
			}
		}

		public void TryWeapon(Weapon weaponToTry)
		{
			DeInitWeapon(currentWeapon);
			var slotIndex = PlayerManager.Instance.DefaultWeaponController.WeaponSet.GetEmptySlotOfTypeInt(PlayerManager.Instance.DefaultWeaponController.GetTargetSlot(weaponToTry),true);
			Weapon fromPool = PoolManager.Instance.GetFromPool(weaponToTry);
			fromPool.transform.parent = WeaponSet.Slots[slotIndex].Placeholder.transform;
			fromPool.transform.localRotation = Quaternion.identity;
			fromPool.transform.localPosition = Vector3.zero;
			fromPool.transform.localScale = Vector3.one;

			currentWeapon = fromPool;
			InitWeapon();
			currentSlotIndex = slotIndex;
		}

		public void RemoveTryWeapon()
		{
			ChooseSlot(0);
		}

		public void ActivateFists()
		{
			if (WeaponSet.Locked)
			{
				if (WeaponImage.sprite != currentWeapon.image)
				{
					InitWeapon();
				}
			}
			else
			{
				DeInitWeapon(currentWeapon);
				currentWeapon = WeaponSet.GetFirstSlotOfType(WeaponSlotTypes.None).WeaponInstance;
				InitWeapon();
			}
		}

		private void UpdateWeaponSlotPlaceholder(int slotIndex)
		{
			if ((bool)WeaponSet.Slots[slotIndex].WeaponInstance)
			{
				PoolManager.Instance.ReturnToPool(WeaponSet.Slots[slotIndex].WeaponInstance);
			}
			Weapon fromPool = PoolManager.Instance.GetFromPool(WeaponSet.Slots[slotIndex].WeaponPrefab);
			fromPool.transform.parent = WeaponSet.Slots[slotIndex].Placeholder.transform;
			fromPool.transform.localRotation = Quaternion.identity;
			fromPool.transform.localPosition = Vector3.zero;
			fromPool.transform.localScale = Vector3.one;
			WeaponSet.Slots[slotIndex].WeaponInstance = fromPool;
			WeaponSet.Slots[slotIndex].WeaponInstance.gameObject.SetActive(value: false);
		}

		public void LockWeponSet()
		{
			WeaponSet.Locked = true;
		}

		public void UnlockWeponSet()
		{
			WeaponSet.Locked = false;
		}
	}
}
