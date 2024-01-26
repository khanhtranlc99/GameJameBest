using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestTransparent : MonoBehaviour
{
    public Image img;

    private void Awake()
    {
        img.alphaHitTestMinimumThreshold = .1f;
    }
}
