using Game.Character;
using Game.Shop;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SyncStoredData : MonoBehaviour
{
	private const string VersionKeyHesh = "VersionKey";

	public bool work;

	public string VersionKey;

	public float MoneyExchangeRate;

	public float ExpExchangeRate;

	//public Text debug;

	public GameObject Panel;

	private int currentMoney;

	private int currentSP;

	private int currentExp;

	private int newMoney;

	private int newExp;

	private string[] WeaponsNames;

	private void Awake()
	{
		WeaponsNames = Enum.GetNames(typeof(WeaponNameList));
	}

	private void Start()
	{
		if (work)
		{
			if (UserHaventActualVersionKey() && HaveDataForSave())
			{
				ClearAllPrefs();
				UpdateData();
			}
			Invoke("DisablePanel", 5f);
		}
	}

	private void DisablePanel()
	{
		Panel.SetActive(value: false);
	}

	private bool HaveDataForSave()
	{
		currentMoney = PlayerInfoManager.Money;
		currentExp = PlayerInfoManager.Experience;
		currentSP = PlayerInfoManager.UpgradePoints;
		string[] weaponsNames = WeaponsNames;
		foreach (string weaponName in weaponsNames)
		{
			if (PlayerStoreProfile.Instance.GetOldWeapon(weaponName))
			{
				currentMoney += 3000;
			}
		}
		if (currentMoney != 0 || currentExp != 0)
		{
			return true;
		}
		return false;
	}

	private void UpdateData()
	{
		ConvertStats();
		AddVersionKey();
		ShowMessage();
	}

	private void ConvertStats()
	{
		newMoney = (int)((float)currentMoney * MoneyExchangeRate);
		newExp = (int)((float)currentExp * ExpExchangeRate);
		PlayerInfoManager.Money = newMoney;
		PlayerInfoManager.Experience = newExp;
		PlayerInfoManager.UpgradePoints = currentSP;
	}

	private bool UserHaventActualVersionKey()
	{
		if (!PlayerPrefs.HasKey("VersionKey"))
		{
			return true;
		}
		string @string = PlayerPrefs.GetString("VersionKey");
		if (@string == VersionKey)
		{
			//debug.text = "User already have version actual key";
			return false;
		}
		return true;
	}

	private void ClearAllPrefs()
	{
		BaseProfile.ClearBaseProfileWithoutSystemSettings();
	}

	private void AddVersionKey()
	{
		PlayerPrefs.SetString("VersionKey", VersionKey);
	}

	private void ShowMessage()
	{
		//debug.text = $"old money {currentMoney}, new money {newMoney} \n old exp {currentExp}, new exp {newExp}";
	}
}
