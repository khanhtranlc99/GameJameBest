using Game.Character;
using UnityEngine;

namespace Game.Items
{
	//[CreateAssetMenu(fileName = "ItemBonusesData", menuName = "RopeData/ItemData/Bonuses", order = 100)]
	public class GameItemBonus : GameItem
	{
		[Space(10f)]
		public BonusTypes BonusType;

		[Space(10f)]
		public int BonusValue = 1;

		public override bool CanBeBought
		{
			get
			{
				if (BonusType == BonusTypes.VIP)
				{
					return PlayerInfoManager.VipLevel < BonusValue;
				}
				if (BonusType == BonusTypes.Money)
				{
					return PlayerInfoManager.Money + BonusValue <= PlayerInfoManager.Instance.GetMaxValue(PlayerInfoType.Money);
				}
				return true;
			}
		}

		public override bool SameParametrWithOther(object[] parametrs)
		{
			if (parametrs.Length == 1)
			{
				return BonusType == (BonusTypes)(int)parametrs[0];
			}
			if (parametrs.Length == 2)
			{
				return BonusType == (BonusTypes)(int)parametrs[0] && BonusValue == (int)parametrs[1];
			}
			return false;
		}
	}
}
