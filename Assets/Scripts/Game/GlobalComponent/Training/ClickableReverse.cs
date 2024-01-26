using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.GlobalComponent.Training
{
	public class ClickableReverse : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler, IEndDragHandler, IEventSystemHandler, IPointerExitHandler
	{
		public GraphicRaycaster RootRaycaster;

		private readonly List<Component> downPressedComponents = new List<Component>();

		private PointerEventData downEventData;

		private readonly List<Component> dragedComponents = new List<Component>();

		private PointerEventData dragEventData;

		public void OnPointerDown(PointerEventData eventData)
		{
			List<RaycastResult> list = new List<RaycastResult>();
			RootRaycaster.Raycast(eventData, list);
			foreach (RaycastResult item in list)
			{
				if (!(item.gameObject == base.gameObject))
				{
					Component[] components = item.gameObject.GetComponents<Component>();
					Component[] array = components;
					foreach (Component component in array)
					{
						IPointerDownHandler pointerDownHandler = component as IPointerDownHandler;
						if (pointerDownHandler != null)
						{
							pointerDownHandler.OnPointerDown(eventData);
							downPressedComponents.Add(component);
							downEventData = eventData;
						}
					}
				}
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			List<RaycastResult> list = new List<RaycastResult>();
			RootRaycaster.Raycast(eventData, list);
			foreach (RaycastResult item in list)
			{
				if (!(item.gameObject == base.gameObject))
				{
					Component[] components = item.gameObject.GetComponents<Component>();
					Component[] array = components;
					foreach (Component component in array)
					{
						(component as IPointerUpHandler)?.OnPointerUp(eventData);
					}
				}
			}
			downPressedComponents.Clear();
			downEventData = null;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			List<RaycastResult> list = new List<RaycastResult>();
			RootRaycaster.Raycast(eventData, list);
			foreach (RaycastResult item in list)
			{
				if (!(item.gameObject == base.gameObject))
				{
					Component[] components = item.gameObject.GetComponents<Component>();
					Component[] array = components;
					foreach (Component component in array)
					{
						(component as IPointerClickHandler)?.OnPointerClick(eventData);
					}
				}
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			foreach (Component downPressedComponent in downPressedComponents)
			{
				(downPressedComponent as IPointerUpHandler)?.OnPointerUp(downEventData);
			}
			downPressedComponents.Clear();
			downEventData = null;
		}

		public void OnDrag(PointerEventData eventData)
		{
			List<RaycastResult> list = new List<RaycastResult>();
			RootRaycaster.Raycast(eventData, list);
			foreach (RaycastResult item in list)
			{
				if (!(item.gameObject == base.gameObject))
				{
					Component[] components = item.gameObject.GetComponents<Component>();
					Component[] array = components;
					foreach (Component component in array)
					{
						IDragHandler dragHandler = component as IDragHandler;
						if (dragHandler != null)
						{
							dragHandler.OnDrag(eventData);
							dragedComponents.Add(component);
							dragEventData = eventData;
						}
					}
				}
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			foreach (Component dragedComponent in dragedComponents)
			{
				(dragedComponent as IEndDragHandler)?.OnEndDrag(dragEventData);
			}
			dragedComponents.Clear();
			dragEventData = null;
		}

		private void OnDisable()
		{
			if (downEventData != null)
			{
				OnPointerExit(downEventData);
			}
			if (dragEventData != null)
			{
				OnEndDrag(dragEventData);
			}
		}
	}
}
