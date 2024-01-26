using Game.Shop;

namespace Game.GlobalComponent.Qwest
{
	public class ComboTaskNode : TaskNode
	{
		[Separator("Specific")]
		public bool WeaponDependent = true;

		public WeaponNameList RequiredWeapon = WeaponNameList.Any;

		public int RequiredComboCount = 10;

		public override BaseTask ToPo()
		{
			ComboTask comboTask = new ComboTask();
			comboTask.WeaponDependent = WeaponDependent;
			comboTask.RequiredWeapon = RequiredWeapon;
			comboTask.RequiredComboCount = RequiredComboCount;
			ToPoBase(comboTask);
			return comboTask;
		}
	}
}
