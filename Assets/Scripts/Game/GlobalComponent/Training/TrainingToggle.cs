using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent.Training
{
	public class TrainingToggle : MonoBehaviour
	{
		private Toggle toggle;

		public void ToggleAction(bool isChecked)
		{
			BaseProfile.SkipTraining = !isChecked;
			if (isChecked)
			{
				TrainingManager.ClearCompletedTrainingsInfo();
			}
			if (!(TrainingManager.Instance == null))
			{
				TrainingManager.Instance.SetTrainingStatus(isChecked);
				TrainingManager.Instance.ClearLocalCompletedTrainingsInfo();
			}
		}

		private void OnEnable()
		{
			if (!toggle)
			{
				toggle = GetComponent<Toggle>();
			}
			toggle.isOn = !BaseProfile.SkipTraining;
		}
	}
}
