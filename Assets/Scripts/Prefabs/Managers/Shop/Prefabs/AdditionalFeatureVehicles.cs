using UnityEngine;
using UnityEngine.UI;

namespace Prefabs.Managers.Shop.Prefabs
{
	public class AdditionalFeatureVehicles : MonoBehaviour
	{
		[SerializeField]
		private Image image;

		[SerializeField]
		private Text description;

		public void SetImage(Sprite sprite)
		{
			image.sprite = sprite;
		}

		public void SetDescription(string description)
		{
			this.description.text = string.Empty + description;
		}
	}
}
