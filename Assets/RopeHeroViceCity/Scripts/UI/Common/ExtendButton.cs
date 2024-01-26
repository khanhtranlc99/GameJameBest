using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
[RequireComponent(typeof(Button))]
public class ExtendButton : MonoBehaviour
{
    private Button button;
    public float delayAnim = 0.35f;
    private Vector3 vectorScale = new Vector3(.1f, .1f, .1f);
    private UnityAction baseAction;
    private CanvasGroup canvasGroup;

    private bool isInited;
    void Awake()
    {
        if (isInited)
            return;
        Init();
        isInited = true;
    }
    private void Init()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }
        canvasGroup = GetComponent<CanvasGroup>();
        button.onClick.AddListener(PerformActionAnimation);
    }
    private void PerformActionAnimation()
	{
        SetInteraction(false);
        transform.DOPunchScale(vectorScale, delayAnim, 0, 0).OnComplete(ActionActive);
        //AudioAssistant.Shot(NameSound.UI_Touch);
    }
    private void ActionActive()
    {
        SetInteraction(true);
        if (baseAction != null)
        {
            baseAction();
        }
        
    }
    public void AddListener(UnityAction action)
    {
        if (action != null)
            baseAction = action;
    }
    public void SetInteraction(bool isInteraction)
    {
        if(!isInited)
        {
            Init();
            isInited = true;
        }
        button.interactable = isInteraction;
        if (canvasGroup != null) canvasGroup.alpha = isInteraction ? 1 : .7f;
    }
}
