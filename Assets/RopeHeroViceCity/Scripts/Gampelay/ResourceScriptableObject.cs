using Game.Shop;
using UnityEngine;

namespace RopeHeroViceCity.Scripts.Gampelay
{
    [CreateAssetMenu(fileName = "ResourcesData", menuName = "RopeData/ResourceData", order = 100)]
    public class ResourceScriptableObject : ScriptableObject
    {
        public ShopIcons shopIcons;
    }
}
