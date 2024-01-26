using System;
using RopeHeroViceCity.UI_Features.UI_Setting.Scripts;
using UnityEngine;

namespace Game.GlobalComponent.Quality
{
	public class QualityButton : MonoBehaviour
	{

		public QualityLvls QualityLvl;

		public GameObject OnStateObject;

		public GameObject OffStateObject;

		private void Awake()
		{
			QualityManager.updateQuality = (QualityManager.UpdateQuality)Delegate.Combine(QualityManager.updateQuality, new QualityManager.UpdateQuality(UpdateButtonState));
		}

		private void OnDestroy()
		{
			QualityManager.updateQuality = (QualityManager.UpdateQuality)Delegate.Remove(QualityManager.updateQuality, new QualityManager.UpdateQuality(UpdateButtonState));
		}

		private void OnEnable()
		{
			UpdateButtonState();
		}

		public void UpdateButtonState()
		{
			bool flag = QualityManager.QualityLvl == QualityLvl;
			OnStateObject.SetActive(flag);
			OffStateObject.SetActive(!flag);
		}

		public void SetQualityLvl()
		{
			QualityManager.ChangeQuality(QualityLvl, UI_Setting.isApplyNow);
		}
	}
}
