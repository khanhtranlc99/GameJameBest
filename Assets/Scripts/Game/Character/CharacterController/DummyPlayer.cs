using Game.Character.Input;
using Game.Character.Modes;
using UnityEngine;

namespace Game.Character.CharacterController
{
	public class DummyPlayer : MonoBehaviour
	{
		private Transform cam;

		private InputManager inputManager;

		private Vector3 targetPos;

		private void Start()
		{
			cam = CameraManager.Instance.UnityCamera.transform;
			inputManager = InputManager.Instance;
			targetPos = base.transform.position;
		}

		private void UpdateThirdPersonController()
		{
			inputManager.SetInputPreset(InputPreset.ThirdPerson);
			Vector2 input = inputManager.GetInput(InputType.Move, Vector2.zero);
			bool input2 = inputManager.GetInput(InputType.Sprint, defaultValue: false);
			Vector3 normalized = Vector3.Scale(cam.forward, new Vector3(1f, 0f, 1f)).normalized;
			Vector3 a = input.y * normalized + input.x * cam.right;
			if (a.magnitude > 1f)
			{
				a.Normalize();
			}
			float d = (!inputManager.GetInput(InputType.Walk).Valid || !(bool)inputManager.GetInput(InputType.Walk).Value) ? 1f : 0.5f;
			a *= d;
			input2 &= (a.sqrMagnitude > 0.5f);
			if (input2)
			{
				a *= 1.5f;
			}
			bool input3 = inputManager.GetInput(InputType.Aim, defaultValue: false);
			CameraMode currentCameraMode = CameraManager.Instance.GetCurrentCameraMode();
			if (input2)
			{
				currentCameraMode.SetCameraConfigMode("Sprint");
			}
			else if (input3)
			{
				currentCameraMode.SetCameraConfigMode("Aim");
			}
			else
			{
				currentCameraMode.SetCameraConfigMode("Default");
			}
			base.transform.position += a * 0.1f;
			if (a.sqrMagnitude > 0f)
			{
				base.transform.forward = a.normalized;
			}
			if (input3)
			{
				base.transform.forward = normalized;
			}
		}

		private void UpdateRPG()
		{
			inputManager.SetInputPreset(InputPreset.RPG);
			Game.Character.Input.Input input = inputManager.GetInput(InputType.WaypointPos);
			if (input.Valid)
			{
				targetPos = (Vector3)input.Value;
			}
			if ((base.transform.position - targetPos).sqrMagnitude > 1f)
			{
				Vector3 vector = -(base.transform.position - targetPos) * 1f;
				base.transform.forward = vector;
				base.transform.position += vector * Time.deltaTime;
			}
			GetComponent<Rigidbody>().position = base.transform.position;
		}

		private void UpdateRTS()
		{
			inputManager.SetInputPreset(InputPreset.RTS);
			UpdateRPG();
		}

		private void FixedUpdate()
		{
			if (inputManager.IsValid)
			{
				CameraManager instance = CameraManager.Instance;
				CameraMode currentCameraMode = instance.GetCurrentCameraMode();
				switch (currentCameraMode.Type)
				{
				case Type.ThirdPersonVehicle:
					break;
				case Type.ThirdPerson:
					UpdateThirdPersonController();
					break;
				case Type.RPG:
					UpdateRPG();
					break;
				case Type.RTS:
					UpdateRTS();
					break;
				}
			}
		}
	}
}
