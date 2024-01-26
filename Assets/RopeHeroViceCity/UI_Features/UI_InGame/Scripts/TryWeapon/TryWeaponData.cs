using UnityEngine;

namespace UI.TryWeapon
{
    [CreateAssetMenu(fileName = "TryWeaponData", menuName = "TryWeapon/Data", order = 100)]
    public class TryWeaponData : ScriptableObject
    {
        public WeaponTryType typeTry;
        public int ExpireAmount;
    }
}
