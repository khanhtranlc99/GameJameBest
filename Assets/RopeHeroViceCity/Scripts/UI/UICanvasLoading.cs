using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvasLoading : MonoBehaviour
{
    public void ShowLoading(bool autoHide)
    {
        gameObject.SetActive(true);
        if (autoHide)
            StartCoroutine(WaitToHideLoading());
    }

    public void HideLoading()
    {
        FunctionHelper.ShowDebugColor("Close Loading");
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    public static IEnumerator WaitToHideLoading()
    {
        yield return new WaitForSeconds(60f);
        UICanvasController.Instance.HideLoading();
    }
}
