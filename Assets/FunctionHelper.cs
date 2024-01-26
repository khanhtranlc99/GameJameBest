using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FunctionHelper
{
    #region Logic rig2d

    public static float GetAbleAttackSpecial(Vector2 velocity, float distanceDetal, float gravity)
    {
        float velecityY = velocity.y;
        // Get Time Move To MaxY
        float timeGoMaxY = velecityY / gravity;
        // Get Distance Move To MaxY
        float disGoMaxY = velecityY * timeGoMaxY - 0.5f * gravity * timeGoMaxY * timeGoMaxY;
        // Get Time Move To Target New
        float timeGoMinY = Mathf.Pow(2 * (disGoMaxY + distanceDetal) / gravity, 0.5f);
        // Get Distance move Max follow coordinate
        float disMax = velocity.x * (timeGoMaxY + timeGoMinY);

        return disMax;
    }

    public static float LenghVector2Aquare(Vector2 vectorInput)
    {
        return vectorInput.x * vectorInput.x + vectorInput.y * vectorInput.y;
    }

    #endregion

    #region Debug

    public static void ShowDebug(object str)
    {
        //return;
#if !UNITY_EDITOR 
        return;
#endif
        Debug.Log("__________" + str.ToString() + "____________");
    }

    public static void ShowDebug(object str, object str2)
    {
        //return;
#if !UNITY_EDITOR
        return;
#endif
        Debug.Log("__________" + str.ToString() + "____________," + str2.ToString());
    }

    public static void ShowDebugColor(object str, object str2)
    {
        //return;
#if !UNITY_EDITOR
        return;
#endif
        Debug.Log("<color=blue>__________</color>" + str.ToString() + "____________," + str2.ToString());
    }

    public static void ShowDebugColorRed(object str, object str2)
    {
        //return;
#if !UNITY_EDITOR
        return;
#endif
        Debug.Log("<color=red>__________</color>" + str.ToString() + "____________," + str2.ToString());
    }

    public static void ShowDebugColor(object str)
    {
        //return;
#if !UNITY_EDITOR
        return;
#endif
        Debug.Log("<color=blue>__________</color>" + str.ToString() + "____________");
    }

    public static void ShowDebugColorRed(object str)
    {
        //return;
#if !UNITY_EDITOR
        return;
#endif
        Debug.Log("<color=red>__________</color>" + str.ToString() + "____________");
    }

    #endregion

    // String Define
    public static string layerPlayer = "Player";


}

public enum TypeCharactor : byte
{
    Robin = 0,
    Gorskiy = 1,
    Arnold = 2,
    Julius = 3,
    KaceyRich = 4,
    RonEtienne = 5,
    FeiLi = 6,
    BadGirl = 7,
    Terrance = 8,
    Raymonde = 9,
    Claus = 10,
    Thor = 11,
    IceLord = 12,
    CoreyThunderstring = 13,
    Neko = 14,
    Jeremy = 15,
    Shark = 16,
    Anonymous = 17,
    VargBlackburn = 18,
    Soldier = 19,
    ShovelDemon = 20,
    Maestro = 21,
    ChangWuKing = 22,
    Leonidas = 23,
    Mime = 24,
    Mike = 25,
    Sick = 26,
    Fernando = 27,
    Jim = 28,
    Devourer = 29,
    Y = 30,
    Cyberstar = 31,
    KingOctopus = 32,
    Reddish = 33,
    OldMan = 34,
    Hipster = 35,
    Tanithmetil = 36,
    Vlad = 37,
    Overlord = 38,
    Betty = 39,
    Penny = 40,
    Barbara = 41
}



public enum TypeBuyPlayer : byte
{
    Free = 1,
    PayByMoney = 2,
    PayByCoinOrMoney = 3,
    PayByMoneyOrInChest = 4
}

public enum TypeFree : byte
{
    FreeDefault = 0,
    WatchVideo = 1,
    InviteFriend = 2,
    PlayOnline = 3
}
