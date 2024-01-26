using Game.Character.Stats;
using Game.Items;
using Game.Shop;
using Prefabs.Managers.Shop.Prefabs;
using UnityEngine;

namespace Code.Game.Shop.InfoPanels
{
	public class ShopInfoPanelVehicles : ShopInfoPanel
	{
		[SerializeField]
		private StatViewVehicles statViewPrefab;

		[SerializeField]
		private AdditionalFeatureVehicles additionalFeaturePrefab;

		[SerializeField]
		private RectTransform statViewHolder;

		[SerializeField]
		private StatIconList statDefinitions;

		public override void ShowInfo(GameItem incItem)
		{
			base.ShowInfo(incItem);
			statViewHolder.DestroyChildrens();
			GameItemVehicle gameItemVehicle = incItem as GameItemVehicle;
			if (gameItemVehicle == null)
			{
				UnityEngine.Debug.LogWarning("Пытался показать инфо о скине для неизвестного типа предмета.");
				return;
			}
			StatAttribute[] statAttributes = gameItemVehicle.StatAttributes;
			for (int i = 0; i < statAttributes.Length; i++)
			{
				StatAttribute statAttribute = statAttributes[i];
				StatViewVehicles statViewVehicles = Object.Instantiate(statViewPrefab, statViewHolder, worldPositionStays: false) as StatViewVehicles;
				switch (statAttribute.StatType)
				{
				case StatsList.CarAcceleration:
					statViewVehicles.SetMaxAmount(GameItemVehicle.maxAcceleration);
					break;
				case StatsList.DrivingMaxSpeed:
					statViewVehicles.SetMaxAmount(GameItemVehicle.maxSpeed);
					break;
				case StatsList.CarHealth:
					statViewVehicles.SetMaxAmount(GameItemVehicle.maxHealth);
					break;
				}
				StatIcon definition = statDefinitions.GetDefinition(statAttribute.StatType);
				statViewVehicles.SetIcon(definition.Icon);
				statViewVehicles.SetNameStat(definition.ShowedName);
				statViewVehicles.SetValue(statAttribute.GetStatValue);
			}
			AdditionalFeature[] additionalFeatures = gameItemVehicle.AdditionalFeatures;
			foreach (AdditionalFeature additionalFeature in additionalFeatures)
			{
				AdditionalFeatureVehicles additionalFeatureVehicles = Object.Instantiate(additionalFeaturePrefab, statViewHolder, worldPositionStays: false) as AdditionalFeatureVehicles;
				additionalFeatureVehicles.SetImage(additionalFeature.GetSprite());
				additionalFeatureVehicles.SetDescription(additionalFeature.GetDescription());
			}
		}
	}
}
