using Game.GlobalComponent.Qwest;
using Game.UI;
using System;
using System.Collections;
using RopeHeroViceCity.UI_Features.UI_UniversalPopUp.Scripts;
using UnityEngine;

namespace Code.Game.Race
{
    public class QuestToRaceAdapter : MonoBehaviour
    {
        private static QuestToRaceAdapter instance;

        [SerializeField]
        private string displayText = "Are you sure you want to start the race?";

        [SerializeField]
        private int delayTime = 10;

        public static QuestToRaceAdapter Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new Exception("QuestToRaceAdapter is not initialized");
                }
                return instance;
            }
        }

        public void InitQuest(int raceNumber, Qwest qwest)
        {
            StartCoroutine(StartUniversalYesNoPanel(raceNumber, qwest));
        }

        private void Awake()
        {
            instance = this;
        }

        private IEnumerator StartUniversalYesNoPanel(int raceNumber, Qwest qwest)
        {
            yield return new WaitForSeconds(0.1f);
            UI_GeneralPopup.ShowPopup(null, displayText, false, delegate
             {
                 RaceManager.Instance.InitRace(raceNumber);
             }, delegate
             {
                //base._003C_003Ef__this.StartCoroutine(base._003C_003Ef__this.Timer(base._003C_003Ef__this.delayTime, qwest));
                StartCoroutine(Timer(delayTime, qwest));
                //Debug.LogError("k tim thay _003C_003Ef__this");
            });
        }

        private IEnumerator Timer(int sec, Qwest qwest)
        {
            GameEventManager.Instance.QwestCancel(qwest);
            yield return new WaitForSeconds(sec);
            GameEventManager.Instance.ResetQwestCancel(qwest);
        }
    }
}
