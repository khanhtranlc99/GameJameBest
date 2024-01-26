using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RopeHeroViceCity.UI_Features.UI_Setting.Scripts
{
    [Serializable]
    public class MyToggleEvent : UnityEvent {}
    
    public class CustomToggle : MonoBehaviour
    {
        public MyToggleEvent onEnableToggle, onDisableToggle;
        private Toggle toggle;

         void Awake()
         {
             toggle = GetComponent<Toggle>();
             toggle.onValueChanged.AddListener(OnValueChange);  
        }

        private void OnValueChange(bool isEnable)
        {
            if (isEnable)
            {
                onEnableToggle?.Invoke();
            }
            else
            {
                onDisableToggle?.Invoke();
            }
        }
    }
}
