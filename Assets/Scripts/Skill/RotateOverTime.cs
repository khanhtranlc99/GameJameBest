using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOverTime : MonoBehaviour
{
    public float rotateSpeed;
    public Transform objectToRotate;
    private Vector3 eulerSpin = Vector3.up;

    private void Update()
    {
        objectToRotate.Rotate(eulerSpin*rotateSpeed*Time.deltaTime);
    }
}
