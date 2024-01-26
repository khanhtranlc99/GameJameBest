using Game.Character.Extras;
using Game.Managers;
using UnityEngine;

namespace Game.Character
{
	public class WaterSensor : MonoBehaviour
	{
		private const float defaultSurfaceHeight = -1000f;

		public bool DebugLog;

		protected static int WaterLayerNumber = -1;

		[Tooltip("Capsule collider is NOT recomended!")]
		public Collider sensorCollider;

		protected float waterHeight;

		protected SurfaceStatePack currSurfaceStatePack = new SurfaceStatePack(aboveGround: false, aboveWater: false, inWater: false);

		private int waterTriggersCount;

		private Collider lastEnteredTrigger;

		protected bool gameIsStarted;

		public float CurrWaterSurfaceHeight => waterHeight;

		public bool InWater => currSurfaceStatePack.InWater;

		public bool AboveWater => currSurfaceStatePack.AboveWater;

		protected virtual void Awake()
		{
			if (WaterLayerNumber == -1)
			{
				WaterLayerNumber = LayerMask.NameToLayer("Water");
			}
			waterHeight = -1000f;
			if (sensorCollider == null)
			{
				sensorCollider = GetComponent<Collider>();
			}
			
		}

		protected virtual void Start()
		{
			gameIsStarted = true;
		}

		public virtual void Init()
		{
			CheckSurface();
		}

		public virtual void Reset()
		{
			waterTriggersCount = 0;
			waterHeight = -1000f;
			lastEnteredTrigger = null;
			currSurfaceStatePack.SetTypePack(aboveGround: false, aboveWater: false, inWater: false);
		}

		protected virtual void FixedUpdate()
		{
			CheckSurface();
		}

		protected virtual void CheckSurface()
		{
			if (waterTriggersCount > 0)
			{
				currSurfaceStatePack.InWater = true;
			}
			else
			{
				currSurfaceStatePack.InWater = false;
			}
		}

		protected void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.layer == WaterLayerNumber && !(lastEnteredTrigger == other))
			{
				Vector3 position = other.transform.position;
				waterHeight = position.y;
				waterTriggersCount++;
				lastEnteredTrigger = other;
				WaterSpraySFX.Instance.Emit(transform.position);
				if (DebugLog)
				{
					UnityEngine.Debug.Log("Вошел в триггер: " + waterTriggersCount + " " + other.name, other.gameObject);
				}
			}
		}

		protected void OnTriggerExit(Collider other)
		{
			if (other.gameObject.layer == WaterLayerNumber)
			{
				if (lastEnteredTrigger == other)
				{
					lastEnteredTrigger = null;
				}
				waterTriggersCount = ((waterTriggersCount > 0) ? (waterTriggersCount - 1) : 0);
				if (DebugLog && GameManager.ShowDebugs)
				{
					UnityEngine.Debug.Log("Вышел из триггера: " + waterTriggersCount);
				}
			}
		}

		protected void OnEnable()
		{
			if (DebugLog && GameManager.ShowDebugs)
			{
				UnityEngine.Debug.Log("Сенсор воды проснулся.");
			}
			if (gameIsStarted)
			{
				CheckWaterTriggers();
			}
		}

		protected void CheckWaterTriggers()
		{
			
		}
	}
}
