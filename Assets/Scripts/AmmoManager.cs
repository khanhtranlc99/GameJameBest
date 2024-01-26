using Game.Character.CharacterController;
using Game.Items;
using Game.Weapons;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
	private static AmmoManager instance;

	private List<JointAmmoContainer> ammoContainers = new List<JointAmmoContainer>();

	private bool inited;

	public static AmmoManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<AmmoManager>();
			}
			return instance;
		}
	}

	private void Init()
	{
		if (!inited)
		{
			JointAmmoContainer[] componentsInChildren = GetComponentsInChildren<JointAmmoContainer>();
			JointAmmoContainer[] array = componentsInChildren;
			foreach (JointAmmoContainer jointAmmoContainer in array)
			{
				ammoContainers.Add(jointAmmoContainer);
				jointAmmoContainer.UpdateAmmo();
			}
			inited = true;
		}
	}

	public void AddContainer(GameItemAmmo incAmmoItem)
	{
		if (!inited)
		{
			Init();
		}
		foreach (JointAmmoContainer ammoContainer in ammoContainers)
		{
			if (ammoContainer.GameItemAmmo.ID == incAmmoItem.ID)
			{
				ammoContainer.AmmoCount += incAmmoItem.ShopVariables.PerStackAmount;
				return;
			}
		}
		CreateContainer(incAmmoItem);
	}

	public void CreateContainer(GameItemAmmo incAmmoItem)
	{
		if (!(GetContainer(incAmmoItem.AmmoType) != null))
		{
			GameObject gameObject = new GameObject();
			gameObject.name = incAmmoItem.AmmoType.ToString();
			gameObject.transform.parent = base.transform;
			JointAmmoContainer jointAmmoContainer = gameObject.AddComponent<JointAmmoContainer>();
			jointAmmoContainer.GameItemAmmo = incAmmoItem;
			jointAmmoContainer.AmmoCount = incAmmoItem.ShopVariables.PerStackAmount;
			ammoContainers.Add(jointAmmoContainer);
		}
	}

	public void AddAmmo(AmmoTypes neededType)
	{
		if (!inited)
		{
			Init();
		}
		if (neededType != AmmoTypes.None)
		{
			foreach (JointAmmoContainer ammoContainer in ammoContainers)
			{
				if (ammoContainer.GameItemAmmo.AmmoType == neededType)
				{
					ammoContainer.AmmoCount += ammoContainer.GameItemAmmo.ShopVariables.PerStackAmount;
					RangedWeapon rangedWeapon = PlayerManager.Instance.WeaponController.CurrentWeapon as RangedWeapon;
					if (rangedWeapon != null && rangedWeapon.AmmoType == neededType)
					{
						PlayerManager.Instance.WeaponController.UpdateAmmoText(rangedWeapon.AmmoCountText);
					}
					return;
				}
			}
			if (PlayerManager.Instance.WeaponController.CurrentWeapon.AmmoType == neededType)
			{
				PlayerManager.Instance.WeaponController.UpdateAmmoText();
			}
		}
	}

	public void UpdateAmmo(AmmoTypes neededType)
	{
		if (!inited)
		{
			Init();
		}
		foreach (JointAmmoContainer ammoContainer in ammoContainers)
		{
			if (ammoContainer.GameItemAmmo.AmmoType == neededType)
			{
				ammoContainer.UpdateAmmo();
				break;
			}
		}
	}

	public JointAmmoContainer GetContainer(AmmoTypes neededType)
	{
		if (!inited)
		{
			Init();
		}
		JointAmmoContainer result = null;
		foreach (JointAmmoContainer ammoContainer in ammoContainers)
		{
			if (ammoContainer.GameItemAmmo.AmmoType == neededType)
			{
				return ammoContainer;
			}
		}
		return result;
	}
}
