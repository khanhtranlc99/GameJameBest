using Game.GlobalComponent;
using System;

public class GetFabergeCollectionAchievement : Achievement
{
	private CollectionPickUpsManager.CollectionTypes collectionType = CollectionPickUpsManager.CollectionTypes.Hook;

	public override void Init()
	{
		base.Init();
		CollectionPickUpsManager instance = CollectionPickUpsManager.Instance;
		instance.OnManagerInitAction = (Action)Delegate.Combine(instance.OnManagerInitAction, (Action)delegate
		{
			achiveParams.achiveTarget = CollectionPickUpsManager.Instance.totalAmmount[collectionType];
			achiveParams.achiveCounter = CollectionPickUpsManager.Instance.countAmmount[collectionType];
		});
		CollectionPickUpsManager instance2 = CollectionPickUpsManager.Instance;
		instance2.OnElementTakenEvent = (CollectionPickUpsManager.OnElementTaken)Delegate.Combine(instance2.OnElementTakenEvent, new CollectionPickUpsManager.OnElementTaken(OnElementTaken));
	}

	public override void PickUpCollectionEvent(string collectionName)
	{
		if (collectionType.ToString() == collectionName)
		{
			AchievComplite();
		}
	}

	private void OnElementTaken(CollectionPickUpsManager.CollectionTypes type)
	{
		if (type == collectionType)
		{
			achiveParams.achiveCounter++;
		}
	}
}
