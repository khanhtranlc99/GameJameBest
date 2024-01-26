using System;
using RopeHeroViceCity.UI_Features.UI_Setting.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent.Quality
{
	public class QualitySlider : MonoBehaviour
	{

		private Slider slider;

		private void Awake()
		{
			slider = GetComponentInChildren<Slider>();
			if ((bool)slider)
			{
				slider.maxValue = 650f;
				slider.minValue = 100f;
			}
			QualityManager.updateQuality = (QualityManager.UpdateQuality)Delegate.Combine(QualityManager.updateQuality, new QualityManager.UpdateQuality(UpdateSlider));
		}

		private void OnDestroy()
		{
			QualityManager.updateQuality = (QualityManager.UpdateQuality)Delegate.Remove(QualityManager.updateQuality, new QualityManager.UpdateQuality(UpdateSlider));
		}

		private void OnEnable()
		{
			UpdateSlider();
		}

		public void ChangeValue(float value)
		{
			if (UI_Setting.isApplyNow)
			{
				QualityManager.Instance.ChangeCameraCliping(value);
			}
			else
			{
				QualityManager.FarClipPlane = (int)value;
			}
		}

		public void UpdateSlider()
		{
			if ((bool)slider)
			{
				slider.value = QualityManager.FarClipPlane;
			}
		}
	}
}
