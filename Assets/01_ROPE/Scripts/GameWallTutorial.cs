using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWallTutorial : MonoBehaviour
{
    public BoxCollider gO;

    public int IsDoMission
    {
        get
        {
            return PlayerPrefs.GetInt("IsDoMission", 0);
        }
        set
        {
            PlayerPrefs.SetInt("IsDoMission", value);
        }
    }

    public static int IsFirstTouch
    {
        get
        {
            return PlayerPrefs.GetInt("IsFirstTouch", 0);
        }
        set
        {
            PlayerPrefs.SetInt("IsFirstTouch", value);
        }
    }

    private void Update()
    {
        if (IsDoMission == 1 && IsFirstTouch == 0)
        {
            gO.enabled = true;
        }
        else
        {
            gO.enabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == 15)
        {
            if (IsFirstTouch == 1 && IsDoMission == 1)
            {
                gO.enabled = false;
            }
        }
    }
}
