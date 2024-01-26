using Game.Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSkills : MonoBehaviour
{
    public static Action OnSkillStartAction;
    public static Action OnSkillEndAction;
    public float skillLifeTime = 3f;
    private float cachedStartTime;
    protected bool isSkillStarting;
    public int id;
    private Action OnEndSkill;

    public bool isUseStamina;
    public int staminaUse;

    public bool IsCanUse()
    {
        if (isUseStamina)
        {
            if (PlayerInteractionsManager.Instance.Player.stats.stamina.Current >= staminaUse)
            {
                PlayerInteractionsManager.Instance.Player.stats.stamina.SetAmount(-staminaUse);
                return true;
            }
            else
            {
                UICanvasController.Instance.ShowLayer(UICanvasKey.BUY_STAMINA);
                return false;
            }
        }

        return true;
    }

    public virtual void StartSkill(Action onDoneAction)
    {
        OnSkillEndAction?.Invoke();
        cachedStartTime = Time.time;
        isSkillStarting = true;
        OnEndSkill = onDoneAction;
    }

    protected virtual void EndSkill()
    {
        OnEndSkill?.Invoke();
    }

    private void Update()
    {
        if (!isSkillStarting) return;
        var timeLine = Time.time - cachedStartTime;
        UpdateSkill(timeLine);
        if (timeLine > skillLifeTime)
        {
            EndSkill();
            isSkillStarting = false;
            OnSkillEndAction?.Invoke();
        }
    }

    protected virtual void UpdateSkill(float timeLine)
    {

    }
}
