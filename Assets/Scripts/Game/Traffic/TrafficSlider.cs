using Game.GlobalComponent.Quality;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Traffic
{
	public class TrafficSlider : MonoBehaviour
	{
		public static Slider DensitySlider;

		public static void UpdteValue()
		{
			if ((bool)DensitySlider)
			{
				DensitySlider.value = QualityManager.CountPedestrians;
			}
		}

		private void Awake()
		{
			DensitySlider = GetComponent<Slider>();
		}

		private void Start()
		{
			UpdteValue();
		}

		public void ChangeDensity(float density)
		{
			QualityManager.SetCountPedestrians((int)density);
			QualityManager.SetCountVehicles((int)density);
		}
	}
}
