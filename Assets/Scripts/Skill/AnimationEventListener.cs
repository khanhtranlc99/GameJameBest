using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventListener : MonoBehaviour
{
    public UnityEvent onTriggerEvent;
    
    public void OnEventTrigger()
    {
        onTriggerEvent?.Invoke();    
    }
}
