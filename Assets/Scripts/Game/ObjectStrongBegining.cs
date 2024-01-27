using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStrongBegining : MonoBehaviour
{

    public static ObjectStrongBegining Instance;
   public GameObject ObBegining;

    public void Awake()
    {

        Instance = this;
    }





}
