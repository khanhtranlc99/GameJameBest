using UnityEngine;

namespace UnitySampleAssets.CrossPlatformInput
{
	public class MobileControlRig : MonoBehaviour
	{
		private void OnEnable()
		{
			CheckEnableControlRig();
		}

		private void CheckEnableControlRig()
		{
			EnableControlRig(enabled: false);
		}

		private void EnableControlRig(bool enabled)
		{
			foreach (Transform item in base.transform)
			{
				item.gameObject.SetActive(enabled);
			}
		}
	}
}
