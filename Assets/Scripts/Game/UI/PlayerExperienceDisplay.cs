using Game.Character;
using Game.Character.Stats;
using UnityEngine.UI;

namespace Game.UI
{
    public class PlayerExperienceDisplay : PlayerInfoDisplayBase
    {
        public Slider SliderLink;

        public Text TextLink;

        protected override PlayerInfoType GetInfoType()
        {
            return PlayerInfoType.Experience;
        }

        protected override void Display()
        {
            if ((bool)TextLink)
            {
                TextLink.text = InfoValue + "/" + LevelManager.Instance.ExpForNextLevel.ToString("#") + " Exp";
            }
            if ((bool)SliderLink)
            {
                SliderLink.maxValue = LevelManager.Instance.ExpForNextLevel;
                SliderLink.value = InfoValue;
            }
        }
    }
}
