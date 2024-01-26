using UnityEngine;

public class BaseGameObjectsVFX : BaseFX
{
	[Separator("BaseGameObjectsVFX")]
	public bool DebugLog_BaseGameObjectsVFX;

	[Space(10f)]
	public GameObject[] GameObjects;

	public override void StartEffect()
	{
		ActivateFX();
		base.StartEffect();
	}

	public override void StopEffect()
	{
		DeactivateFX();
		base.StopEffect();
	}

	public override void ActivateFX()
	{
		GameObject[] gameObjects = GameObjects;
		foreach (GameObject gameObject in gameObjects)
		{
			gameObject.SetActive(value: true);
		}
	}

	public override void DeactivateFX()
	{
		GameObject[] gameObjects = GameObjects;
		foreach (GameObject gameObject in gameObjects)
		{
			gameObject.SetActive(value: false);
		}
	}
}
