using System;
using UnityEngine;
using System.Collections;
namespace RopeHeroViceCity.UI_Features.UI_PleaseWaitPanel.Scripts
{
    public class UI_PleaseWait : AbsUICanvas
    {
        private float timer;
        public void StartWaiting(Action callback)
        {
            timer = Time.frameCount + 10;
            StartCoroutine(Show(callback));
        }
        private void LateUpdate()
        {
            SwitchWaitingPanel();
        }
        
        private IEnumerator Show(Action callback)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            callback();
        }
        private void SwitchWaitingPanel()
        {
            if (timer <= (float)Time.frameCount && timer != 0f)
            {
                Close();
                timer = 0f;
            }
        }

        public static void OpenWaiting(Action callback)
        {
            if (UICanvasController.Instance.ShowLayer(UICanvasKey.WAITING_POPUP) is UI_PleaseWait { } popup) popup.StartWaiting(callback);
        }
    }
}
