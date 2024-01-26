using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWall : MonoBehaviour
{
    public float lifetime;

    private float cachedStartTime;
    public void StartWorking()
    {
        cachedStartTime = Time.time;
    }

    private void Update()
    {
        var timeLine = Time.time - cachedStartTime;
        if (timeLine > lifetime)
        {
            EndLife();
        }
    }

    private void EndLife()
    {
        gameObject.SetActive(false);
    }
}
