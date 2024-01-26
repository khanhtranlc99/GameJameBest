using Game.Character;
using Game.Character.Modes;
using Game.GlobalComponent;
using UnityEngine;

public class CustscenCameraControl : Cutscene
{
	public CameraMode CameraMode;

	public Transform Target;

	public Transform CameraPosition;

	private Transform cameraTransform;

	private CameraMode cameraMode;

	private bool isChangeMode;

	public override void StartScene()
	{
		IsPlaying = true;
		Target = PlayerInteractionsManager.Instance.Player.transform;
		cameraMode = PoolManager.Instance.GetFromPool(CameraMode);
		CameraManager.Instance.SetMode(cameraMode);
		CameraManager.Instance.SetCameraTarget(Target);
		cameraTransform = CameraManager.Instance.UnityCamera.transform;
		cameraTransform.position = CameraPosition.position;
		isChangeMode = true;
		if (mainManager.Scenes.Length <= mainManager.CurrentIndex() + 1)
		{
			return;
		}
		Cutscene[] scenes = mainManager.Scenes[mainManager.CurrentIndex() + 1].Scenes;
		foreach (Cutscene cutscene in scenes)
		{
			if ((bool)(cutscene as CustscenCameraControl))
			{
				isChangeMode = false;
			}
		}
	}

	private void LateUpdate()
	{
		if (IsPlaying)
		{
			cameraTransform.position = CameraPosition.position;
		}
	}

	public override void EndScene(bool isCheck)
	{
		base.EndScene(isCheck);
		PoolManager.Instance.ReturnToPool(CameraMode);
		if (isChangeMode || !mainManager.Inited)
		{
			CameraManager.Instance.SetMode(CameraManager.Instance.ActivateModeOnStart);
		}
	}
}
