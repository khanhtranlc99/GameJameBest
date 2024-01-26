using Game.GlobalComponent;
using Game.GlobalComponent.Qwest;

namespace Game.PickUps
{
	public class QuestPickUp : PickUp
	{
		public const float QuestItemSpawnY = 1f;

		public QwestPickupType type;

		public BaseTask RelatedTask;

		public void DeInit()
		{
			RelatedTask = null;
		}

		protected override void TakePickUp()
		{
			InGameLogManager.Instance.RegisterNewMessage(MessageType.QuestItem, type.ToString());
			GameEventManager.Instance.Event.PickedQwestItemEvent(base.transform.position, type, RelatedTask);
			base.TakePickUp();
		}
	}
}
