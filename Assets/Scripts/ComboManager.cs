using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Extras;
using Game.GlobalComponent.Quality;
using Game.Shop;
using Game.Vehicle;
using Game.Weapons;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ComboManager : MonoBehaviour
{
	public class ComboInfo
	{
		public WeaponNameList WeaponNameInList = WeaponNameList.None;

		public int ComboMultiplier;

		public HitEntity LastVictim;
	}

	public class BaseComboEffect : BaseBuff
	{
		[Tooltip("Use combo multiplier as start stack count?")]
		[Separator("Combo specific parameters")]
		public bool MultAsStackCount;

		[Tooltip("Finisher will stop other combo-effects and disallow new one untill it's own effet ends")]
		public bool IsFinisher;

		public virtual void StartEffect(ComboInfo comboInfo)
		{
			if (MultAsStackCount)
			{
				currStacks = comboInfo.ComboMultiplier;
			}
			StartEffect();
		}

		public virtual void StopEffect(ComboInfo comboInfo)
		{
			StopEffect();
		}

		public override void AddStackEffect()
		{
		}

		public override void ClearStacksEffects()
		{
		}
	}

	public delegate void ComboDelegate(ComboInfo curCombo);

	private static ComboManager instance;

	public bool DebugLog;

	[Space(10f)]
	public float MaxTimeBetweenKills = 3f;

	public bool AutoСalculateMaxTime = true;

	public float AutoCalculateMult = 1f;

	[Space(10f)]
	public Transform ComboPanel;

	public Text ComboMeterText;

	public Slider ComboMeterSlider;

	public float ComboMeterUpdateCooldown = 0.01f;

	public ComboDelegate OverallComboEvent;

	public ComboDelegate WeaponComboEvent;

	private Player player;

	private WeaponController weaponController;

	private WeaponNameList previousWeapon = WeaponNameList.None;

	private WeaponNameList neededWeapon = WeaponNameList.None;

	private float maxTimeBetweenKills;

	private float previousKillTime;

	private float previousComboMeterUpdateTime;

	private int overallComboMultiplier;

	private ComboInfo weaponComboInfo = new ComboInfo();

	private ComboInfo overallComboInfo = new ComboInfo();

	public static ComboManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<ComboManager>();
			}
			return instance;
		}
	}

	public ComboInfo WeaponComboInfo
	{
		get
        {
			return weaponComboInfo;
		}
	}
	

	public ComboInfo OverallComboInfo
	{
		get
        {
			return overallComboInfo;
		}
	}
	

	public void Init()
	{
		if (DebugLog)
		{
			UnityEngine.Debug.Log("Combo manager started");
		}
		if (EntityManager.Instance != null)
		{
			EntityManager entityManager = EntityManager.Instance;
			entityManager.PlayerKillEvent = (EntityManager.PlayerKill)Delegate.Combine(entityManager.PlayerKillEvent, new EntityManager.PlayerKill(ComboCount));
		}
		if (PlayerDieManager.Instance != null)
		{
			PlayerDieManager playerDieManager = PlayerDieManager.Instance;
			playerDieManager.PlayerDiedEvent = (PlayerDieManager.PlayerDied)Delegate.Combine(playerDieManager.PlayerDiedEvent, new PlayerDieManager.PlayerDied(OnPlayerDie));
		}
		player = PlayerInteractionsManager.Instance.Player;
		weaponController = player.GetComponent<WeaponController>();
		if (AutoСalculateMaxTime)
		{
			maxTimeBetweenKills = (float)(90 / QualityManager.CountPedestrians) * AutoCalculateMult;
			MaxTimeBetweenKills = maxTimeBetweenKills;
		}
		else
		{
			maxTimeBetweenKills = MaxTimeBetweenKills;
		}
		if (ComboPanel != null && ComboMeterText != null && ComboMeterSlider != null)
		{
			ComboPanel.gameObject.SetActive(value: false);
			ComboMeterText.gameObject.SetActive(value: false);
			ComboMeterSlider.gameObject.SetActive(value: false);
		}
		else
		{
			UnityEngine.Debug.Log("Some components of ComboManager was NOT be seted. It's may cause some problems.");
		}
	}

	private void OnPlayerDie(float deathTime)
	{
		ResetAllCombos();
	}

	private void Update()
	{
		if (KillingDellay() > maxTimeBetweenKills)
		{
			ResetAllCombos();
		}
		ManageComboMeter();
	}

	public float KillingDellay()
	{
		return Time.time - previousKillTime;
	}

	private float TimeLeft()
	{
		float num = previousKillTime + maxTimeBetweenKills - Time.time;
		return (!(num > 0f)) ? 0f : num;
	}

	private void ComboCount(HitEntity enemy)
	{
		if (enemy is VehicleStatus)
		{
			return;
		}
		WeaponNameList weaponNameList = WeaponNameList.None;
		if (KillingDellay() <= maxTimeBetweenKills || previousWeapon == WeaponNameList.None)
		{
			weaponNameList = ((enemy.KilledByAbillity != WeaponNameList.None) ? enemy.KilledByAbillity : weaponController.CurrentWeapon.WeaponNameInList);
			overallComboInfo.ComboMultiplier++;
			overallComboInfo.WeaponNameInList = weaponNameList;
			overallComboInfo.LastVictim = enemy;
			if (previousWeapon == weaponNameList)
			{
				weaponComboInfo.ComboMultiplier++;
			}
			else
			{
				weaponComboInfo.ComboMultiplier = 1;
			}
			weaponComboInfo.WeaponNameInList = weaponNameList;
			weaponComboInfo.LastVictim = enemy;
		}
		previousKillTime = Time.time;
		previousWeapon = weaponNameList;
		if (OverallComboEvent != null)
		{
			OverallComboEvent(OverallComboInfo);
		}
		if (WeaponComboEvent != null && !PlayerInteractionsManager.Instance.inVehicle)
		{
			WeaponComboEvent(WeaponComboInfo);
		}
		if (DebugLog)
		{
			UnityEngine.Debug.Log(enemy.name + " killed. X" + OverallComboInfo.ComboMultiplier + " overallCOMBO by " + OverallComboInfo.WeaponNameInList);
			UnityEngine.Debug.Log(enemy.name + " killed. X" + WeaponComboInfo.ComboMultiplier + " weaponCOMBO by " + WeaponComboInfo.WeaponNameInList);
		}
	}

	public void ResetWeaponCombo()
	{
		weaponComboInfo.WeaponNameInList = (previousWeapon = WeaponNameList.None);
		weaponComboInfo.ComboMultiplier = 0;
	}

	public void ResetOverallCombo()
	{
		OverallComboInfo.WeaponNameInList = (previousWeapon = WeaponNameList.None);
		OverallComboInfo.ComboMultiplier = (overallComboMultiplier = 0);
	}

	public void ResetAllCombos()
	{
		UpdateComboMeter(0, string.Empty, turnOff: true);
		ResetWeaponCombo();
		ResetOverallCombo();
	}

	private void ManageComboMeter()
	{
		float num = TimeLeft();
		if (num <= 0.001f && previousKillTime != 0f)
		{
			UpdateComboMeter(0, string.Empty, turnOff: true);
			previousKillTime = 0f;
		}
		if (ComboMeterSlider != null && ComboMeterSlider.gameObject.activeInHierarchy)
		{
			ComboMeterSlider.value = num / maxTimeBetweenKills;
		}
	}

	public void UpdateComboMeter(int comboMult, string weaponName, bool turnOff = false)
	{
		if (ComboPanel != null && ComboMeterText != null && ComboMeterSlider != null)
		{
			if (DebugLog)
			{
				UnityEngine.Debug.Log("Обновляю стату на экране.");
			}
			if (turnOff)
			{
				ComboPanel.gameObject.SetActive(value: false);
				ComboMeterText.gameObject.SetActive(value: false);
				ComboMeterSlider.gameObject.SetActive(value: false);
				ComboMeterText.text = string.Empty;
				previousComboMeterUpdateTime = Time.time;
			}
			else if (Time.time - previousComboMeterUpdateTime >= ComboMeterUpdateCooldown)
			{
				ComboPanel.gameObject.SetActive(value: true);
				ComboMeterText.gameObject.SetActive(value: true);
				ComboMeterSlider.gameObject.SetActive(value: true);
				ComboMeterText.text = "X" + comboMult + " COMBO with " + weaponName;
				previousComboMeterUpdateTime = Time.time;
			}
		}
	}
}
