using Game.Shop;
using UnityEngine;

namespace RopeHeroViceCity.Scripts.Gampelay
{
    public class ResourcesManager : SingletonMonoBehavior<ResourcesManager>
    {
        [SerializeField] private ResourceScriptableObject resourceScriptableObject;

        public ShopIcons ShopIcons => resourceScriptableObject.shopIcons;
    }
}
