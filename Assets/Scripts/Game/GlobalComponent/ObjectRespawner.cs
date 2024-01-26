using Game.Character.CharacterController;
using Game.Character.Extras;
using Game.Enemy;
using Game.PickUps;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class ObjectRespawner : MonoBehaviour
	{
		public GameObject ObjectPrefab;

		public RespawnedObjectType ObjectType;

		public float RespawnTime = 60f;

		[Tooltip("Заспавнится в виде смарта")]
		public bool SpawnGuardNpc;

		[Tooltip("Будет пиздеть всех \"не друзей\", даже нейтралов")]
		public bool SpawnManiacNpc;

		[Space(10f)]
		public bool isTaken;

		public bool DebugVal;

		protected GameObject controlledObject;

		protected float lostControllTime;

		private bool awaked;

		public void SetCollectionRespawner()
		{
			ObjectType = RespawnedObjectType.CollectionItem;
		}

		public void SetIsTaken(bool val)
		{
			isTaken = val;
		}

		private void Awake()
		{
			if (awaked)
			{
				return;
			}
			UnityEngine.Object.DestroyImmediate(base.transform.GetChild(0).gameObject);
			lostControllTime = Time.time - RespawnTime;
			awaked = true;
			if (ObjectPrefab.GetComponent<CollectionPickup>() != null)
			{
				SetCollectionRespawner();
				if (DebugVal)
				{
					UnityEngine.Debug.LogFormat(this, "isTaken = " + isTaken);
				}
			}
		}

		protected void OnEnable()
		{
			if (!awaked)
			{
				Awake();
			}
			if (controlledObject == null && Time.time >= lostControllTime + RespawnTime)
			{
				if (ObjectType == RespawnedObjectType.CollectionItem && isTaken)
				{
					return;
				}
				controlledObject = PoolManager.Instance.GetFromPool(ObjectPrefab, base.transform.position, base.transform.rotation);
				HitEntity controlledHitEntity = null;
				PickUp controlledPickup = null;
				if (ObjectType == RespawnedObjectType.Entity)
				{
					controlledHitEntity = controlledObject.GetComponentInChildren<HitEntity>();
					BaseNPC component = controlledObject.GetComponent<BaseNPC>();
					if (component != null)
					{
						component.QuietControllerType = BaseNPC.NPCControllerType.Simple;
						BaseControllerNPC controller;
						component.ChangeController(SpawnGuardNpc ? BaseNPC.NPCControllerType.Smart : BaseNPC.NPCControllerType.Simple, out controller);
						SmartHumanoidController smartHumanoidController = controller as SmartHumanoidController;
						if (smartHumanoidController != null)
						{
							if (!SpawnGuardNpc)
							{
								smartHumanoidController.InitBackToDummyLogic();
							}
							smartHumanoidController.IsFCKingManiac = (SpawnManiacNpc || SpawnGuardNpc);
						}
					}
				}
				else
				{
					controlledPickup = controlledObject.GetComponent<PickUp>();
				}
				PoolManager.Instance.AddBeforeReturnEvent(controlledObject, delegate
				{
					if ((ObjectType != 0) ? PickUpManager.Instance.PickupWasTaked(controlledPickup) : EntityManager.Instance.EntityWasKilled(controlledHitEntity))
					{
						lostControllTime = Time.time;
					}
					controlledObject = null;
				});
			}
			if (controlledObject != null)
			{
				controlledObject.transform.parent = base.transform.parent;
				controlledObject.SetActive(value: true);
			}
			if (ObjectType == RespawnedObjectType.CollectionItem)
			{
				CollectionPickUpsManager.Instance.OnPickupCreate(this, controlledObject);
			}
			if (DebugVal)
			{
				UnityEngine.Debug.LogFormat(controlledObject, "controlledObject = " + controlledObject);
				UnityEngine.Debug.LogFormat(this, "respawner = " + this);
			}
		}
	}
}
