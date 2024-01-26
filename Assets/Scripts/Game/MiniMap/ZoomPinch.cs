using UnityEngine;
using UnityEngine.EventSystems;
using UnitySampleAssets.CrossPlatformInput;

namespace Game.MiniMap
{
	public class ZoomPinch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		public string AxisName = "MapZoom";

		private bool touching;

		private CrossPlatformInputManager.VirtualAxis zoomAxis;

		private float deltaMagnitudeDiff;

		private void OnEnable()
		{
			zoomAxis = new CrossPlatformInputManager.VirtualAxis(AxisName);
		}

		private void OnDisable()
		{
			zoomAxis.Remove();
		}

		private void Update()
		{
			if (touching && UnityEngine.Input.touchCount == 2)
			{
				Touch touch = UnityEngine.Input.GetTouch(0);
				Touch touch2 = UnityEngine.Input.GetTouch(1);
				Vector2 a = touch.position - touch.deltaPosition;
				Vector2 b = touch2.position - touch2.deltaPosition;
				float magnitude = (a - b).magnitude;
				float magnitude2 = (touch.position - touch2.position).magnitude;
				deltaMagnitudeDiff = magnitude - magnitude2;
			}
			else
			{
				deltaMagnitudeDiff = 0f;
			}
			zoomAxis.Update(deltaMagnitudeDiff);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			touching = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			touching = false;
		}
	}
}
