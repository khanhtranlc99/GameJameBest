using Game.GlobalComponent;
using Game.Vehicle;
using UnityEngine;

namespace MiamiGames.Mech
{
	public class MechBigfootController : MechController
	{
		private const float turnBackInputValue = -0.8f;

		public AudioClip toSideRotation;

		public AudioClip toBackRotation;

		public override void Init(DrivableVehicle drivableVehicle)
		{
			base.Init(drivableVehicle);
			MechControlPanel.Instance.EnableToSideRotationButtons();
		}

		protected override void Footsteps()
		{
			if (inputs.right90 || (inputs.left90 && timerForSpecialSound <= 0f))
			{
				EngineAudioSource.clip = toSideRotation;
				EngineAudioSource.pitch = 1f;
				EngineAudioSource.Play();
				timerForSpecialSound = toSideRotation.length;
			}
			if (inputs.move.y <= -0.8f && timerForSpecialSound <= 0f)
			{
				EngineAudioSource.clip = toBackRotation;
				EngineAudioSource.pitch = 1f;
				EngineAudioSource.Play();
				timerForSpecialSound = toBackRotation.length;
			}
			base.Footsteps();
		}
	}
}
