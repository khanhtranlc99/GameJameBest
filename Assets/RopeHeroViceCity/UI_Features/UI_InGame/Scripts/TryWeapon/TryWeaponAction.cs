using System.Collections;
using System.Collections.Generic;
using Game.Character.CharacterController;
using Game.Items;
using UI;
using UnityEngine;

public abstract class TryWeaponAction
{
    public int valueAmount;
    public TryWeaponAction( int valueAmount)
    {
        this.valueAmount = valueAmount;
    }

    public abstract bool CheckExpire();

    public void ForceExpire()
    {
        valueAmount = 0;
    }
    

    public virtual void ApplyItemToTry(UITryWeapon controller,GameItemWeapon choosedWeapon)
    {
        PlayerManager.Instance.WeaponController.TryWeapon(choosedWeapon.Weapon);
    }
}
