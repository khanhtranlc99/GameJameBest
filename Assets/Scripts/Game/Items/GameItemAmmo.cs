using Game.Weapons;
using UnityEngine;

namespace Game.Items
{
	//[CreateAssetMenu(fileName = "ItemAmmoData", menuName = "RopeData/ItemData/Ammo", order = 100)]
	public class GameItemAmmo : GameItem
	{
		public AmmoTypes AmmoType;

		public GameObject ammoPrefab;

		public override void Init()
		{
			base.Init();
			AmmoManager.Instance.CreateContainer(this);
		}

		public override bool SameParametrWithOther(object[] parametrs)
		{
			return AmmoType == (AmmoTypes)(int)parametrs[0];
		}
	}
}
