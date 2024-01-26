using UnityEngine;

namespace UI.TryWeapon
{
    public class TryActionMelee : TryWeaponAction
    {
        public TryActionMelee(int valueAmount) : base(valueAmount)
        {
        }

        public override bool CheckExpire()
        {
            valueAmount--;
            return valueAmount <= 0;
        }
    }
}
