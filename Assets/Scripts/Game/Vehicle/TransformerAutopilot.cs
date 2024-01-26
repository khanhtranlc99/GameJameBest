using Game.Character;
using Game.Character.CharacterController;
using Game.Enemy;
using Game.GlobalComponent;
using Game.Traffic;
using System;
using UnityEngine;

namespace Game.Vehicle
{
	public class TransformerAutopilot : Autopilot
	{
		private GameObject transformer;

		public override void DropPassangers()
		{
			DriverExit = true;
			rootBody.velocity = Vector3.zero;
			isWorking = false;
			if (base.wheels != null)
			{
				WheelCollider[] wheels = base.wheels;
				foreach (WheelCollider wheelCollider in wheels)
				{
					wheelCollider.brakeTorque = 10f;
				}
			}

			NavMeshAgent.isStopped = true;
			transformer = PoolManager.Instance.GetFromPool(rootBody.GetComponent<CarTransformer>().NPCRobotPrefab);
			PoolManager.Instance.AddBeforeReturnEvent(transformer, delegate(GameObject poolingObject)
			{
				TrafficManager.Instance.FreeTransformerVehicleSlot();
				poolingObject.GetComponent<Transformer>().DeInit();
			});
			transformer.transform.position = base.transform.parent.transform.position;
			HitEntity component = transformer.GetComponent<HitEntity>();
			component.DiedEvent = (HitEntity.AliveStateChagedEvent)Delegate.Combine(component.DiedEvent, new HitEntity.AliveStateChagedEvent(DropedCopKillEvent));
			Transformer component2 = transformer.GetComponent<Transformer>();
			HumanoidNPC component3 = transformer.GetComponent<HumanoidNPC>();
			component3.WaterSensor.Reset();
			component2.currentForm = TransformerForm.Car;
			component2.Transform(rootBody.gameObject, PlayerInteractionsManager.Instance.Player);
			TrafficManager.Instance.TakeTransformerVehicleSlot();
			PoolManager.Instance.ReturnToPool(this);
		}

		public override void DropedCopKillEvent()
		{
			DriverWasKilled = true;
		}

		public override void ChangeDropedCopKillEvent()
		{
			HitEntity component = transformer.GetComponent<HitEntity>();
			component.DiedEvent = (HitEntity.AliveStateChagedEvent)Delegate.Remove(component.DiedEvent, new HitEntity.AliveStateChagedEvent(DropedCopKillEvent));
		}
	}
}
