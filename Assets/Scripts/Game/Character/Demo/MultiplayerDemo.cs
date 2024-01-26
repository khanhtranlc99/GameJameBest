using Game.Character.CameraEffects;
using Game.Character.CharacterController;
using Game.Character.Input;
using Game.Character.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Demo
{
	public class MultiplayerDemo : MonoBehaviour
	{
		public Transform playersParent;

		public Vector2 effectsGUIPos;

		public Vector2 gameModesGUIPos;

		public GUISkin skin;

		private List<Player> players;

		private Player currentPlayer;

		private bool switchPlayers;

		private bool showGameModes;

		private void Awake()
		{
			Application.targetFrameRate = 60;
			UnityEngine.Random.InitState(DateTime.Now.TimeOfDay.Milliseconds);
			players = new List<Player>(4);
			for (int i = 0; i < playersParent.childCount; i++)
			{
				Transform child = playersParent.GetChild(i);
				if ((bool)child)
				{
					Player component = child.GetComponent<Player>();
					if ((bool)component)
					{
						component.Remote = true;
						players.Add(component);
					}
				}
			}
			if (players.Count > 0)
			{
				currentPlayer = players[0];
				currentPlayer.Remote = false;
			}
		}

		private void Start()
		{
			if ((bool)currentPlayer)
			{
				CameraManager.Instance.SetCameraTarget(currentPlayer.transform);
			}
		}

		private void SetupThirdPerson()
		{
			CameraManager.Instance.SetCameraTarget(currentPlayer.transform);
			InputManager.Instance.SetInputPreset(InputPreset.ThirdPerson);
			CursorLocking.Lock();
			EffectManager.Instance.StopAll();
		}

		private void SetupFPS()
		{
			CameraManager.Instance.SetCameraTarget(currentPlayer.transform);
			InputManager.Instance.SetInputPreset(InputPreset.FPS);
			CursorLocking.Lock();
			EffectManager.Instance.StopAll();
		}

		private void SetupRTS()
		{
			InputManager.Instance.SetInputPreset(InputPreset.RTS);
			CameraManager.Instance.SetCameraTarget(currentPlayer.transform);
			CursorLocking.Unlock();
			EffectManager.Instance.StopAll();
		}

		private void SetupRPG()
		{
			InputManager.Instance.SetInputPreset(InputPreset.RPG);
			CameraManager.Instance.SetCameraTarget(currentPlayer.transform);
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

		private void SetupLookAt()
		{
		}

		private void ShowGameModes()
		{
			float num = gameModesGUIPos.y + 30f;
			float x = gameModesGUIPos.x;
			if (GUI.Button(new Rect(x, num + 10f, 120f, 30f), "ThirdPerson"))
			{
				SetupThirdPerson();
			}
			if (GUI.Button(new Rect(x, num + 50f, 120f, 30f), "RTS"))
			{
				SetupRTS();
			}
			if (GUI.Button(new Rect(x, num + 90f, 120f, 30f), "RPG"))
			{
				SetupRPG();
			}
			if (GUI.Button(new Rect(x, num + 130f, 120f, 30f), "Orbit"))
			{
				SetupOrbit();
			}
			if (GUI.Button(new Rect(x, num + 170f, 120f, 30f), "LookAt"))
			{
				SetupLookAt();
			}
			if (GUI.Button(new Rect(x, num + 210f, 120f, 30f), "Dead"))
			{
				SetupDead();
			}
			if (GUI.Button(new Rect(x, num + 250f, 120f, 30f), "FPS"))
			{
				SetupFPS();
			}
		}

		private void SwitchPlayers()
		{
			float num = effectsGUIPos.y + 30f;
			float x = effectsGUIPos.x;
			if (GUI.Button(new Rect(x, num + 10f, 120f, 30f), "Select None"))
			{
				currentPlayer.Remote = true;
			}
			int num2 = 50;
			int num3 = 0;
			foreach (Player player in players)
			{
				num3++;
				if (GUI.Button(new Rect(x, num + (float)num2, 120f, 30f), "Select Player " + num3.ToString()))
				{
					currentPlayer.Remote = true;
					currentPlayer = player;
					currentPlayer.Remote = false;
					CameraManager.Instance.SetCameraTarget(currentPlayer.transform);
				}
				num2 += 40;
			}
		}

		private void OnGUI()
		{
			GUI.skin = skin;
			switchPlayers = GUI.Toggle(new Rect(effectsGUIPos.x, effectsGUIPos.y, 150f, 30f), switchPlayers, "Switch players");
			if (switchPlayers)
			{
				SwitchPlayers();
			}
			showGameModes = GUI.Toggle(new Rect(gameModesGUIPos.x, gameModesGUIPos.y, 150f, 30f), showGameModes, "Camera modes");
			if (showGameModes)
			{
				ShowGameModes();
			}
		}
	}
}
