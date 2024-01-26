using Game.Character.CharacterController;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Items
{
	//[CreateAssetMenu(fileName = "ItemPlayerChanger", menuName = "RopeData/ItemData/PlayerChanger", order = 100)]
	public class GameItemPlayerChanger : GameItemPowerUp
	{
		public GameObject NewPlayerPrefab;

		[Tooltip("Ширина, высота, глубина модели")]
		public Vector3 ModelSizeVector = new Vector3(1f, 2f, 1f);

		public override bool CanBeEquiped
		{
			get
			{
				Transform transform = PlayerManager.Instance.Player.transform;
				Vector3 vector = transform.position + Vector3.up * 0.05f;
				LayerMask obstaclesLayerMask = PlayerManager.Instance.DefaulAnimationController.ObstaclesLayerMask;
				return base.CanBeEquiped && (base.isActive || (!Physics.Raycast(vector - transform.right, transform.right, ModelSizeVector.x, obstaclesLayerMask) && !Physics.Raycast(vector, transform.up, ModelSizeVector.y, obstaclesLayerMask) && !Physics.Raycast(vector - transform.forward, transform.forward, ModelSizeVector.z, obstaclesLayerMask) && !PlayerManager.Instance.Player.IsDead));
			}
		}

		public override void Activate()
		{
			base.Activate();
			GameItemPlayerChanger gameItemPlayerChanger = null;
			List<GameItemPlayerChanger> list = new List<GameItemPlayerChanger>();
			foreach (GameItemPowerUp activePowerUp in StuffManager.ActivePowerUps)
			{
				gameItemPlayerChanger = (activePowerUp as GameItemPlayerChanger);
				if (gameItemPlayerChanger != null && gameItemPlayerChanger != this)
				{
					list.Add(gameItemPlayerChanger);
					gameItemPlayerChanger = null;
				}
			}
			foreach (GameItemPlayerChanger item in list)
			{
				item.Deactivate();
			}
			PlayerManager.Instance.SwitchPlayer(NewPlayerPrefab);
		}

		public override void Deactivate()
		{
			base.Deactivate();
			PlayerManager.Instance.ResetPlayer();
		}

		public override bool SameParametrWithOther(object[] parametrs)
		{
			return NewPlayerPrefab == (GameObject)parametrs[0];
		}
	}
}
