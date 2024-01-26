using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticScaleButton : MonoBehaviour
{
    public bool state;

    private void Start()
    {
        Activate(true);
    }

    private void Activate(bool value)
    {
        state = value;
        if (value)
        {
            transform.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            transform.DOKill();
            GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
    }
}
