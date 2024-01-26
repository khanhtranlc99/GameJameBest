using Game.Character;
using Game.Character.CharacterController;
using Game.MiniMap;
using Game.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalComponent
{
    public class MetroManager : MonoBehaviour
    {
        private const int MaxCopStarsToLostAttention = 2;

        private const float DistanceToDisableButton = 7f;

        private const float SlowUpdateProcTime = 2f;

        private static MetroManager instance;

        public bool InMetro;

        public bool isButtonMetro;

        public Metro CurrentMetro;

        public MetroPanel MetroPanel;

        public List<Metro> Metros = new List<Metro>();

        public Metro TerminusMetro;

        private Transform player;

        private SlowUpdateProc slowUpdate;

        private GameObject getInButton;

        public static MetroManager Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new Exception("Metro Manager not find");
                }
                return instance;
            }
        }

        public static bool InstanceExists => instance != null;

        public void SetTerminus(Metro metro)
        {
            TerminusMetro = metro;
        }

        public void GetInMetro()
        {
            if (!InMetro && CurrentMetro != null)
            {
                CurrentMetro.EnterInMetro(delegate
                {
                    ChangePoliceRelationOnEnter();
                    Game.MiniMap.MiniMap.Instance.ChangeMapSize(fullScreen: true);
                    //AdsManager.ShowFullscreenAd();
                    MetroPanel.Open();
                });
                InMetro = true;
            }
        }

        public void TakeTheSubway()
        {
            if (CurrentMetro && TerminusMetro)
            {
                GameplayUtils.ResumeGame();
                TerminusMetro.ExitFromMetro();
            }
        }

        public void ExitMetro()
        {
            CurrentMetro.ExitFromMetro();
        }

        public void RegisterMetro(Metro m)
        {
            if (!Metros.Contains(m))
            {
                Metros.Add(m);
            }
        }
        public void Init()
        {
            getInButton = PlayerInteractionsManager.Instance.GetInMetroButton.gameObject;
            player = PlayerInteractionsManager.Instance.Player.transform;
            slowUpdate = new SlowUpdateProc(SlowUpdate, 2f);
        }

        private void SlowUpdate()
        {
            CheckDistance();
        }

        private void CheckDistance()
        {
            if ((bool)player && (bool)CurrentMetro && Vector3.Distance(player.transform.position, CurrentMetro.transform.position) > 7f)
            {
                getInButton.SetActive(value: false);
            }
        }

        private void FixedUpdate()
        {
            if (slowUpdate != null)
                slowUpdate.ProceedOnFixedUpdate();
        }

        private void ChangePoliceRelationOnEnter()
        {
            float playerRelationsValue = FactionsManager.Instance.GetPlayerRelationsValue(Faction.Police);
            int num = (int)Math.Truncate(Mathf.Abs(playerRelationsValue) / 2f);
            if (num <= 2 && num >= 1)
            {
                FactionsManager.Instance.ChangePlayerRelations(Faction.Police, 2f);
                FactionsManager.Instance.ChangePlayerRelations(Faction.Police, 2f);
            }
        }

        private void Awake()
        {
            instance = this;
        }
    }
}
