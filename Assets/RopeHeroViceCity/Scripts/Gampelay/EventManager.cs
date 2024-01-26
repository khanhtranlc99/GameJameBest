using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace RopeHeroViceCity.Scripts.Gampelay
{
    public class EventManager : SingletonMonoBehavior<EventManager>
    {
        public  UnityEvent OnExitInMenu= new UnityEvent();
        public  UnityEvent OnPausePanelOpen = new UnityEvent();
        public  UnityEvent OnPausePanelClose = new UnityEvent();
        public  UnityEvent ShopOpeningEvent= new UnityEvent();
        public  UnityEvent ShopClosingEvent= new UnityEvent();
        
        public UnityEvent<string,float> onChangePlayerStat = new UnityEvent<string,float>();
        public UnityEvent<float> onFactionUpdate = new UnityEvent<float>();
        public UnityEvent<string> onUpdateAmmo = new UnityEvent<string>();
        public void OnUserExitToMainMenu()
        {
            OnExitInMenu?.Invoke();
        }      
        public void OnOpenPausePanel()
        {
            OnPausePanelOpen?.Invoke();
        } 
        public void OnClosePausePanel()
        {
            OnPausePanelClose?.Invoke();
        } 
        public void OnShopOpenning()
        {
            ShopOpeningEvent?.Invoke();
        }       
        public void OnShopClosing()
        {
            ShopClosingEvent?.Invoke();
        }
        public void OnChangePlayerStat(string nameStat,float value)
        {
            onChangePlayerStat?.Invoke(nameStat,value);
        }

        public void UpdatePlayerFactionValue(float value)
        {
            onFactionUpdate?.Invoke(value);
        }   
        public void UpdateAmmoPlayer(string value)
        {
            onUpdateAmmo?.Invoke(value);
        }
    }
}
