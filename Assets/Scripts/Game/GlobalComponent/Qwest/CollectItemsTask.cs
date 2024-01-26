using Game.Character.CharacterController;
using Game.Enemy;
using Game.PickUps;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class CollectItemsTask : BaseTask
	{
		public int InitialCountToCollect;

		public QwestPickupType PickupType;

		public Faction TargetFaction;

		private int itemsCollected;

		public int MarksCount;

		public string MarksTypeNPC;

		public string MarksTypePickUp;

		private GameObject searchGo;

		public override void TaskStart()
		{
			searchGo = new GameObject
			{
				name = "SearchProcessing_" + GetType().Name
			};
			SearchProcess<QuestPickUp> searchProcess = new SearchProcess<QuestPickUp>(CheckConditionPickUp);
			searchProcess.countMarks = MarksCount;
			searchProcess.markType = MarksTypePickUp;
			SearchProcess<QuestPickUp> process = searchProcess;
			SearchProcessing searchProcessing = searchGo.AddComponent<SearchProcessing>();
			searchProcessing.process = process;
			searchProcessing.Init();
			SearchProcess<HumanoidStatusNPC> searchProcess2 = new SearchProcess<HumanoidStatusNPC>(CheckConditionNPC);
			searchProcess2.countMarks = MarksCount;
			searchProcess2.markType = MarksTypeNPC;
			SearchProcess<HumanoidStatusNPC> process2 = searchProcess2;
			SearchProcessing searchProcessing2 = searchGo.AddComponent<SearchProcessing>();
			searchProcessing2.process = process2;
			searchProcessing2.Init();
			base.TaskStart();
			itemsCollected = 0;
		}

		public override void NpcKilledEvent(Vector3 position, Faction npcFaction, HitEntity victim, HitEntity killer)
		{
			if (!npcFaction.Equals(TargetFaction))
			{
				return;
			}
			int num = 0;
			QuestPickUp questPickUp;
			while (true)
			{
				if (num < GameEventManager.Instance.questPickUps.Length)
				{
					questPickUp = GameEventManager.Instance.questPickUps[num];
					if (questPickUp.type.Equals(PickupType))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			QuestPickUp qwestItem = PoolManager.Instance.GetFromPool(questPickUp);
			PoolManager.Instance.AddBeforeReturnEvent(qwestItem, delegate
			{
				qwestItem.DeInit();
			});
			qwestItem.transform.parent = GameEventManager.Instance.transform;
			qwestItem.RelatedTask = this;
			Vector2 insideUnitCircle = Random.insideUnitCircle;
			qwestItem.transform.position = position + new Vector3(insideUnitCircle.x, 1f, insideUnitCircle.y);
		}

		public override void PickedQwestItemEvent(Vector3 position, QwestPickupType pickupType, BaseTask relatedTask)
		{
			if (Equals(relatedTask) && pickupType.Equals(PickupType))
			{
				itemsCollected++;
				if (itemsCollected >= InitialCountToCollect)
				{
					CurrentQwest.MoveToTask(NextTask);
				}
			}
		}

		public override void PlayerDeadEvent(SuicideAchievment.DethType i = SuicideAchievment.DethType.None)
		{
			itemsCollected = 0;
		}

		public override string TaskStatus()
		{
			return base.TaskStatus() + "\nItem collected " + itemsCollected + "/" + InitialCountToCollect;
		}

		public override void Finished()
		{
			if ((bool)searchGo)
			{
				UnityEngine.Object.Destroy(searchGo);
			}
			base.Finished();
		}

		private bool CheckConditionPickUp(QuestPickUp pickup)
		{
			return pickup.type.Equals(PickupType);
		}

		private bool CheckConditionNPC(HumanoidStatusNPC npc)
		{
			return npc.Faction == TargetFaction;
		}
	}
}
