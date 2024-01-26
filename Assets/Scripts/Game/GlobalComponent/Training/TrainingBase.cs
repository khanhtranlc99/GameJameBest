using UnityEngine;

namespace Game.GlobalComponent.Training
{
	public class TrainingBase : MonoBehaviour
	{
		public string TrainingName;

		public RectTransform ObjectForActive;

		public string ObjectDescription;

		public float AdditionalPointerScalling;

		private bool timeWasFreezed;

		public virtual string GetContinueMessage()
		{
			return "Tap anywhere to continue.";
		}

		public virtual void StartTraining()
		{
			if (!GameplayUtils.OnPause)
			{
				GameplayUtils.PauseGame();
				timeWasFreezed = true;
			}
		}

		public virtual void EndTraining()
		{
			if (timeWasFreezed)
			{
				GameplayUtils.ResumeGame();
				timeWasFreezed = false;
			}
			TrainingManager.Instance.TrainingEnd();
		}

		public virtual void ClickOnPanel()
		{
			EndTraining();
		}
	}
}
