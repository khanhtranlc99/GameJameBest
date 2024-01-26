using Game.Character;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using System;
using UnityEngine;

namespace Game.Enemy
{
	public class BaseNPC : MonoBehaviour, IInitable
	{
		public enum NPCControllerType
		{
			None = -1,
			Simple,
			Smart,
			Pedestrian
		}

		[Serializable]
		public class NPCControllerLink
		{
			public NPCControllerType ControllerType;

			public BaseControllerNPC Controller;
		}

		private const int ToCheckSectorTime = 1;

		public NPCControllerLink[] Controllers;

		public NPCControllerType QuietControllerType;

		[Separator("Public Links")]
		public Rigidbody NPCRigidbody;

		public Animator NPCAnimator;

		public GameObject RootModel;

		public SpecificNPCLinks SpecificNpcLinks;

		public BaseStatusNPC StatusNpc;

		public TalkingObject NPCPhrases;

		public WaterSensor WaterSensor;

		private NPCControllerType currentControllerType = NPCControllerType.None;

		private BaseControllerNPC currentNpcController;

		private SlowUpdateProc slowUpdateProc;

		private float toCheckSectorTimer;

		public BaseControllerNPC CurrentController => currentNpcController;

		public NPCControllerType CurrentControllerType => currentControllerType;

		public virtual void OnAlarm(HitEntity disturber)
		{
			if (FactionsManager.Instance.GetRelations(disturber.Faction, StatusNpc.Faction) != 0)
			{
				ChangeController(NPCControllerType.Smart);
				SmartHumanoidController smartHumanoidController = CurrentController as SmartHumanoidController;
				if ((bool)smartHumanoidController)
				{
					smartHumanoidController.AddPersonalTarget(disturber);
					smartHumanoidController.InitBackToDummyLogic();
				}
			}
		}

		public virtual void Init()
		{
			ChangeController(QuietControllerType);
			toCheckSectorTimer = 1f;
		}

		public virtual void DeInit()
		{
			DeInitCurrentController();
		}

		public virtual void ChangeController(NPCControllerType newControllerType)
		{
			BaseControllerNPC _;
			ChangeController(newControllerType, out _);
		}

		public virtual void ChangeController(NPCControllerType newControllerType, out BaseControllerNPC controller)
		{
			if (currentControllerType == newControllerType)
			{
				controller = currentNpcController;
				return;
			}
			DeInitCurrentController();
			BaseControllerNPC baseControllerNPC = null;
			NPCControllerLink[] controllers = Controllers;
			foreach (NPCControllerLink nPCControllerLink in controllers)
			{
				if (nPCControllerLink.ControllerType == newControllerType)
				{
					baseControllerNPC = nPCControllerLink.Controller;
					break;
				}
			}
			if (baseControllerNPC == null)
			{
				throw new Exception("Controller type '" + newControllerType + "' for NPC '" + base.gameObject.name + "' not assigned!");
			}
			InitController(baseControllerNPC);
			currentControllerType = newControllerType;
			controller = currentNpcController;
		}

		public void TryTalk(TalkingObject.PhraseType type)
		{
			NPCPhrases.TalkPhraseOfType(type);
		}

		private void Awake()
		{
			slowUpdateProc = new SlowUpdateProc(SlowUpdate, 1f);
			if (WaterSensor == null)
			{
				WaterSensor = GetComponentInChildren<WaterSensor>();
			}
		}

		private void FixedUpdate()
		{
			slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void SlowUpdate()
		{
			if (toCheckSectorTimer > 0f)
			{
				toCheckSectorTimer -= slowUpdateProc.DeltaTime;
			}
			else if (!SectorManager.Instance.IsInActiveSector(base.transform.position) || PlayerInteractionsManager.Instance.IsPlayerInMetro)
			{
				PoolManager.Instance.ReturnToPool(this);
			}
		}

		private void DeInitCurrentController()
		{
			if (!(currentNpcController == null))
			{
				currentNpcController.DeInit();
				PoolManager.Instance.ReturnToPool(currentNpcController);
				currentNpcController = null;
				currentControllerType = NPCControllerType.None;
			}
		}

		private void InitController(BaseControllerNPC newController)
		{
			currentNpcController = PoolManager.Instance.GetFromPool(newController, base.transform.position, base.transform.rotation);
			currentNpcController.transform.parent = base.transform;
			currentNpcController.Init(this);
		}

		private void OnTriggerStay()
		{
		}
	}
}
