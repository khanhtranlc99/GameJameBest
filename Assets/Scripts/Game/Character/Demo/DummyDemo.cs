using Game.Character.CameraEffects;
using Game.Character.Input;
using Game.Character.Utils;
using System;
using UnityEngine;

namespace Game.Character.Demo
{
	public class DummyDemo : MonoBehaviour
	{
		public Transform player;

		public Vector2 effectsGUIPos;

		public Vector2 gameModesGUIPos;

		public GUISkin skin;

		public bool showEffects = true;

		private bool displayEffects;

		private bool showGameModes;

		private void Awake()
		{
			Application.targetFrameRate = 60;
			UnityEngine.Random.InitState(DateTime.Now.TimeOfDay.Milliseconds);
		}

		private void Update()
		{
		}

		private void Start()
		{
		}

		private void SetupThirdPerson()
		{
			CameraManager.Instance.SetCameraTarget(player.transform);
			InputManager.Instance.SetInputPreset(InputPreset.ThirdPerson);
			CursorLocking.Lock();
			EffectManager.Instance.StopAll();
		}

		private void SetupFPS()
		{
			CameraManager.Instance.SetCameraTarget(player.transform);
			InputManager.Instance.SetInputPreset(InputPreset.FPS);
			CursorLocking.Lock();
			EffectManager.Instance.StopAll();
		}

		private void SetupRTS()
		{
			InputManager.Instance.SetInputPreset(InputPreset.RTS);
			CameraManager.Instance.SetCameraTarget(player.transform);
			CursorLocking.Unlock();
			EffectManager.Instance.StopAll();
		}

		private void SetupRPG()
		{
			InputManager.Instance.SetInputPreset(InputPreset.RPG);
			CameraManager.Instance.SetCameraTarget(player.transform);
			CursorLocking.Unlock();
			EffectManager.Instance.StopAll();
		}

		private void SetupOrbit()
		{
			InputManager.Instance.SetInputPreset(InputPreset.Orbit);
			CursorLocking.Unlock();
			EffectManager.Instance.StopAll();
		}

		private void SetupDead()
		{
			EffectManager.Instance.StopAll();
		}

		private void SetupDebug()
		{
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
			if (GUI.Button(new Rect(x, num + 210f, 100f, 30f), "Dead"))
			{
				SetupDead();
			}
			if (GUI.Button(new Rect(x, num + 250f, 100f, 30f), "FPS"))
			{
				SetupFPS();
			}
			if (GUI.Button(new Rect(x, num + 290f, 100f, 30f), "Debug"))
			{
				SetupDebug();
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
		}

		private void OnGUI()
		{
			GUI.skin = skin;
			if (showEffects)
			{
				displayEffects = GUI.Toggle(new Rect(effectsGUIPos.x, effectsGUIPos.y, 150f, 30f), displayEffects, "Camera effects");
				if (displayEffects)
				{
					DisplayEffects();
				}
			}
			showGameModes = GUI.Toggle(new Rect(gameModesGUIPos.x, gameModesGUIPos.y, 150f, 30f), showGameModes, "Camera modes");
			if (showGameModes)
			{
				ShowGameModes();
			}
		}
	}
}
