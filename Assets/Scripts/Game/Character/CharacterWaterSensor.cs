using Game.Character.CharacterController;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Character
{
	public class CharacterWaterSensor : WaterSensor
	{
		private const float slowUpdateTime = 0.25f;

		[Space(10f)]
		public bool CWS_DebugLog;

		[Space(10f)]
		public ParticleSystem WaterEffect;

		public float DepthForDrowning = 1.6f;

		public int DrowningDamageMult = 1;

		private HitEntity currentHitEntity;

		private SlowUpdateProc slowUpdateProc;

		protected override void Awake()
		{
			base.Awake();
			if ((bool)WaterEffect)
			{
				var emit = WaterEffect.emission;
				emit.enabled = false;
			}
			slowUpdateProc = new SlowUpdateProc(SlowUpdate, 0.25f);
		}

		public void Init(HitEntity hitEntity)
		{
			base.Init();
			currentHitEntity = hitEntity;
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
			slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void SlowUpdate()
		{
			if(currentHitEntity == null)
				return;
			Vector3 position = currentHitEntity.transform.position;
			if (position.y + DepthForDrowning <= waterHeight)
			{
				currentHitEntity.Drowning(waterHeight - DepthForDrowning, 1f);
			}
			else
			{
				currentHitEntity.IsInWater = false;
			}
			WaterEffects();
		}

		private void WaterEffects()
		{
			if ((bool)WaterEffect)
			{
				Vector3 position = currentHitEntity.transform.position;
				if (position.y + 0.2f <= base.waterHeight)
				{
					var emit = WaterEffect.emission;
					emit.enabled = true;
					Transform transform = WaterEffect.transform;
					Vector3 position2 = base.transform.position;
					float x = position2.x;
					float waterHeight = base.waterHeight;
					Vector3 position3 = base.transform.position;
					transform.position = new Vector3(x, waterHeight, position3.z);
				}
				else
				{
					var emit = WaterEffect.emission;
					emit.enabled = false;
				}
			}
		}
	}
}
