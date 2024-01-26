using UnityEngine;

namespace RopeHeroViceCity.UI_Features.DailyReward.Scripts
{
    [CreateAssetMenu(fileName = "DailyBonus", menuName = "UI_Feature/DailyBonus", order = 100)]
    public class DailyBonusData : ScriptableObject
    {
        public DailyBonus[] Bonuses;
    }
}
