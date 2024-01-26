using Game.Character.CharacterController;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Character
{
	public class PlayerSurfaceSensor : SurfaceSensor
	{
		[Space(10f)]
		public Player player;

		private UnderwaterEffect underwaterEffect;

		protected override void Awake()
		{
			base.Awake();
			underwaterEffect = UnderwaterEffect.Instance;
		}

		public override void Init()
		{
			base.Init();
			if ((bool)player)
			{
				PlayerDieManager.Instance.PlayerResurrectEvent = OnResurrect;
			}
		}

		private void OnResurrect(float resurrectionTime)
		{
			CheckWaterTriggers();
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
			if ((bool)player && PlayerInteractionsManager.Instance.inVehicle)
			{
				player.CheckDrowning();
			}
			if (underwaterEffect != null && (base.AboveWater || base.InWater))
			{
				underwaterEffect.SetDepth(base.CurrWaterSurfaceHeight);
			}
		}
	}
}
