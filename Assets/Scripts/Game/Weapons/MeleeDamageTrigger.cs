using Game.Character.CharacterController;
using Game.Enemy;
using Game.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Weapons
{
	public class MeleeDamageTrigger : MonoBehaviour
	{
		public bool DebugLog;

		public Collider DamageTrigger;

		public float Damage;

		private HitEntity owner;

		private MeleeWeapon parentWeapon;

		private readonly List<HitEntity> hitedObjectsList = new List<HitEntity>();

		public void Init(MeleeWeapon weapon)
		{
			if (DamageTrigger == null)
			{
				DamageTrigger = GetComponent<Collider>();
			}
			owner = GetComponentInParent<HitEntity>();
			parentWeapon = weapon;
		}

		public void SetAttackStatus(bool status)
		{
			if (DamageTrigger == null)
			{
				DamageTrigger = GetComponent<Collider>();
			}
			DamageTrigger.enabled = status;
			if (status)
			{
				hitedObjectsList.Clear();
			}
		}

		private void OnTriggerEnter(Collider col)
		{
			HitEntity hitEntity = col.GetComponent<HitEntity>();
			BodyPartDamageReciever bodyPartDamageReciever = hitEntity as BodyPartDamageReciever;
			RagdollDamageReciever ragdollDamageReciever = hitEntity as RagdollDamageReciever;
			if (bodyPartDamageReciever != null)
			{
				hitEntity = bodyPartDamageReciever.RerouteEntity;
			}
			if (ragdollDamageReciever != null)
			{
				hitEntity = ragdollDamageReciever.RdStatus;
			}
			if (!(hitEntity != null) || !(hitEntity != owner) || hitedObjectsList.Contains(hitEntity))
			{
				return;
			}
			if (DebugLog && GameManager.ShowDebugs)
			{
				Debug.Log("Boom");
			}
			if (FactionsManager.Instance.GetRelations(owner.Faction, hitEntity.Faction) != 0)
			{
				Vector3 vector = hitEntity.transform.position + hitEntity.NPCShootVectorOffset - base.transform.position;
				if (parentWeapon.InflictDamageEvent != null)
				{
					parentWeapon.InflictDamageEvent(parentWeapon, owner, hitEntity, base.transform.position, vector.normalized, 0f);
				}
				parentWeapon.PlayHitSound(base.transform.position);
				hitedObjectsList.Add(hitEntity);
			}
		}
	}
}
