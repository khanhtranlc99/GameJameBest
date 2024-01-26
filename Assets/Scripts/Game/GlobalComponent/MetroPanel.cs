using Game.Character;
using Game.Character.CharacterController;
using Game.MiniMap;
using Game.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent
{
    public class MetroPanel : MonoBehaviour
    {
        [Serializable]
        public class AnimationSetup
        {
            public float Scale = 2f;

            public Color StartColor;

            public Color EndColor;

            public float timePerFrame = 2f;
        }

        public static MetroPanel Instance;

        public Sprite MenuMapSprite;

        public Sprite MetroMapSprite;

        public float NormalScale;


        public AnimationSetup AnimationSettings;

        private MetroManager manager;

        private Metro animatedMetro;

        private float timer;

        private Player player;

        [SerializeField] private Button btnGetOutMetro;
        [SerializeField] private Button btnTakeRide;

        private MetroManager metroManager
        {
            get
            {
                if ((bool)manager)
                {
                    return manager;
                }
                manager = MetroManager.Instance;
                return manager;
            }
        }

        private void Awake()
        {
            Instance = this;
            btnGetOutMetro.onClick.AddListener(OnClickGetOut);
            btnTakeRide.onClick.AddListener(OnClickTakeRide);
        }

        private void OnClickGetOut()
        {
            MiniMap.MiniMap.Instance.ChangeMapSize(false);
            CloseMetroPanel();
        }

        private void OnClickTakeRide()
        {
            metroManager.ExitMetro();
        }

        private void Start()
        {
            player = PlayerInteractionsManager.Instance.Player;
        }

        public void Open()
        {
            if (!player)
            {
                player = PlayerInteractionsManager.Instance.Player;
            }
            if (!player.IsDead)
            {
                gameObject.SetActive(true);
            }
        }

        private void OnEnable()
        {
            //UIGame.Instance.Pause();
            GameplayUtils.PauseGame();
            foreach (Metro metro in metroManager.Metros)
            {
                if ((bool)metro.MetroMark)
                {
                    metro.MetroMark.SetMetroSprite(MetroMapSprite);
                }
            }
            CheckSelected();
        }

        public void CloseMetroPanel()
        {
            gameObject.SetActive(false);
            if (manager.isButtonMetro)
            {
                manager.isButtonMetro = false;
                UICanvasController.Instance.ShowLayer(UICanvasKey.INGAME);
            }
        }

        private void OnDisable()
        {

            if (manager)
            {
                GameplayUtils.ResumeGame();
                foreach (Metro metro in metroManager.Metros)
                {
                    if ((bool)metro.MetroMark)
                    {
                        metro.MetroMark.SetNormalSprite(MenuMapSprite);
                    }
                }
                Game.MiniMap.MiniMap.Instance.ChangeMapSize(fullScreen: false);
            }
        }

        private void Update()
        {
            AnimateMetro();
        }

        private void AnimateMetro()
        {
            if (!metroManager.TerminusMetro)
            {
                return;
            }
            if (!animatedMetro)
            {
                animatedMetro = metroManager.TerminusMetro;
            }
            if (animatedMetro.Equals(metroManager.TerminusMetro))
            {
                timer += Time.fixedDeltaTime;
                if (AnimationSettings.timePerFrame / 2f > timer)
                {
                    animatedMetro.MetroMark.drawedIconSprite.color = Color.Lerp(animatedMetro.MetroMark.drawedIconSprite.color, AnimationSettings.StartColor, Time.fixedDeltaTime * 8f);
                    animatedMetro.MetroMark.IconScale = Mathf.Lerp(animatedMetro.MetroMark.IconScale, NormalScale * AnimationSettings.Scale, Time.fixedDeltaTime * 2f);
                }
                else
                {
                    animatedMetro.MetroMark.drawedIconSprite.color = Color.Lerp(animatedMetro.MetroMark.drawedIconSprite.color, AnimationSettings.EndColor, Time.fixedDeltaTime * 8f);
                    animatedMetro.MetroMark.IconScale = Mathf.Lerp(animatedMetro.MetroMark.IconScale, NormalScale, Time.fixedDeltaTime * 2f);
                }
                if (AnimationSettings.timePerFrame < timer)
                {
                    timer = 0f;
                }
            }
            else
            {
                animatedMetro.MetroMark.IconScale = NormalScale;
                animatedMetro = null;
            }
        }

        public void CheckSelected()
        {
            if (gameObject.activeSelf)
            {
                foreach (Metro metro in MetroManager.Instance.Metros)
                {
                    if (metro.Equals(metroManager.CurrentMetro))
                    {
                        metro.MetroMark.SetCurrent();
                    }
                    else if (metro.Equals(metroManager.TerminusMetro))
                    {
                        metro.MetroMark.Select();
                    }
                    else
                    {
                        metro.MetroMark.DisableSelected();
                    }
                }
            }
        }
    }
}
