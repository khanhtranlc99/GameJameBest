using Game.Character.CharacterController;
using Game.Character.CharacterController.Enums;
using Game.Character.Extras;
using Game.Character.Stats;
using System;
using System.Collections;
using UnityEngine;

namespace Game.Weapons
{
	public class MeleeWeapon : Weapon
	{
		public enum MeleeWeaponType
		{
			Hand,
			Knife,
			DoubleHand,
			BattleAxe
		}

		[Serializable]
		public class MeleeAnim
		{
			public MeleeAttackState AnimationState;

			public MeleeDamageTrigger DamageTrigger;

			public float AnimationLength;
		}

		private const int AlarmRange = 5;

		[Separator("Melee weapon settings")]
		public MeleeWeaponType MeleeType;

		public MeleeAnim[] MeleeAnimations;

		private int animationRandom;

		private float fistPlayerAttackSpeed = 1f;

		private float meleeWeaponPlayerAttackSpeed = 1f;

		private float baseDelay;

		private float[] animationsLength;

		private Player player;

		protected override void Start()
		{
			base.Start();
			Archetype = WeaponArchetype.Melee;
			player = (base.WeaponOwner as Player);
			InitUpgrades();
		}

		private void InitUpgrades()
		{
			animationsLength = new float[MeleeAnimations.Length];
			baseDelay = AttackDelay;
			for (int i = 0; i < MeleeAnimations.Length; i++)
			{
				MeleeAnim meleeAnim = MeleeAnimations[i];
				meleeAnim.DamageTrigger.Init(this);
				meleeAnim.DamageTrigger.Damage = Damage;
				meleeAnim.DamageTrigger.SetAttackStatus(status: false);
				animationsLength[i] = meleeAnim.AnimationLength;
			}
			if ((bool)player)
			{
				meleeWeaponPlayerAttackSpeed = player.stats.GetPlayerStat(StatsList.MeleeWeaponAttackSpeed);
				UpdateDelay(meleeWeaponPlayerAttackSpeed);
			}
		}

		public void UpdateStats(Player player)
		{
			float playerStat = player.stats.GetPlayerStat(StatsList.MeleeWeaponAttackSpeed);
			if (playerStat > meleeWeaponPlayerAttackSpeed)
			{
				UpdateDelay(playerStat);
				meleeWeaponPlayerAttackSpeed = playerStat;
			}
		}

		private void UpdateDelay(float attackSpeed)
		{
			if (animationsLength != null)
			{
				AttackDelay = baseDelay / attackSpeed;
				for (int i = 0; i < MeleeAnimations.Length; i++)
				{
					MeleeAnim meleeAnim = MeleeAnimations[i];
					meleeAnim.AnimationLength = animationsLength[i] / attackSpeed;
				}
			}
		}

		public MeleeAttackState GetMeleeAttackState()
		{
			if (base.IsOnCooldown)
			{
				return MeleeAttackState.Idle;
			}
			if (MeleeAnimations.Length > 0)
			{
				animationRandom = UnityEngine.Random.Range(0, MeleeAnimations.Length);
				return MeleeAnimations[animationRandom].AnimationState;
			}
			return MeleeAttackState.None;
		}

		private IEnumerator TriggerController(MeleeDamageTrigger trigger, float timer, bool status)
		{
			yield return new WaitForSeconds(timer);
			trigger.SetAttackStatus(status);
		}

		public override void Init()
		{
			base.Init();
			DamageTriggerActivator(activate: true);
		}

		public override void DeInit()
		{
			base.DeInit();
			DamageTriggerActivator(activate: false);
		}

		private void DamageTriggerActivator(bool activate)
		{
			MeleeAnim[] meleeAnimations = MeleeAnimations;
			foreach (MeleeAnim meleeAnim in meleeAnimations)
			{
				meleeAnim.DamageTrigger.SetAttackStatus(status: false);
				meleeAnim.DamageTrigger.gameObject.SetActive(activate);
				if (activate)
				{
					meleeAnim.DamageTrigger.Damage = Damage;
				}
			}
		}

		public void MeleeAttack(int attackState)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			MeleeAnim[] meleeAnimations = MeleeAnimations;
			foreach (MeleeAnim meleeAnim in meleeAnimations)
			{
				if (meleeAnim.AnimationState == (MeleeAttackState)attackState)
				{
					lastAttackTime = Time.time;
					StartCoroutine(TriggerController(meleeAnim.DamageTrigger, meleeAnim.AnimationLength / 5f, status: true));
					StartCoroutine(TriggerController(meleeAnim.DamageTrigger, meleeAnim.AnimationLength, status: false));
					if (PerformAttackEvent != null)
					{
						PerformAttackEvent(this);
					}
				}
			}
			HitAlarm();
		}

		private void HitAlarm()
		{
			if ((bool)player)
			{
				EntityManager.Instance.OverallAlarm(player, null, base.transform.position, 5f);
			}
		}

		public override void Attack(HitEntity owner)
		{
		}

		public override void Attack(HitEntity owner, Vector3 direction)
		{
		}

		public override void Attack(HitEntity owner, HitEntity victim)
		{
		}

		public void SetDamage(float newDamage)
		{
			Damage = newDamage;
			for (int i = 0; i < MeleeAnimations.Length; i++)
			{
				MeleeAnimations[i].DamageTrigger.Damage = Damage;
			}
		}
	}
}
