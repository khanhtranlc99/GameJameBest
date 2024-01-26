using Game.Character.Stats;
using UnityEngine;

namespace RopeHeroViceCity.Scripts.Gampelay
{
    public class UIStatDisplay : CharacterStatDisplay
    {
        protected override void UpdateDisplayValue()
        {
            var percent = Mathf.Clamp01(current / max);
            EventManager.Instance.OnChangePlayerStat(nameStat,percent);
        }

        public override void OnChanged(float amount)
        {
            var percent = Mathf.Clamp01(current / max);
            EventManager.Instance.OnChangePlayerStat(nameStat,percent);
        }
    }
}
