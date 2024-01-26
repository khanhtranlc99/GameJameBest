using Game.Shop;
using System;

namespace Game.GlobalComponent.Qwest
{
	public class ComboTask : BaseTask
	{
		public bool WeaponDependent = true;

		public WeaponNameList RequiredWeapon = WeaponNameList.Any;

		public int RequiredComboCount = 10;

		private int maxComboAchieved;

		public override void TaskStart()
		{
			base.TaskStart();
			if (WeaponDependent)
			{
				ComboManager instance = ComboManager.Instance;
				instance.WeaponComboEvent = (ComboManager.ComboDelegate)Delegate.Combine(instance.WeaponComboEvent, new ComboManager.ComboDelegate(CheckForComplite));
			}
			else
			{
				ComboManager instance2 = ComboManager.Instance;
				instance2.OverallComboEvent = (ComboManager.ComboDelegate)Delegate.Combine(instance2.OverallComboEvent, new ComboManager.ComboDelegate(CheckForComplite));
			}
		}

		public void CheckForComplite(ComboManager.ComboInfo comboInfo)
		{
			if (!WeaponDependent || RequiredWeapon == WeaponNameList.Any || comboInfo.WeaponNameInList == RequiredWeapon)
			{
				ComboManager.Instance.UpdateComboMeter(comboInfo.ComboMultiplier, comboInfo.WeaponNameInList.ToString());
				if (comboInfo.ComboMultiplier > maxComboAchieved)
				{
					maxComboAchieved = comboInfo.ComboMultiplier;
					if (maxComboAchieved == RequiredComboCount)
					{
						CurrentQwest.MoveToTask(NextTask);
					}
				}
			}
			else
			{
				ComboManager.Instance.UpdateComboMeter(0, string.Empty, turnOff: true);
			}
		}

		public override void Finished()
		{
			base.Finished();
			if (WeaponDependent)
			{
				ComboManager instance = ComboManager.Instance;
				instance.WeaponComboEvent = (ComboManager.ComboDelegate)Delegate.Remove(instance.WeaponComboEvent, new ComboManager.ComboDelegate(CheckForComplite));
			}
			else
			{
				ComboManager instance2 = ComboManager.Instance;
				instance2.OverallComboEvent = (ComboManager.ComboDelegate)Delegate.Remove(instance2.OverallComboEvent, new ComboManager.ComboDelegate(CheckForComplite));
			}
			ComboManager.Instance.UpdateComboMeter(0, string.Empty, turnOff: true);
		}

		public override string TaskStatus()
		{
			return base.TaskStatus() + "\nMax combo achieved with " + RequiredWeapon + ": X" + maxComboAchieved + "/" + RequiredComboCount;
		}
	}
}
