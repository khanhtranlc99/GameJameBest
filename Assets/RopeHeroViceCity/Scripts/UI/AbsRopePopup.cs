using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsRopePopup : AbsUICanvas
{
    private bool shouldUnpause;
    public override void ShowLayer()
    {
        base.ShowLayer();
        if (GameplayUtils.OnPause)
        {
            shouldUnpause = false;
        }
        else
        {
            shouldUnpause = true;
            GameplayUtils.PauseGame();
        }
    }

    public override void Close()
    {
        base.Close();
        if(shouldUnpause)
            GameplayUtils.ResumeGame();
    }
}
