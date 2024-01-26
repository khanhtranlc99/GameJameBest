using Game.GlobalComponent;
using Game.Weapons;

namespace Game.PickUps
{
	public class BulletsPickup : PickUp
	{
		public AmmoTypes BulletType;

		protected override void TakePickUp()
		{
			AmmoManager.Instance.AddAmmo(BulletType);
			InGameLogManager.Instance.RegisterNewMessage(MessageType.Bullets, BulletType.ToString());
			base.TakePickUp();
		}
	}
}
