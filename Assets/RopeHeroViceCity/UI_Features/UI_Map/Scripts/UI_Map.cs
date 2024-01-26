using Game.MiniMap;
using UnityEngine;

namespace RopeHeroViceCity.UI_Features.UI_Map.Scripts
{
    public class UI_Map : AbsUICanvas
    {
        public override void ShowLayer()
        {
            base.ShowLayer();
            MiniMap.Instance.ChangeMapSize(true);
            GameplayUtils.PauseGame();
        }

        public override void Close()
        {
            base.Close();
            MiniMap.Instance.ChangeMapSize(false);
            GameplayUtils.ResumeGame();
        }
    }
}
