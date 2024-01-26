using System;
using UnityEngine;
using UnityEngine.UI;
using EventDispatcher;

namespace Skill
{
    public class ButtonSkill : MonoBehaviour
    {
        public int id;
        private Button btnSkill;
        public GameObject lockIcon;
        public GameObject adsIcon;
        private SkillManager skillManager;
        [HideInInspector] public bool canUse;
        [HideInInspector] public bool ShowSkill;

        private void Awake()
        {
            EventDispatcher.EventDispatcher.Instance.RegisterListener(EventID.BUY_SKILL_DONE, Init);
            skillManager = FindObjectOfType<SkillManager>();
            Init();
            InitBtn();
        }
        private void Init(object param = null)
        {
            LoadCanUseSkill();
            LoadShowSkill();
            SetupShow();
            SetupUse();
        }
        private void InitBtn()
        {
            btnSkill = GetComponent<Button>();
            btnSkill.onClick.AddListener(() =>
            {
                skillManager.ActiveSkill(id, this);
            });
        }
        public void SetupShow()
        {
            this.gameObject.SetActive(ShowSkill);
        }
        public void SetupUse()
        {
            lockIcon.SetActive(!canUse);
            adsIcon.SetActive(!canUse);
        }
        public void Unlock()
        {
            canUse = true;
            SaveCanUseSkill();
            var skillManager = FindObjectOfType<SkillManager>();
            skillManager.ShowNextSkill(id);
            EventDispatcher.EventDispatcher.Instance.PostEvent(EventID.BUY_SKILL_DONE);
        }
        public void SaveShowSkill()
        {
            BaseProfile.StoreValue(ShowSkill, "ShowSkill" + id);
        }

        private void LoadShowSkill()
        {
            if (id == 0)
            {
                ShowSkill = BaseProfile.ResolveValue("ShowSkill" + id, defaultValue: true);
            }
            else
            {
                ShowSkill = BaseProfile.ResolveValue("ShowSkill" + id, defaultValue: false);
            }
        }
        private void SaveCanUseSkill()
        {
            BaseProfile.StoreValue(canUse, "CanUseSkill" + id);
        }

        private void LoadCanUseSkill()
        {
            canUse = BaseProfile.ResolveValue("CanUseSkill" + id, defaultValue: false);
        }
        private void OnDestroy()
        {
            EventDispatcher.EventDispatcher.Instance.RemoveListener(EventID.BUY_SKILL_DONE, Init);
        }
    }
}
