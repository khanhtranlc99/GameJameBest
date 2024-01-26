using Game.Character.CharacterController;
using Game.Character.Modes;
using UnityEngine;

namespace Game.Character.Demo
{
	public class HelpScreen : MonoBehaviour
	{
		public Vector2 guiPos;

		public Vector2 showLiveGUIPos;

		public GUISkin skin;

		private bool show = true;

		private bool showLiveGUI;

		private void Show(bool dieInfo)
		{
			float y = guiPos.y + 30f;
			float x = guiPos.x;
			Type type = CameraManager.Instance.GetCurrentCameraMode().Type;
			GUIStyle gUIStyle = new GUIStyle("box");
			gUIStyle.fontSize = 12;
			gUIStyle.alignment = TextAnchor.MiddleLeft;
			string empty = string.Empty;
			if (dieInfo)
			{
				empty += "Press 'R' to resurect player.";
			}
			else
			{
				empty += "Press Tab to lock mouse cursor\nPress Escape to unlock it\n";
				empty += "Press 'H' to show/hide this window\n\n";
				switch (type)
				{
				case Type.ThirdPerson:
					empty += "WASD - move around the scene\n";
					empty += "Right mouse button - Aim\n";
					empty += "Left mouse button - Shoot\n";
					empty += "Space - jump\n";
					empty += "C - crouch\n";
					empty += "LShift - Sprint\n";
					empty += "CapsLock - Walk\n";
					empty += "----------------------------\n";
					empty += "You can use gamepad as well!\n";
					break;
				case Type.RPG:
					empty += "Use Right mouse button to set waypoint position.\n";
					empty += "Use Right mouse button to attack enemies.\n";
					empty += "Use Mouse scrollwheel to zoom the camera.\n";
					empty += "Use '[' and ']' to rotate the camera.\n";
					break;
				case Type.RTS:
					empty += "Use Right mouse button to set waypoint position.\n";
					empty += "Use Right mouse button to attack enemies.\n";
					empty += "To move the camera move your mouse to screen.\nborder, use WSAD or drag the scene.\n";
					empty += "Use Mouse scrollwheel to zoom the camera.\n";
					empty += "Use '[' and ']' to rotate the camera.\n";
					break;
				case Type.Orbit:
					empty += "Use Right mouse button to rotate the camera.\n";
					empty += "Use Left right mouse button to pan the camera.\n";
					empty += "Use Mouse scrollwheel to zoom the camera.\n";
					empty += "Use Middle mouse double-click button to reset camera target\n";
					break;
				case Type.LookAt:
					empty += "Randomly choose camera position and target.\n";
					empty += "You can click on LookAt button again to repeat\nthe process.\n";
					break;
				case Type.Dead:
					empty += "Camera without controls, just rotating around\ncharacter.\n";
					break;
				}
			}
			GUI.Box(new Rect(x, y, 300f, (!dieInfo) ? 200 : 50), empty, gUIStyle);
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.H))
			{
				show = !show;
			}
		}

		private void OnGUI()
		{
			GUI.skin = skin;
			show = GUI.Toggle(new Rect(guiPos.x, guiPos.y, 100f, 30f), show, "Help");
			showLiveGUI = GUI.Toggle(new Rect(showLiveGUIPos.x, showLiveGUIPos.y, 150f, 30f), showLiveGUI, "Live configuration");
			if (show)
			{
				Show(dieInfo: false);
			}
			CameraManager.Instance.GetCurrentCameraMode().EnableLiveGUI = showLiveGUI;
			Transform cameraTarget = CameraManager.Instance.CameraTarget;
			if ((bool)cameraTarget && (bool)cameraTarget.GetComponent<Player>() && cameraTarget.GetComponent<Player>().IsDead)
			{
				Show(dieInfo: true);
			}
		}
	}
}
