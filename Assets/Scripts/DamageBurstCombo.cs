using Game.Weapons;

public class DamageBurstCombo : ComboManager.BaseComboEffect
{
	[Separator("DamageBurstCombo")]
	public float DamagePercentPerStack = 10f;

	private Weapon linkedWeapon;

	private float originalDamage;

	protected override void Start()
	{
		base.Start();
		linkedWeapon = GetComponent<SpecialWeaponComboController>().LinkedWeapon;
		originalDamage = linkedWeapon.Damage;
	}

	public override void AddStackEffect()
	{
		float damage = originalDamage + (float)currStacks * originalDamage * DamagePercentPerStack / 100f;
		if (linkedWeapon is MeleeWeapon)
		{
			(linkedWeapon as MeleeWeapon).SetDamage(damage);
		}
		else
		{
			linkedWeapon.Damage = damage;
		}
	}

	public override void ClearStacksEffects()
	{
		if (linkedWeapon is MeleeWeapon)
		{
			(linkedWeapon as MeleeWeapon).SetDamage(originalDamage);
		}
		else
		{
			linkedWeapon.Damage = originalDamage;
		}
	}
}
