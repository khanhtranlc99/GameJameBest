using Game.Character.Utils;
using Game.GlobalComponent;
using System;
using UnityEngine;

namespace Game.Character.Input
{
	[Serializable]
	public class ThirdPersonInput : GameInput
	{
		private const string ResetName = "Reset";

		private const string HorizontalRName = "Horizontal_R";

		private const string VerticalRName = "Vertical_R";

		private const string MouseXName = "Mouse X";

		private const string MouseYName = "Mouse Y";

		private const string HorizontalName = "Horizontal";

		private const string VerticalName = "Vertical";

		private const string AimName = "Aim";

		private const string FireName = "Fire";

		private const string CrouchName = "Crouch";

		private const string WalkName = "Walk";

		private const string JumpName = "Jump";

		private const string SprintName = "Sprint";

		private const string JabName = "Jab";

		private const string CrossName = "Cross";

		private const string ShootRopeName = "ShootRope";

		private const string FlyName = "Fly";

		private const string ActionName = "Action";

		public override InputPreset PresetType => InputPreset.ThirdPerson;

		public override void UpdateInput(Input[] inputs)
		{
			Vector2 vector = new Vector2(Controls.GetAxis("Horizontal_R"), Controls.GetAxis("Vertical_R"));
			SetInput(inputs, InputType.Rotate, vector);
			if (vector.sqrMagnitude < Mathf.Epsilon && CursorLocking.IsLocked)
			{
				SetInput(inputs, InputType.Rotate, new Vector2(Controls.GetAxis("Mouse X"), Controls.GetAxis("Mouse Y")));
			}
			SetInput(inputs, InputType.Reset, Controls.GetButton("Reset"));
			float axis = Controls.GetAxis("Horizontal");
			float axis2 = Controls.GetAxis("Vertical");
			Vector2 sample = new Vector2(axis, axis2);
			padFilter.AddSample(sample);
			SetInput(inputs, InputType.Move, padFilter.GetValue());
			SetInput(inputs, InputType.Aim, Controls.GetButton("Aim"));
			SetInput(inputs, InputType.Fire, Controls.GetButton("Fire"));
			SetInput(inputs, InputType.Crouch, Controls.GetButton("Crouch"));
			SetInput(inputs, InputType.Walk, Controls.GetButton("Walk"));
			SetInput(inputs, InputType.Jump, Controls.GetButton("Jump"));
			SetInput(inputs, InputType.Sprint, Controls.GetButton("Sprint"));
			SetInput(inputs, InputType.EyeLaser, Controls.GetButton("EyeLaser"));
			SetInput(inputs, InputType.MeleeArm, Controls.GetButton("Jab"));
			SetInput(inputs, InputType.MeleeFoot, Controls.GetButton("Cross"));
			SetInput(inputs, InputType.ShootRope, Controls.GetButton("ShootRope"));
			SetInput(inputs, InputType.Fly, Controls.GetButton("Fly"));
			SetInput(inputs, InputType.Action, Controls.GetButton("Action"));
		}
	}
}
