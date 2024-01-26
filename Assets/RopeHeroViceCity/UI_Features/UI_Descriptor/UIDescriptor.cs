using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RopeHeroViceCity.UI_Features.UI_Descriptor
{
	public class UIDescriptor : AbsUICanvas
	{

		public Text DescriptionText;

		private RectTransform myRectTransform;

		public void ShowInfo(RectTransform showedRectTransform, string description, float showedTime)
		{
			var transform1 = transform;
			transform1.parent = showedRectTransform;
			var localPosition = transform1.localPosition;
			localPosition = Vector3.zero;
			Vector3 position = base.transform.position;
			Vector3 a = (!(position.y > (float)Screen.height / 2f)) ? Vector3.up : Vector3.down;
			Vector3 position2 = base.transform.position;
			Vector3 a2 = (!(position2.x > (float)Screen.width / 2f)) ? Vector3.right : Vector3.left;
			localPosition += a * (myRectTransform.rect.height / 2f + showedRectTransform.rect.height / 2f);
			localPosition += a2 * (myRectTransform.rect.width / 2f + showedRectTransform.rect.width / 2f);
			transform1.localPosition = localPosition;
			transform1.localScale = Vector3.one;
			DescriptionText.text = description;
			StopAllCoroutines();
			StartCoroutine(Timer(showedTime, Close));
		}
		public override void Close()
		{
			base.Close();
			StopAllCoroutines();
		}

		public override void StartLayer()
		{
			base.StartLayer();
			myRectTransform = (transform as RectTransform);
		}

		private IEnumerator Timer(float time, Action afterAction)
		{
			yield return new WaitForSecondsRealtime(time);
			afterAction?.Invoke();
		}

		public static void OpenPopup(RectTransform showedRectTransform, string description, float showedTime)
		{
			var popup = UICanvasController.Instance.ShowLayer(UICanvasKey.DESCRIPTOR) as UIDescriptor;
			if (popup is { }) popup.ShowInfo(showedRectTransform, description, showedTime);
		}
	}
}
