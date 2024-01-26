using Game.Character.CharacterController;
using Game.GlobalComponent;
using Game.GlobalComponent.Qwest;
using Game.Items;
using Game.UI;
using Game.Vehicle;
using System;
using RopeHeroViceCity.UI_Features.UI_InGame.Scripts;
using UnityEngine;
using UnityEngine.UI;

public abstract class Achievement : MonoBehaviour, IEvent
{
	[Serializable]
	public struct SaveLoadAchievmentStruct
	{
		public bool isDone;

		public int achiveCounter;

		public int achiveTarget;

		public SaveLoadAchievmentStruct(bool complite, int counter, int target)
		{
			achiveCounter = counter;
			achiveTarget = target;
			isDone = complite;
		}
	}

	public string achievementName = "NoNameAchievment";

	public string achievementDiscription = "NoNameAchievment";

	public Sprite achievementPicture;

	public SaveLoadAchievmentStruct achiveParams;

	public UniversalReward Rewards;

	public virtual void Init()
	{
		achiveParams = new SaveLoadAchievmentStruct(false, 0, 0);
	}

	public virtual void SaveAchiev()
	{
		BaseProfile.StoreValue(achiveParams, achievementName);
	}

	public virtual void LoadAchiev()
	{
		try
		{
			achiveParams = BaseProfile.ResolveValue(achievementName, achiveParams);
		}
		catch (Exception)
		{
			Init();
			SaveAchiev();
		}
	}

	public virtual void AchievComplite()
	{
		achiveParams.isDone = true;
		GameEventManager.Instance.activeAchievements.Remove(this);
		SaveAchiev();
		Transform transform = UI_InGame.Instance.achivementTransform;
		transform.gameObject.SetActive(value: true);
		if (achievementPicture != null)
		{
			transform.Find("Picture").GetComponent<Image>().sprite = achievementPicture;
		}
		else
		{
			transform.Find("Picture").gameObject.SetActive(value: false);
		}
		transform.Find("AchevmentNameText").GetComponent<Text>().text = achievementName;
		PointSoundManager.Instance.PlaySoundAtPoint(PlayerManager.Instance.Player.transform.position, TypeOfSound.GetAchievment);
		Rewards.GiveReward();
		QwestCompletePanel.Instance.ShowCompletedQwestInfo("Achievment unlocked", Rewards);
	}

	public virtual void PlayerDeadEvent(SuicideAchievment.DethType i = SuicideAchievment.DethType.None)
	{
	}

	public virtual void NpcKilledEvent(Vector3 position, Faction npcFaction, HitEntity victim, HitEntity killer)
	{
	}

	public virtual void PickedQwestItemEvent(Vector3 position, QwestPickupType pickupType, BaseTask relatedTask)
	{
	}

	public virtual void PointReachedEvent(Vector3 position, BaseTask task)
	{
	}

	public virtual void PointReachedByVehicleEvent(Vector3 position, BaseTask task, DrivableVehicle vehicle)
	{
	}

	public virtual void GetIntoVehicleEvent(DrivableVehicle vehicle)
	{
	}

	public virtual void GetOutVehicleEvent(DrivableVehicle vehicle)
	{
	}

	public virtual void PickUpCollectionEvent(string collectionName)
	{
	}

	public virtual void GetLevelEvent(int level)
	{
	}

	public virtual void GetShopEvent()
	{
	}

	public virtual void VehicleDrawingEvent(DrivableVehicle vehicle)
	{
	}

	public void BuyItemEvent(GameItem item)
	{
	}
}
