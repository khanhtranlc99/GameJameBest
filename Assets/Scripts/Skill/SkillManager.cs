using System;
using System.Collections;
using System.Collections.Generic;
using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Modes;
using Game.GlobalComponent;
using Root.Scripts.Helper;
using RopeHeroViceCity.UI_Features.UI_UniversalPopUp.Scripts;
using Skill;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public BaseSkills[] allSkills;
    private Player player;
    private int currentSkillIndex;
    private ButtonSkill currentSkill;
    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    public void ActiveSkill(int index, ButtonSkill buttonSkill)
    {
        if (!player.CanUseSkill) return;
        currentSkillIndex = index;
        currentSkill = buttonSkill;
        if (!buttonSkill.canUse)
        {
            //if (AdManager.Instance.IsRewardVideoLoaded())
            //{
            //    AdManager.Instance.ShowRewardVideo(UnlockSkill);
            //    return;
            //}
            //else
            //{
                UI_GeneralPopup.ShowPopup(StringHelper.ADS_NOT_AVAILABLE_HEADER, StringHelper.ADS_NOT_AVAILABLE);
            //}
            return;
        }
        if (!allSkills[currentSkillIndex].IsCanUse())
        {
            return;
        }

        //if (AdManager.Instance.IsInterstitialLoaded())
        //{
        //    AdManager.Instance.ShowInterstitial();
        //}
        //else
        //{
            UseSkill(true);
        //}
    }
    public void ShowNextSkill(int id)
    {
        ButtonSkill[] allSkills = FindObjectsOfType<ButtonSkill>(true);
        for (int i = 0; i < allSkills.Length; i++)
        {
            if (allSkills[i].id == (id + 1))
            {
                allSkills[i].ShowSkill = true;
                allSkills[i].SaveShowSkill();
            }
        }
    }
    private void UseSkill(bool isReward)
    {
        if (!isReward) return;
        var skill = allSkills[currentSkillIndex];
        player.CastSkill(skill.id);
        skill.StartSkill(OnEndSkill);
        Controls.SetControlsSubPanel(ControlsType.None);
        CameraMode currentCameraMode = CameraManager.Instance.GetCurrentCameraMode();
        currentCameraMode.SetCameraConfigMode("Sprint");
    }
    private void UnlockSkill(bool isReward)
    {
        if (!isReward) return;
        currentSkill.Unlock();
        currentSkill = null;
    }

    private void OnEndSkill()
    {
        Controls.SetControlsSubPanel(ControlsType.Character);
        CameraMode currentCameraMode = CameraManager.Instance.GetCurrentCameraMode();
        currentCameraMode.SetCameraConfigMode("Default");
    }
}
