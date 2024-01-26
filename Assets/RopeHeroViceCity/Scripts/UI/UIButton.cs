using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Transform currentTransform;
    private Vector3 realScale;
    private float currentScale;
    private float scaleMulti = 0.1f;
    private float scaleTimeDown = 0.15f;
    private float scaleTimeUp = 0.1f;
    private void Start()
    {
        if (currentTransform == null)
        {
            currentTransform = this.transform;
        }
        currentScale = currentTransform.localScale.x;
        realScale = currentTransform.localScale;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        currentTransform.DOScale(currentScale - scaleMulti, scaleTimeDown).SetUpdate(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        currentTransform.DOScale(currentScale, scaleTimeUp).SetUpdate(true);
    }
    private void OnDestroy()
    {
        currentTransform.DOKill();
    }
}
