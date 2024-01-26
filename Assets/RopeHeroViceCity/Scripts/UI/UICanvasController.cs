using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UICanvasController : MonoBehaviour
{
    public Camera uiCamera;

    public UICanvasLoading layerLoading;

    public UICanvasKey firstLayer;

    public int deepOrder = 5;
    public int deepPlaneDistance = 100;
    //public bool isLandscape;

    public int[] deepOrderStarts = new int[3] { 0, 60, 120 };
    public int[] planeDistanceStarts = new int[3] { 1800, 1200, 600 };

    // Layer create when start game
    public List<UICanvasKey> readyLayers;

    public DataUICanvas data;
    // Save layer when close
    private Dictionary<UICanvasKey, List<AbsUICanvas>> layerCachesDict;
    // Layers active
    private Dictionary<AbsUICanvas.Position, List<AbsUICanvas>> layersDict;

    private Queue<AbsUICanvas> listQueueLayer = new Queue<AbsUICanvas>();
    public Action onLastLayerOfQueueClose;

    //public float screenRatio;

    private DateTime timePause;
    private bool isDisableBack = false;

    //  private Vector2 screnSize = new Vector2(Screen.width, Screen.height);

    //private float timeUpdate = 1f;
    //private float timeNow = 0f;
    //private float lastTime = 0;

    #region Singleton

    private static UICanvasController instance;

    public static UICanvasController Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<UICanvasController>();
            return instance;
        }
    }

    void OnDestroy()
    {
        instance = null;
    }

    #endregion

    #region Unity Method
    void Awake()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
        layersDict = new Dictionary<AbsUICanvas.Position, List<AbsUICanvas>>();
        layerCachesDict = new Dictionary<UICanvasKey, List<AbsUICanvas>>();

        layersDict.Add(AbsUICanvas.Position.Bottom, new List<AbsUICanvas>());
        layersDict.Add(AbsUICanvas.Position.Middle, new List<AbsUICanvas>());
        layersDict.Add(AbsUICanvas.Position.Top, new List<AbsUICanvas>());

        //screenRatio = (float)Screen.width / Screen.height;
        //lastTime = Time.realtimeSinceStartup;

        //if (screenRatio > 1)
        //{
        //    isLandscape = true;
        //}
        //else
        //{
        //    isLandscape = false;
        //}
    }

    void Start()
    {
        if (readyLayers != null && readyLayers.Count > 0)
        {
            foreach (var key in readyLayers)
            {
                AbsUICanvas layer = CreateLayer(key);
                layer.BeforeHideLayer();
                layer.HideLayer();
                layer.gameObject.SetActive(false);

                CacheLayer(layer);
            }
        }
        if (firstLayer != UICanvasKey.NONE)
        {
            AbsUICanvas mLayer = ShowLayer(firstLayer);
            mLayer.FirstLoadLayer();
        }
    }

    // void OnApplicationPause(bool paused)
    // {
    //     if (paused)
    //     {
    //         timePause = DateTime.Now;
    //     }
    //     else
    //     {
    //         if (timePause == DateTime.MinValue)
    //             return;
    //
    //         TimeSpan timeRange = DateTime.Now - timePause;
    //         if (timeRange.TotalSeconds > 600)
    //         {
    //
    //             //@Todo
    //             //GotoLogin();
    //             //LPopup.OpenPopup("WARNING", "Disconnect server");
    //         }
    //     }
    // }

    private void Update()
    {
        ProcessBackButton();
        //timeNow = Time.realtimeSinceStartup;
        //if (timeNow - lastTime > timeUpdate)
        //{
        //    lastTime = timeNow;
        //    //if (dataGlobal.eventManager.SendEventChangeSolutionScreen == null)
        //    //{
        //    //    return;
        //    //}
        //  //  UpdateStateRotation();
        //}

    }
    private void ProcessBackButton()
    {
        if (isDisableBack)
            return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AbsUICanvas currentCanvas = GetCurrentLayer(AbsUICanvas.Position.Bottom);
            if (currentCanvas != null && currentCanvas.isLayerAnimOpenDone)
                currentCanvas.ProcessBackButton();
        }
    }
    //public void UpdateStateRotation()
    //{
    //    screenRatio = (float)Screen.width / Screen.height;
    //    if (screenRatio > 1 && isLandscape == false)
    //    {
    //        isLandscape = true;
    //        dataGlobal.eventManager.SendEventChangeSolutionScreen(screenRatio, isLandscape);
    //    }
    //    else if (screenRatio < 1 && isLandscape == true)
    //    {
    //        isLandscape = false;
    //        dataGlobal.eventManager.SendEventChangeSolutionScreen(screenRatio, isLandscape);
    //    }
    //}
    #endregion

    #region Method
    // create
    /* Show layer ở ngay trên layer gọi ra nó
     * keyParent = layer gọi ra nó
     */
    public AbsUICanvas ShowLayer(UICanvasKey key)
    {
        return ShowLayer(CreateLayer(key));
    }
    public void SetStateBackButton(bool isDisable)
    {
        isDisableBack = isDisable;
    }
    public AbsUICanvas ShowLayerToQueueCache(UICanvasKey key)
    {
        AbsUICanvas layer = CreateLayer(key);
        layer.BeforeHideLayer();
        layer.HideLayer();
        layer.gameObject.SetActive(false);

        CacheLayer(layer);
        listQueueLayer.Enqueue(layer);
        ShowFirstLayerQueue();
        return layer;
    }

    private async void ShowFirstLayerQueue()
    {
        await Task.Yield();
        var firstQueueLayer = listQueueLayer.Peek();

        if (IsCurrentLayer(firstQueueLayer.layerKey))
            return;
        ShowLayer(firstQueueLayer.layerKey);
    }

    public bool IsHaveLayerAtQueue()
    {
        return listQueueLayer.Count > 0;
    }


    public void UpdateQueue(UICanvasKey key)
    {
        if (listQueueLayer.Count == 0)
        {
            return;
        }
        if (listQueueLayer.Peek().layerKey != key) return;
        listQueueLayer.Dequeue();
        if (listQueueLayer.Count == 0)
        {
            onLastLayerOfQueueClose?.Invoke();
            onLastLayerOfQueueClose = null;
            return;
        }
        ShowFirstLayerQueue();
    }
    public bool IsCachedLayer(UICanvasKey key)
    {
        return layerCachesDict.ContainsKey(key);
    }

    public AbsUICanvas ShowLayer(UICanvasKey key, GameObject obj)
    {
        return ShowLayer(CreateLayer(key, obj));
    }

    private AbsUICanvas ShowLayer(AbsUICanvas layer)
    {
        if (layer == null)
            return null;

        AbsUICanvas lastLayer = null;

        var uiLayerTemps = layersDict[layer.position];

        var layerCount = uiLayerTemps.Count;

        // disable layer
        if (layerCount > 0)
        {
            lastLayer = uiLayerTemps[layerCount - 1];
            lastLayer.DisableLayer();
        }


        layer.SetLayerIndex(layerCount);
        uiLayerTemps.Add(layer);
        layer.EnableLayer();

        // animation
        switch (layer.layerAnimType)
        {
            case AbsUICanvas.AnimType.Animation:
                layer.PlayAnimation(AbsUICanvas.AnimKey.OpenPopup);
                break;
            case AbsUICanvas.AnimType.Tween:
                layer.PlayTween(AbsUICanvas.AnimKey.OpenPopup);
                break;
            case AbsUICanvas.AnimType.None:
                if (layer.hideBehindLayers)
                {
                    if (lastLayer != null)
                        lastLayer.gameObject.SetActive(false);
                }
                break;
        }
        return layer;
    }

    private AbsUICanvas CreateLayer(UICanvasKey key, GameObject obj = null)
    {
        AbsUICanvas sLayer = null;
        string nameLayer = key.ToString();

        // get exists
        bool isCreate = true;

        if (layerCachesDict.ContainsKey(key) && layerCachesDict[key].Count > 0)
        {
            isCreate = false;
            sLayer = layerCachesDict[key][0];
            sLayer.gameObject.SetActive(true);
            layerCachesDict[key].RemoveAt(0);
        }
        else
        {
            if (obj == null)
            {
                int indexKey = (int)key;
                if (data.listPrefabUICanvas.Length < indexKey)
                {
                    FunctionHelper.ShowDebugColorRed("Obj no exits");
                    return null;
                }
                obj = data.listPrefabUICanvas[indexKey];
            }

            // Setup canvasScaler
            //if ((isLandscape && screenRatio > 1.9f) || (!isLandscape && screenRatio > 0.74f))
            //{
            //    if (!obj.GetComponent<AbsUICanvas>().lockCanvasScale)
            //    {
            //        CanvasScaler canvasScaler = obj.GetComponent<CanvasScaler>();
            //        canvasScaler.matchWidthOrHeight = 1f;
            //    }
            //}
            //else
            //{
            //    if (!obj.GetComponent<AbsUICanvas>().lockCanvasScale)
            //    {
            //        CanvasScaler canvasScaler = obj.GetComponent<CanvasScaler>();
            //        canvasScaler.matchWidthOrHeight = 0f;
            //    }
            //}

            obj = Instantiate(obj) as GameObject;
            sLayer = obj.GetComponent<AbsUICanvas>();

            // seting init
            sLayer.InitLayer(key);

            sLayer.canvas.renderMode = RenderMode.ScreenSpaceCamera;
            sLayer.canvas.worldCamera = uiCamera;
        }

        List<AbsUICanvas> uiLayerTemps = layersDict[sLayer.position];
        int countLayer = uiLayerTemps.Count;

        // set position
        int sorting = countLayer == 0 ? deepOrderStarts[(int)sLayer.position] : (uiLayerTemps[countLayer - 1].canvas.sortingOrder + deepOrder);
        float distance = countLayer == 0 ? planeDistanceStarts[(int)sLayer.position] : (uiLayerTemps[countLayer - 1].canvas.planeDistance - deepPlaneDistance);

        sLayer.transform.SetAsLastSibling();
        sLayer.name = nameLayer + "_" + (countLayer + 1);

        sLayer.SetSortOrder(sorting);
        sLayer.canvas.planeDistance = distance;

        // action
        if (isCreate)
            sLayer.StartLayer();
        sLayer.ShowLayer();

        return sLayer;
    }

    public void HideLayer(UICanvasKey key)
    {
        var layer = GetLayer(key);
        if (layer != null)
            HideLayer(layer);
    }

    public void HideLayer(AbsUICanvas layer)
    {
        if (layer == null)
            return;
        var uiLayerTemps = layersDict[layer.position];

        if (!uiLayerTemps.Contains(layer))
            return;
        // remove
        uiLayerTemps.Remove(layer);

        AbsUICanvas lastLayer = null;
        if (layer.layerIndex > 0 && uiLayerTemps.Count > layer.layerIndex - 1)
        {
            try
            {
                lastLayer = uiLayerTemps[layer.layerIndex - 1];
                lastLayer.gameObject.SetActive(true);
                lastLayer.ReloadLayer();
            }
            catch (Exception e)
            {
                FunctionHelper.ShowDebugColor("DONOT HAVE LAYER " + (layer.layerIndex - 1) + " - " + e.Message);
            }
        }

        if (layer.layerIndex == uiLayerTemps.Count)
        {
            if (lastLayer != null)
            {
                lastLayer.EnableLayer();
            }
        }
        else
        {
            for (int i = layer.layerIndex; i < uiLayerTemps.Count; i++)
                uiLayerTemps[i].SetLayerIndex(i);
        }

        // call hide
        layer.BeforeHideLayer();

        switch (layer.layerAnimType)
        {
            case AbsUICanvas.AnimType.None:
                layer.HideLayer();

                if (layer.allowDestroy)
                {
                    layer.DestroyLayer();
                    Destroy(layer.gameObject);
                    UnloadAllAssets();
                }
                else
                {
                    CacheLayer(layer);
                }
                break;
            case AbsUICanvas.AnimType.Animation:
                layer.PlayAnimation(AbsUICanvas.AnimKey.ClosePopup);
                break;
            case AbsUICanvas.AnimType.Tween:
                layer.PlayTween(AbsUICanvas.AnimKey.ClosePopup);
                break;
        }
    }

    public void CacheLayer(AbsUICanvas layer)
    {
        if (layer.allowDestroy)
        {
            layer.DestroyLayer();
            Destroy(layer.gameObject);
            UnloadAllAssets();
        }
        else
        {
            layer.gameObject.SetActive(false);
            if (!layerCachesDict.ContainsKey(layer.layerKey))
                layerCachesDict.Add(layer.layerKey, new List<AbsUICanvas>());
            layerCachesDict[layer.layerKey].Add(layer);
        }
    }

    private void PrivateHideLayer(AbsUICanvas layer)
    {
        layer.DisableLayer();
        layer.BeforeHideLayer();
        layer.HideLayer();

        if (layer.allowDestroy)
        {
            layer.DestroyLayer();
            Destroy(layer.gameObject);
        }
        else
        {
            CacheLayer(layer);
        }
    }

    private IEnumerator WaitRemoveLayerGame(AbsUICanvas layer)
    {
        yield return new WaitUntil(() => (layer == null || !layer.gameObject.activeSelf));

        RemoveLayerGame();
    }

    private void RemoveLayerGame()
    {
        foreach (var layerTemps in layerCachesDict.Select(item => item.Value).Where(layerTemps => layerTemps.Count > 0))
        {
            for (var i = layerTemps.Count - 1; i >= 0; i--)
            {
                var layer = layerTemps[i];
                if (!layer.isGameLayer) continue;
                layerTemps.Remove(layer);

                layer.DisableLayer();
                layer.BeforeHideLayer();
                layer.HideLayer();
                layer.DestroyLayer();

                Destroy(layer.gameObject);
            }
        }

        UnloadAllAssets();
    }

    // Back to login
    public void GotoLogin()
    {
        // hide mask
        HideLoading();
        //layerMiniMask.SetActive(false);

        // hide layer
        var loginLayer = layersDict[AbsUICanvas.Position.Bottom][0];

        // khong co lobby nen load truoc
        foreach (var layer in layersDict[AbsUICanvas.Position.Middle])
            PrivateHideLayer(layer);

        // khong co lobby nen load truoc
        foreach (var layer in layersDict[AbsUICanvas.Position.Top])
            PrivateHideLayer(layer);

        // khong co lobby nen load truoc
        var layerBottoms = layersDict[AbsUICanvas.Position.Bottom];
        for (int i = layerBottoms.Count - 1; i >= 0; i--)
        {
            var layer = layerBottoms[i];
            if (!layer.Equals(loginLayer))
                PrivateHideLayer(layer);
        }

        layersDict[AbsUICanvas.Position.Bottom].Clear();
        layersDict[AbsUICanvas.Position.Middle].Clear();
        layersDict[AbsUICanvas.Position.Top].Clear();

        layersDict[AbsUICanvas.Position.Bottom].Add(loginLayer);

        RemoveLayerGame();

        // clear Audio
        //AudioController.Instance.ClearAudioCache();

        UnloadAllAssets();

        loginLayer.transform.GetChild(0).localPosition = Vector3.zero;
        loginLayer.gameObject.SetActive(true);

        loginLayer.DisableLayer();
        loginLayer.ShowLayer();
        loginLayer.EnableLayer();
    }

    public void BackToLogin()
    {
        // hide mask
        HideLoading();
        //layerMiniMask.SetActive(false);

        // khong co lobby nen load truoc
        foreach (var layer in layersDict[AbsUICanvas.Position.Middle])
            PrivateHideLayer(layer);

        // khong co lobby nen load truoc
        foreach (var layer in layersDict[AbsUICanvas.Position.Top])
            PrivateHideLayer(layer);

        layersDict[AbsUICanvas.Position.Middle].Clear();
        layersDict[AbsUICanvas.Position.Top].Clear();

        RemoveLayerGame();

        // clear Audio
        //AudioController.Instance.ClearAudioCache();

        UnloadAllAssets();
    }

    // loading
    public void ShowLoading()
    {
        ShowLoading(true);
    }

    private void ShowLoading(bool autoHide)
    {
        layerLoading.ShowLoading(autoHide);
    }

    public void HideLoading()
    {
        layerLoading.HideLoading();
    }

    //close all popup bottom
    public void CloseAllPopupBottom()
    {
        var layerTemps = layersDict[AbsUICanvas.Position.Bottom].Where(a => a.layerKey == UICanvasKey.POPUP).ToList();
        foreach (var layer in layerTemps)
        {
            layer.Close();
        }
    }

    public void CloseAllOnePositionPanel(AbsUICanvas.Position popupPos)
    {
        var layerTemps = layersDict[popupPos].ToList();
        foreach (var layer in layerTemps)
        {
            layer.Close();
        }
    }

    public void CloseAllPanel()
    {
        var layerTemps = layersDict[AbsUICanvas.Position.Bottom].ToList();
        foreach (var layer in layerTemps)
        {
            layer.Close();
        }
        layerTemps = layersDict[AbsUICanvas.Position.Middle].ToList();
        foreach (var layer in layerTemps)
        {
            layer.Close();
        }
        layerTemps = layersDict[AbsUICanvas.Position.Top].ToList();
        foreach (var layer in layerTemps)
        {
            layer.Close();
        }
    }

    // get
    public T GetLayer<T>()
    {
        foreach (var layer in layersDict.SelectMany(item => item.Value).OfType<T>())
        {
            return (T)(object)layer;
        }

        return default(T);
    }

    public AbsUICanvas GetLayer(UICanvasKey key)
    {
        AbsUICanvas layer = null;
        foreach (var item in layersDict.Where(item => layer == null))
        {
            layer = item.Value.FirstOrDefault(a => a.layerKey == key);
        }

        return layer;
    }

    public AbsUICanvas GetCurrentLayer(AbsUICanvas.Position position)
    {
        var layerTemps = layersDict[position];
        return layerTemps.Count > 0 ? layerTemps[layerTemps.Count - 1] : null;
    }

    public bool IsCurrentLayer(UICanvasKey key)
    {
        return layersDict.Where(item => item.Value.Count > 0).Any(item => item.Value[item.Value.Count - 1].layerKey == key);
    }

    public bool IsLayerExisted<T>()
    {
        return GetLayer<T>() != null;
    }

    public bool IsLayerExisted(UICanvasKey key)
    {
        var exist = false;
        foreach (var item in layersDict.Where(item => !exist))
        {
            exist = item.Value.Exists(a => a.layerKey == key);
        }

        return exist;
    }

    public bool IsLayerLoadingActive()
    {
        return layerLoading.gameObject.activeSelf;
    }


    // unload aset
    private void UnloadAllAssets()
    {
        StartCoroutine(UnloadAllUnusedAssets());
    }

    private IEnumerator UnloadAllUnusedAssets()
    {
        yield return Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    #endregion

}
