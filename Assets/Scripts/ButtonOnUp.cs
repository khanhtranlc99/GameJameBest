using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonOnUp : MonoBehaviour, IPointerUpHandler, IEventSystemHandler
{
	public Button.ButtonClickedEvent OnUpEvents;

	public void OnPointerUp(PointerEventData eventData)
	{
		OnUpEvents.Invoke();
	}
}
