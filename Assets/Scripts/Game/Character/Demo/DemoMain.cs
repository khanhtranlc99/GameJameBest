using Game.Character.CameraEffects;
using Game.Character.CharacterController;
using Game.Character.Extras;
using Game.Character.Input;
using System;
using UnityEngine;

namespace Game.Character.Demo
{
	public class DemoMain : MonoBehaviour
	{
		public Player Player;

		public Vector2 effectsGUIPos;

		public Vector2 gameModesGUIPos;

		private bool displayEffects;

		private bool showGameModes;

		private void Awake()
		{
			Application.targetFrameRate = 60;
			UnityEngine.Random.InitState(DateTime.Now.TimeOfDay.Milliseconds);
		}

		private void Start()
		{
			if (!Player && (bool)CameraManager.Instance.CameraTarget)
			{
				Player = CameraManager.Instance.CameraTarget.gameObject.GetComponent<Player>();
			}
			SetupThirdPerson();
		}

		private void SetupThirdPerson()
		{
			CameraManager.Instance.SetCameraTarget(Player.gameObject.transform);
			InputManager.Instance.SetInputPreset(InputPreset.ThirdPerson);
			RTSProjector.Instance.Disable();
		}

		private void SetupFPS()
		{
			CameraManager.Instance.SetCameraTarget(Player.gameObject.transform);
			InputManager.Instance.SetInputPreset(InputPreset.FPS);
			RTSProjector.Instance.Disable();
		}

		private void SetupRTS()
		{
			InputManager.Instance.SetInputPreset(InputPreset.RTS);
			RTSProjector.Instance.Enable();
			CameraManager.Instance.SetCameraTarget(Player.gameObject.transform);
			TargetManager.Instance.HideCrosshair = true;
		}

		private void SetupRPG()
		{
			InputManager.Instance.SetInputPreset(InputPreset.RPG);
			RTSProjector.Instance.Enable();
			CameraManager.Instance.SetCameraTarget(Player.gameObject.transform);
			TargetManager.Instance.HideCrosshair = true;
		}

		private void SetupOrbit()
		{
			InputManager.Instance.SetInputPreset(InputPreset.Orbit);
			RTSProjector.Instance.Disable();
			TargetManager.Instance.HideCrosshair = true;
		}

		private void SetupLookAt()
		{
		}

		private void ShowGameModes()
		{
			float num = gameModesGUIPos.y + 30f;
			float x = gameModesGUIPos.x;
			if (GUI.Button(new Rect(x, num + 10f, 100f, 30f), "ThirdPerson"))
			{
				SetupThirdPerson();
			}
			if (GUI.Button(new Rect(x, num + 50f, 100f, 30f), "RTS"))
			{
				SetupRTS();
			}
			if (GUI.Button(new Rect(x, num + 90f, 100f, 30f), "RPG"))
			{
				SetupRPG();
			}
			if (GUI.Button(new Rect(x, num + 130f, 100f, 30f), "Orbit"))
			{
				SetupOrbit();
			}
			if (GUI.Button(new Rect(x, num + 170f, 100f, 30f), "LookAt"))
			{
				SetupLookAt();
			}
		}

		private void DisplayEffects()
		{
			EffectManager instance = EffectManager.Instance;
			float num = effectsGUIPos.y + 30f;
			float x = effectsGUIPos.x;
			if (GUI.Button(new Rect(x, num + 10f, 100f, 30f), "Earthquake"))
			{
				Earthquake earthquake = instance.Create<Earthquake>();
				earthquake.Play();
			}
			if (GUI.Button(new Rect(x, num + 50f, 100f, 30f), "Yes"))
			{
				Yes yes = instance.Create<Yes>();
				yes.Play();
			}
			if (GUI.Button(new Rect(x, num + 90f, 100f, 30f), "No"))
			{
				No no = instance.Create<No>();
				no.Play();
			}
			if (GUI.Button(new Rect(x, num + 130f, 100f, 30f), "FireKick"))
			{
				FireKick fireKick = instance.Create<FireKick>();
				fireKick.Play();
			}
			if (GUI.Button(new Rect(x, num + 170f, 100f, 30f), "Stomp"))
			{
				Stomp stomp = instance.Create<Stomp>();
				stomp.Play();
			}
			if (GUI.Button(new Rect(x, num + 210f, 100f, 30f), "Fall"))
			{
				Fall fall = instance.Create<Fall>();
				fall.ImpactVelocity = 2f;
				fall.Play();
			}
			if (GUI.Button(new Rect(x, num + 250f, 100f, 30f), "Explosion"))
			{
				ExplosionCFX explosionCFX = instance.Create<ExplosionCFX>();
				explosionCFX.position = CameraManager.Instance.UnityCamera.transform.position + UnityEngine.Random.insideUnitSphere * 2f;
				explosionCFX.position.y = 0f;
				explosionCFX.Play();
			}
			if (GUI.Button(new Rect(x, num + 290f, 100f, 30f), "Sprint"))
			{
				SprintShake sprintShake = instance.Create<SprintShake>();
				sprintShake.Play();
			}
		}

		private void OnGUI()
		{
			displayEffects = GUI.Toggle(new Rect(effectsGUIPos.x, effectsGUIPos.y, 150f, 30f), displayEffects, "Camera effects");
			if (displayEffects)
			{
				DisplayEffects();
			}
			showGameModes = GUI.Toggle(new Rect(gameModesGUIPos.x, gameModesGUIPos.y, 150f, 30f), showGameModes, "Game modes");
			if (showGameModes)
			{
				ShowGameModes();
			}
		}
	}
}
