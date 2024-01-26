using System.Collections;
using UnityEngine;

namespace Game.MiniMap
{
	public class MarkForMiniMap : MonoBehaviour
	{
		private const int MMLayer = 21;

		private const int MarkVisibleOffset = 13;

		private const int ScaleCounterReduction = 8;

		public Sprite IconImage;

		public Sprite ArrowImage;

		public bool RotateArrow = true;

		public bool FullMapArrow = true;

		public float NotFullMapVisibleRange;

		public Color Color = Color.white;

		public float IconScale = 20f;

		public float ArrowScale = 90f;

		public float SortLayer;

		private GameObject drawedIcon;

		private GameObject drawedArrow;

		[HideInInspector]
		public SpriteRenderer drawedIconSprite;

		private SpriteRenderer drawedArrowSprite;

		private bool drawing;

		private float iconY;

		private GameObject target;

		private Transform parent;

		private float visibleRange;

		private MiniMap miniMap;

		private void Awake()
		{
			Init();
		}

		private void Init()
		{
			miniMap = MiniMap.Instance;
			parent = miniMap.WorldSpace;
			visibleRange = miniMap.MiniMapOrthographicSize - 13f;
			Vector3 position = parent.position;
			iconY = position.y + 1f + SortLayer;
			DrawIcon();
		}

		protected virtual void DrawIcon()
		{
			if (!drawedIcon)
			{
				GameObject gameObject = new GameObject("MapIcon");
				Transform transform = gameObject.transform;
				Vector3 position = base.gameObject.transform.position;
				float x = position.x;
				float y = iconY;
				Vector3 position2 = base.gameObject.transform.position;
				transform.position = new Vector3(x, y, position2.z);
				gameObject.transform.eulerAngles = new Vector3(90f, 0f, 0f);
				gameObject.transform.localScale = new Vector3(IconScale, IconScale, IconScale);
				gameObject.layer = 21;
				gameObject.transform.parent = parent;
				drawedIconSprite = gameObject.AddComponent<SpriteRenderer>();
				drawedIconSprite.sprite = IconImage;
				drawedIconSprite.color = Color;
				drawedIcon = gameObject;
				drawing = true;
			}
		}

		private void DrawArrow(Vector3 position, bool draw)
		{
			if (!drawing)
			{
				return;
			}
			if (!drawedArrow)
			{
				GameObject gameObject = new GameObject("ArrowIcon");
				gameObject.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
				gameObject.transform.localScale = new Vector3(ArrowScale, ArrowScale, ArrowScale);
				gameObject.layer = 21;
				gameObject.transform.parent = parent;
				drawedArrowSprite = gameObject.AddComponent<SpriteRenderer>();
				drawedArrowSprite.sprite = ArrowImage;
				drawedArrowSprite.color = Color;
				drawedArrow = gameObject;
			}
			if (draw)
			{
				drawedArrow.transform.position = new Vector3(position.x, iconY, position.z);
				if (RotateArrow)
				{
					Vector3 position2 = base.transform.position;
					float x = position2.x;
					float y = iconY;
					Vector3 position3 = base.transform.position;
					Quaternion rotation = Quaternion.LookRotation(new Vector3(x, y, position3.z) - drawedArrow.transform.position);
					drawedArrow.transform.rotation = rotation;
				}
				Transform transform = drawedArrow.transform;
				Vector3 eulerAngles = drawedArrow.transform.eulerAngles;
				float y2 = eulerAngles.y;
				Vector3 eulerAngles2 = drawedArrow.transform.eulerAngles;
				transform.eulerAngles = new Vector3(90f, y2, eulerAngles2.z);
			}
			drawedArrow.SetActive(draw);
			drawedIcon.SetActive(!draw);
		}

		public void ShowIcon()
		{
			drawing = true;
			if ((bool)drawedIcon)
			{
				drawedIconSprite.color = new Color(Color.r, Color.g, Color.b, 1f);
			}
			if ((bool)drawedArrow)
			{
				drawedArrowSprite.color = new Color(Color.r, Color.g, Color.b, 1f);
			}
		}

		public void HideIcon()
		{
			drawing = false;
			if ((bool)drawedIcon)
			{
				drawedIconSprite.color = new Color(Color.r, Color.g, Color.b, 0f);
			}
			if ((bool)drawedArrow)
			{
				drawedArrowSprite.color = new Color(Color.r, Color.g, Color.b, 0f);
			}
		}

		public void RotateIcon(Vector3 eulerRotation)
		{
			if (!drawedIcon)
			{
				Init();
			}
			drawedIcon.transform.localEulerAngles = new Vector3(0f, 180f, eulerRotation.y);
		}

		public virtual void MarckOnClick()
		{
		}

		private void OnDestroy()
		{
			if ((bool)drawedIcon)
			{
				UnityEngine.Object.Destroy(drawedIcon);
			}
		}

		private void Update()
		{
			if (drawing)
			{
				Transform transform = drawedIcon.transform;
				Vector3 position = base.transform.position;
				float x = position.x;
				float y = iconY;
				Vector3 position2 = base.transform.position;
				transform.position = new Vector3(x, y, position2.z);
			}
			ScaleControll();
		}

		private IEnumerator CheckVisibility()
		{
			WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
			while (true)
			{
				target = miniMap.Target;
				if ((bool)target)
				{
					Vector3 position = target.transform.position;
					float x = position.x;
					float y = iconY;
					Vector3 position2 = target.transform.position;
					Vector3 playerPos = new Vector3(x, y, position2.z);
					Vector3 position3 = base.transform.position;
					float x2 = position3.x;
					float y2 = iconY;
					Vector3 position4 = base.transform.position;
					Vector3 iconPos = new Vector3(x2, y2, position4.z);
					float distance = Vector3.Distance(playerPos, iconPos);
					if (distance > visibleRange && !miniMap.IsFullScreen)
					{
						if (FullMapArrow)
						{
							Vector3 dirVec2 = (iconPos - playerPos).normalized * visibleRange;
							DrawArrow(dirVec2 + playerPos, draw: true);
						}
						else if (distance < NotFullMapVisibleRange)
						{
							Vector3 dirVec = (iconPos - playerPos).normalized * visibleRange;
							DrawArrow(dirVec + playerPos, draw: true);
						}
						else
						{
							DrawArrow(default(Vector3), draw: false);
						}
					}
					else
					{
						DrawArrow(default(Vector3), draw: false);
					}
				}
				yield return waitForEndOfFrame;
			}
		}

		private void ScaleControll()
		{
			float num = (!miniMap.IsFullScreen) ? 0f : (miniMap.MiniMapCamera.orthographicSize / 8f);
			drawedIcon.transform.localScale = new Vector3(IconScale + num, IconScale + num, IconScale + num);
		}

		private void OnDisable()
		{
			HideIcon();
			StopAllCoroutines();
		}

		private void OnEnable()
		{
			if ((bool)ArrowImage)
			{
				StartCoroutine(CheckVisibility());
			}
			ShowIcon();
		}
	}
}
