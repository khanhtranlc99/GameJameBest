using System.Collections;
using System.Collections.Generic;
using Game.GlobalComponent;
using Root.Scripts.Helper;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitScene : MonoBehaviour
{
    [SerializeField] private GameObject objPanelConsent;
    [SerializeField] private Button btnYes,btnNo;

    private const string KEY_SAVE_DECIDED_CONSENT = "KEY_SAVE_DECIDED_CONSENT";

    public static bool IsFirstAccount
    {
        get
        {
            return PlayerPrefs.GetInt("IsFirstAccount", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("IsFirstAccount", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!BaseProfile.ResolveValue(SystemSettingsList.PerformanceDetected.ToString(), defaultValue: false))
        {
            // MenuPanelManager.FirstOpen.gameObject.SetActive(value: false);
            // MenuPanelManager.FirstOpen = null;
            GetComponent<LoadSceneController>().Load();
            return;
        }
        Invoke(nameof(LoadMenuScene),.1f);
        //IronSource.Agent.setMetaData("is_child_directed","false");
        // if (PlayerPrefs.HasKey(KEY_SAVE_DECIDED_CONSENT))
        // {
            //AdManager.Instance.Initialized();
           // Invoke(nameof(LoadMenuScene),.1f);
           // return;
        //}
        //objPanelConsent.SetActive(true);
        // btnYes.onClick.AddListener(OnClickYes);
        // btnNo.onClick.AddListener(OnClickNo);
    }

    // private void OnClickYes()
    // {
    //     //IronSource.Agent.setConsent(true);
    //     AdManager.Instance.Initialized();
    //     objPanelConsent.SetActive(false);
    //     PlayerPrefs.SetInt(KEY_SAVE_DECIDED_CONSENT,1);
    //     Invoke(nameof(LoadMenuScene),.1f);
    // }
    //
    // private void OnClickNo()
    // {
    //     PlayerPrefs.SetInt(KEY_SAVE_DECIDED_CONSENT,1);
    //     //IronSource.Agent.setConsent(false);
    //     AdManager.Instance.Initialized();
    //     objPanelConsent.SetActive(false);
    //     Invoke(nameof(LoadMenuScene),.1f);
    // }

    void LoadMenuScene()
    {
        if (IsFirstAccount)
        {
            SceneManager.LoadScene(Constants.Scenes.Game.ToString());
            IsFirstAccount = false;
        }
        else
            SceneManager.LoadScene("Menu");
    }
}
