using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Random = UnityEngine.Random;
using Root.Scripts.Helper;

public enum PopupDisplayType
{
	None = 0,
	Zoom = 1,
	LeftToRight = 2,
	RightToLeft = 3,
	UptoBottom = 4,
	BottomToUp = 5,
	Fade = 6,
	Special = 7,
	Random = 8,
}
public class AbsUICanvas : MonoBehaviour
{
	public enum AnimKey
	{
		OpenPopup,
		ClosePopup,
	};

	public enum Position
	{
		Bottom = 0,
		Middle,
		Top
	}

	public enum AnimType
	{
		None = 0,
		Animation,
		Tween
	}

	[Space(10)]
	public GameObject gContentAll;

	[Space(10)]
	public AnimType layerAnimType;

	[Space(10)]
	public bool allowDestroy;
	public bool isGameLayer;
	//public bool lockCanvasScale;
	public bool hideBehindLayers;
	public RectTransform targetTween;
	[SerializeField] protected PopupDisplayType displayType = PopupDisplayType.Zoom;
	[Space(10)]
	public Position position = Position.Bottom;

	[Space(10)]
	public List<UIChildCanvas> childOrders;

	[HideInInspector]
	public Animator anim;
	[HideInInspector]
	public Canvas canvas;

	protected CanvasGroup canvasGroup;
	[HideInInspector]
	public GraphicRaycaster graphicRaycaster;
	[HideInInspector]
	public int layerIndex;
	[HideInInspector]
	public UICanvasKey layerKey = UICanvasKey.NONE;
	public bool isAllowBack;
	[HideInInspector]
	public bool isLayerAnimOpenDone;

	//[SerializeField]protected BaseUserDataAssetUIShow[] arrayShows;

	public Action OnClosePanel;
	protected const float Duration = .35f;

	public void InitLayer(UICanvasKey layerKey)
	{
		canvasGroup = GetComponent<CanvasGroup>();
		isLayerAnimOpenDone = false;
		this.layerKey = layerKey;
		canvas = GetComponent<Canvas>();
		anim = GetComponent<Animator>();
		graphicRaycaster = GetComponent<GraphicRaycaster>();
		if (displayType == PopupDisplayType.Random)
		{
			displayType = (PopupDisplayType)Random.Range(1, 6);
		}
		counterTime = 1;
	}

	public void SetLayerIndex(int index)
	{
		layerIndex = index;
	}

	/**
	* Khoi chay 1 lan khi layer duoc tao
	*/
	public virtual void StartLayer()
	{
#if  UNITY_EDITOR
		Debug.Log("Start layer ne : "+layerKey);
#endif
		if (layerAnimType == AnimType.None)
		{
			isLayerAnimOpenDone = true;
		}
		//arrayShows = gameObject.GetComponentsInChildren<BaseUserDataAssetUIShow>();
	}

	/**
	* Khoi chay 1 lan tren layer dau tien duoc tao tren scene
	*/
	public virtual void FirstLoadLayer()
	{

	}

	/**
	* Khoi chay khi layer duoc add vao list active
	*/
	public virtual void ShowLayer()
	{
	}

	/**
	* Khoi chay khi layer la layer dau tien
	*/
	public virtual void EnableLayer()
	{
		if(canvasGroup)
			canvasGroup.interactable = true;
		graphicRaycaster.enabled = true;
	}

	/**
	* Khoi chay 1 lan khi layer duoc goi lai
	*/
	public virtual void ReloadLayer()
	{
	}
	protected virtual void Update()
	{
		counterTime -= Time.deltaTime;
		if (counterTime > 0)
			return;
		counterTime = 1;
		UpdateEachSecond();
	}
	private float counterTime;
	protected virtual void UpdateEachSecond()
	{

	}
	public virtual void BeforeHideLayer()
	{
	}

	public virtual void DisableLayer()
	{
		if (position != Position.Middle)
			graphicRaycaster.enabled = false;
	}

	public virtual void HideLayer()
	{
		if (OnClosePanel != null)
		{
			OnClosePanel();
			OnClosePanel = null;
		}
		UICanvasController.Instance.UpdateQueue(layerKey);
	}

	public virtual void DestroyLayer()
	{
      
	}


	// func
	public void SetSortOrder(int order)
	{
		canvas.sortingOrder = order;

		if (childOrders != null)
			childOrders.ForEach(a => a.ResetOrder(canvas.sortingOrder));
	}
	public void PlayAnimation(AnimKey key)
	{
		if (anim != null)
		{
			isLayerAnimOpenDone = false;
			anim.enabled = true;
			if (canvasGroup != null) canvasGroup.interactable = false;
			anim.SetTrigger(key.ToString());
		}
		else
		{
			isLayerAnimOpenDone = true;
		}
	}
	public void PlayTween(AnimKey key)
	{
		if (canvasGroup != null) canvasGroup.interactable = false;
		switch (displayType)
		{
		case PopupDisplayType.None:
			break;
		case PopupDisplayType.Zoom:
			switch (key)
			{
			case AnimKey.OpenPopup:
				ZoomIn();
				break;
			case AnimKey.ClosePopup:
				ZoomOut();
				break;
			}
			break;
		case PopupDisplayType.Fade:
			switch (key)
			{
			case AnimKey.OpenPopup:
				FadeIn();
				break;
			case AnimKey.ClosePopup:
				FadeOut();
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(key), key, null);
			}
			break;
		case PopupDisplayType.Special:
			switch (key)
			{
			case AnimKey.OpenPopup:
				SpecialMoveIn();
				break;
			case AnimKey.ClosePopup:
				SpecialMoveOut();
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(key), key, null);
			}
			break;
		case PopupDisplayType.LeftToRight:
		case PopupDisplayType.RightToLeft:
		case PopupDisplayType.UptoBottom:
		case PopupDisplayType.BottomToUp:
			switch(key)
			{
			case AnimKey.OpenPopup:
				MoveIn();
				break;
			case AnimKey.ClosePopup:
				MoveOut();
				break;
			}
			break;
		case PopupDisplayType.Random:
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

    #region  TweenAnimation
	private void ZoomIn()
	{
		Vector3 originalScale = targetTween.localScale;
		targetTween.localScale = Vector3.zero;
		Sequence sequenceTween = DOTween.Sequence();
		sequenceTween.Join(targetTween.DOScale(originalScale, Duration)
			.SetEase(Ease.OutBack));
		if (canvasGroup)
		{
			canvasGroup.alpha = 0;
			sequenceTween.Join(canvasGroup.DOFade(1f,Duration));
		}			
		sequenceTween.onComplete = OnLayerOpenDone;
	}
	private void ZoomOut()
	{
		Vector3 originalScale = targetTween.localScale;
		Sequence sequenceTween = DOTween.Sequence();
		sequenceTween.Join(targetTween.DOScale(Vector3.zero, Duration)
			.SetEase(Ease.OutBack));
		if (canvasGroup)
		{
			sequenceTween.Join(canvasGroup.DOFade(0,Duration/2));
		}

		if (sequenceTween != null)
			sequenceTween.onComplete = () =>
			{
				OnLayerCloseDone();
				targetTween.localScale = originalScale;
			};
	}

	private void FadeIn()
	{
		if (canvasGroup)
		{
			float originalValue = canvasGroup.alpha;
			canvasGroup.alpha = 0;
			canvasGroup.DOFade(originalValue,Duration)
				.SetEase(Ease.OutBack)
				.onComplete = () =>
				{
					OnLayerOpenDone();
				};
		}
	}

	private void FadeOut()
	{
		if (canvasGroup)
		{
			float originalValue = canvasGroup.alpha;
			canvasGroup.DOFade(0,Duration)
				.SetEase(Ease.OutBack)
				.onComplete = () =>
				{
					OnLayerCloseDone();
					canvasGroup.alpha = originalValue;
				};
		}
	}
	private void MoveIn()
	{
		var type = displayType;
		var size = targetTween.rect.size;
		Vector3 _startPosition = targetTween.position;
		RectTransform canvasRect = canvas.GetComponent<RectTransform>();
		switch (type)
		{
		case PopupDisplayType.LeftToRight:
			_startPosition = new Vector3((-canvasRect.rect.size.x - size.x)/2, _startPosition.y, _startPosition.z);
			break;
		case PopupDisplayType.RightToLeft:
			_startPosition = new Vector3((size.x+ canvasRect.rect.size.x)/2, _startPosition.y, _startPosition.z);
			break;
		case PopupDisplayType.UptoBottom:
			_startPosition = new Vector3(_startPosition.x, (size.y+ canvasRect.rect.size.y)/2, _startPosition.z);
			break;
		case PopupDisplayType.BottomToUp:
			_startPosition = new Vector3(_startPosition.x, (-canvasRect.rect.size.y - size.y)/2, _startPosition.z);
			break;
		}
		Vector3 _endPosition = targetTween.localPosition;
		targetTween.localPosition = _startPosition;
		Sequence sequenceTween = DOTween.Sequence();
		sequenceTween.Join(targetTween.DOLocalMove(_endPosition, Duration)
			.SetEase(Ease.InBack));
		if (canvasGroup)
		{
			canvasGroup.alpha = 0;
			sequenceTween.Join(canvasGroup.DOFade(1f,Duration));
		}
		sequenceTween.onComplete = OnLayerOpenDone;
	}
	private void MoveOut()
	{
		var type = displayType;
		var size = targetTween.rect.size;
		Vector3 _endPosition = targetTween.localPosition;
		RectTransform canvasRect = canvas.GetComponent<RectTransform>();
		switch (type)
		{
		case PopupDisplayType.LeftToRight:
			_endPosition = new Vector3((-canvasRect.rect.size.x - size.x) / 2, _endPosition.y, _endPosition.z);
			break;
		case PopupDisplayType.RightToLeft:
			_endPosition = new Vector3((size.x + canvasRect.rect.size.x) / 2, _endPosition.y, _endPosition.z);
			break;
		case PopupDisplayType.UptoBottom:
			_endPosition = new Vector3(_endPosition.x, (size.y + canvasRect.rect.size.y) / 2, _endPosition.z);
			break;
		case PopupDisplayType.BottomToUp:
			_endPosition = new Vector3(_endPosition.x, (-canvasRect.rect.size.y - size.y) / 2, _endPosition.z);
			break;
		}
		Vector3 _startPosition = targetTween.localPosition;
		Sequence sequenceTween = DOTween.Sequence();
		sequenceTween.Join(targetTween.DOLocalMove(_endPosition, Duration)
			.SetEase(Ease.OutBack));
		if (canvasGroup)
		{
			sequenceTween.Join(canvasGroup.DOFade(0,Duration/2));
		}
		sequenceTween.onComplete = ()=>
		{
			OnLayerCloseDone();
			targetTween.localPosition = _startPosition;
		};
	}

	protected virtual void SpecialMoveIn()
	{
        
	}
	protected virtual void SpecialMoveOut()
	{
        
	}
    
    #endregion
    public virtual void Close()
    {
	    if(canvasGroup)
			canvasGroup.interactable = false;
	    if(layerAnimType == AnimType.None)
			graphicRaycaster.enabled = false;
		UICanvasController.Instance.HideLayer(this);
		//if(AdManager.Instance.IsInterstitialLoaded())
		//	AdManager.Instance.ShowInterstitial();
	}
	public virtual void ProcessBackButton()
	{
		if(layerAnimType == AnimType.Animation && !isLayerAnimOpenDone)
			return;
		if (isAllowBack)
		{
			Close();
		}
	}

	#region Anim Action Done

    protected bool IsLayerOpenDone => layerAnimType == AnimType.None || isLayerAnimOpenDone;

    protected virtual void OnLayerOpenDone()
	{
		if(anim)anim.enabled = false;
		if (canvasGroup != null) canvasGroup.interactable = true;
		FunctionHelper.ShowDebugColorRed("Open done");
		isLayerAnimOpenDone = true;
	}

    protected virtual void OnLayerCloseDone()
	{
		if (anim) anim.enabled = false;
		FunctionHelper.ShowDebugColorRed("Close done");
		if (canvasGroup != null) canvasGroup.interactable = true;
		HideLayer();
		UICanvasController.Instance.CacheLayer(this);
		isLayerAnimOpenDone = true;
	}

    #endregion
}
