using Game.Character;
using Game.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
	public class ShopInfoPanelPowerUp : ShopInfoPanel
	{
		[Space(5f)]
		public Slider RemainingTimeSlider;

		public Slider RemainingCooldownSlider;

		public override void ShowInfo(GameItem incItem)
		{
			base.ShowInfo(incItem);
			GameItemPowerUp gameItemPowerUp = incItem as GameItemPowerUp;
			if (gameItemPowerUp == null)
			{
				UnityEngine.Debug.LogWarning("Пытался показать инфо PowerUp'а для неизвестного типа предмета.");
				return;
			}
			RemainingTimeSlider.minValue = 0f;
			RemainingTimeSlider.maxValue = gameItemPowerUp.Duration * PlayerInfoManager.Instance.GetBIValue(gameItemPowerUp.ID, gameItemPowerUp.ShopVariables.gemPrice);
			RemainingTimeSlider.value = gameItemPowerUp.RemainingDuration;
			RemainingCooldownSlider.minValue = 0f;
			RemainingCooldownSlider.maxValue = gameItemPowerUp.Cooldawn;
			RemainingCooldownSlider.value = gameItemPowerUp.RemainingCooldawn;
		}
	}
}
