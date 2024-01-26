using Game.Character;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using Game.Vehicle;
using UnityEngine;

namespace Game.Enemy
{
	public class DriverStatus : HitEntity
	{
		private GameObject driver;

		private DummyDriver dummyDriver;

		private Player playerScript;

		public bool IsPlayer
		{
			get;
			protected set;
		}

		public bool InArmoredVehicle
		{
			get;
			protected set;
		}

		public void Init(GameObject originalDriverGO, bool Vulnerable)
		{
			base.Initialization();
			InArmoredVehicle = !Vulnerable;
			Dead = false;
			dummyDriver = originalDriverGO.GetComponent<DummyDriver>();
			if (dummyDriver == null)
			{
				playerScript = originalDriverGO.GetComponent<Player>();
				if (playerScript != null)
				{
					Health.Max = playerScript.Health.Max;
					Health.Current = playerScript.Health.Current;
					Defence = playerScript.Defence;
					Faction = playerScript.Faction;
					IsPlayer = true;
				}
			}
			else
			{
				BaseStatusNPC component = dummyDriver.DriverNPC.GetComponent<BaseStatusNPC>();
				Health.Max = component.Health.Max;
				Health.Current = component.Health.Current;
				Defence = component.Defence;
				Faction = component.Faction;
				IsPlayer = false;
			}
		}

		public override void OnHit(DamageType damageType, HitEntity owner, float damage, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			if (!InArmoredVehicle || damageType == DamageType.Instant || (IsPlayer && playerScript.IsTransformer))
			{
				if (owner != null && owner.Faction == Faction.Player)
				{
					FactionsManager.Instance.PlayerAttackHuman(this);
				}
				if (IsPlayer)
				{
					playerScript.OnHit(damageType, owner, damage, hitPos, hitVector, defenceReduction);
				}
				else
				{
					base.OnHit(damageType, owner, damage, hitPos, hitVector, defenceReduction);
				}
			}
		}

		protected override void OnDie()
		{
			base.OnDie();
			if (IsPlayer)
			{
				PlayerInteractionsManager.Instance.DieInCar();
			}
			else
			{
				dummyDriver.DriverDie();
			}
			PoolManager.Instance.ReturnToPool(gameObject);
		}
	}
}
