using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonOnDown : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	public Button.ButtonClickedEvent OnDownEvents;

	public void OnPointerDown(PointerEventData eventData)
	{
		OnDownEvents.Invoke();
	}
}
