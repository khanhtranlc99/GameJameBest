using Game.Character;
using Game.Character.CharacterController;
using Game.Weapons;
using System;
using UnityEngine;

public class SpecialWeaponComboController : MonoBehaviour
{
	[Serializable]
	public class ComboEffectTrigger
	{
		public int ComboTriggerMultiplier;

		public ComboManager.BaseComboEffect[] ComboEffects;
	}

	public bool DebugLog;

	[Space(10f)]
	public bool WeaponDependent = true;

	public Weapon LinkedWeapon;

	[Space(10f)]
	public ComboEffectTrigger[] comboEffectTriggers;

	private bool comboAllowed = true;

	private float stopCDTime;

	private void OnPlayerDie(float deathTime)
	{
		StopAllEffects();
	}

	private void OnPlayerEnterVehicle(bool isIn)
	{
		StopAllEffects();
	}

	private void Start()
	{
		if (ComboManager.Instance != null)
		{
			if (WeaponDependent)
			{
				ComboManager instance = ComboManager.Instance;
				instance.WeaponComboEvent = (ComboManager.ComboDelegate)Delegate.Combine(instance.WeaponComboEvent, new ComboManager.ComboDelegate(OnCombo));
			}
			else
			{
				ComboManager instance2 = ComboManager.Instance;
				instance2.OverallComboEvent = (ComboManager.ComboDelegate)Delegate.Combine(instance2.OverallComboEvent, new ComboManager.ComboDelegate(OnCombo));
			}
		}
		if (WeaponDependent && !LinkedWeapon)
		{
			LinkedWeapon = GetComponent<Weapon>();
		}
		if (WeaponDependent && !LinkedWeapon)
		{
			comboAllowed = false;
			if (DebugLog)
			{
				UnityEngine.Debug.LogError("Не выставленно комбо-оружие! Комбо заблокированно!");
			}
		}
		if (PlayerDieManager.Instance != null)
		{
			PlayerDieManager instance3 = PlayerDieManager.Instance;
			instance3.PlayerDiedEvent = (PlayerDieManager.PlayerDied)Delegate.Combine(instance3.PlayerDiedEvent, new PlayerDieManager.PlayerDied(OnPlayerDie));
		}
		if (PlayerInteractionsManager.Instance.Player != null)
		{
			PlayerInteractionsManager.Instance.Player.PlayerGetInOutVehicleEvent = OnPlayerEnterVehicle;
		}
	}

	private void Update()
	{
		AutoAllowCombo();
	}

	private void OnCombo(ComboManager.ComboInfo comboInfo)
	{
		if (comboAllowed)
		{
			if (WeaponDependent && comboInfo.WeaponNameInList != LinkedWeapon.WeaponNameInList)
			{
				ComboManager.Instance.UpdateComboMeter(0, string.Empty, turnOff: true);
				return;
			}
			if (DebugLog)
			{
				UnityEngine.Debug.Log("X" + comboInfo.ComboMultiplier + " COMBO with " + comboInfo.WeaponNameInList + "!");
			}
			if (WeaponDependent)
			{
				ComboManager.Instance.UpdateComboMeter(comboInfo.ComboMultiplier, (!string.Equals(LinkedWeapon.Name, string.Empty)) ? LinkedWeapon.Name : LinkedWeapon.WeaponNameInList.ToString());
			}
			else
			{
				ComboManager.Instance.UpdateComboMeter(comboInfo.ComboMultiplier, comboInfo.WeaponNameInList.ToString());
			}
			ComboEffectTrigger[] array = comboEffectTriggers;
			foreach (ComboEffectTrigger comboEffectTrigger in array)
			{
				if (comboInfo.ComboMultiplier < comboEffectTrigger.ComboTriggerMultiplier)
				{
					continue;
				}
				ComboManager.BaseComboEffect[] comboEffects = comboEffectTrigger.ComboEffects;
				foreach (ComboManager.BaseComboEffect baseComboEffect in comboEffects)
				{
					if (baseComboEffect.IsFinisher)
					{
						StopAllEffects();
						DisallowCombo(baseComboEffect.EffectDuration);
					}
					baseComboEffect.StartEffect(comboInfo);
				}
			}
		}
		else
		{
			ComboManager.Instance.UpdateComboMeter(0, string.Empty, turnOff: true);
		}
	}

	private void StopAllEffects()
	{
		ComboEffectTrigger[] array = comboEffectTriggers;
		foreach (ComboEffectTrigger comboEffectTrigger in array)
		{
			ComboManager.BaseComboEffect[] comboEffects = comboEffectTrigger.ComboEffects;
			foreach (ComboManager.BaseComboEffect baseComboEffect in comboEffects)
			{
				baseComboEffect.StopEffect();
			}
		}
	}

	private void DisallowCombo(float cooldown)
	{
		stopCDTime = Time.time + cooldown;
		comboAllowed = false;
	}

	private void AutoAllowCombo()
	{
		if (stopCDTime != 0f && Time.time >= stopCDTime)
		{
			stopCDTime = 0f;
			comboAllowed = true;
			ComboManager.Instance.ResetWeaponCombo();
			ComboManager.Instance.UpdateComboMeter(0, string.Empty, turnOff: true);
		}
	}
}
