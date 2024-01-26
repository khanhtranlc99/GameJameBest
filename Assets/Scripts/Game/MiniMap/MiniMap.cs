using Game.Character.CharacterController;
using UnityEngine;
using UnityEngine.UI;
using UnitySampleAssets.CrossPlatformInput;

namespace Game.MiniMap
{
	public class MiniMap : SingletonMonoBehavior<MiniMap>
	{
		private const int AdditionalBorderSize = 1000;

		private const string MiniMapLayerName = "MiniMap";

		private const string ShaderSpecularColorPropertyName = "_SpecColor";

		private const string ShaderEmissionPropertyName = "_Emission";

		private const string HorizontalScrollAxis = "Horizontal_Map";

		private const string VerticalScrollAxis = "Vertical_Map";

		private const string ZoomAxis = "MapZoom";

		private static int miniMapLayerNumber = -1;

		public GameObject Target;

		public GameObject GameCamera;

		public Camera MiniMapCamera;

		public RectTransform WorldSpace;

		public Image PlayerIcon;

		public Image VisibleAreaImage;

		public Shader MapPlaneShader;

		public float MiniMapOrthographicSize = 120f;

		public float FullMapOrthographicSize = 550f;

		[Separator("FullMapControlOptions")]
		public float ScrollSpeed = 2f;

		public float ZoomSpeed = 10f;

		public float MinOrthographicSize = 250f;

		public float MaxOrthographicSize = 1600f;

		public GameObject UserMark;

		public GameObject PlayerMark;

		[Separator]
		public Texture MapTexture;

		public Vector2 MapTextureResolution = new Vector2(1024f, 1024f);

		[HideInInspector]
		public bool IsFullScreen;

		private readonly Color tintColor = new Color(1f, 1f, 1f, 0.9f);

		private readonly Color specularColor = new Color(1f, 1f, 1f, 0.9f);

		private readonly Color emessiveColor = new Color(0f, 0f, 0f, 0.9f);
		

		private GameObject firstTarget => PlayerManager.Instance.Player.gameObject;
		
		protected override void Awake()
		{
			base.Awake();
			if (miniMapLayerNumber == -1)
			{
				miniMapLayerNumber = LayerMask.NameToLayer("MiniMap");
			}
			Init();
		}

		private void Init()
		{
			MiniMapCamera.orthographicSize = MiniMapOrthographicSize;
			PlayerMark.SetActive(value: false);
			CreateMapPlane();
		}

		private void Update()
		{
			if ((bool)Target)
			{
				if (IsFullScreen)
				{
					FullMapControll();
					return;
				}
				PositionControl();
				RotationControl(PlayerIcon.gameObject, Target);
				RotationControl(VisibleAreaImage.gameObject, GameCamera);
			}
		}

		public void ApplyPlayerImage(Image playerIcon, Image visibaleArea)
		{
			this.PlayerIcon = playerIcon;
			this.VisibleAreaImage = visibaleArea;
		}

		public void SetTarget(GameObject newTarget)
		{
			Target = newTarget;
		}

		public void ResetTarget()
		{
			Target = firstTarget;
		}

		private void CreateMapPlane()
		{
			MiniMapCamera.cullingMask = 1 << miniMapLayerNumber;
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
			gameObject.transform.Rotate(Vector3.up, 180f);
			Vector3 vector = WorldSpace.sizeDelta;
			gameObject.transform.localPosition = WorldSpace.localPosition;
			gameObject.transform.localScale = new Vector3(vector.x, 1f, vector.y) / 10f;
			gameObject.layer = miniMapLayerNumber;
			gameObject.GetComponent<Renderer>().material = CreateMaterial();
		}

		private Material CreateMaterial()
		{
			Material material = new Material(MapPlaneShader);
			material.mainTexture = MapTexture;
			material.color = tintColor;
			Material material2 = material;
			material2.SetColor("_SpecColor", specularColor);
			material2.SetColor("_Emission", emessiveColor);
			return material2;
		}

		private void PositionControl()
		{
			Vector3 position = Target.transform.position;
			position.y = 0f;
			base.transform.position = position;
		}

		private void RotationControl(GameObject rotatedObject, GameObject sourceObject)
		{
			RectTransform component = rotatedObject.GetComponent<RectTransform>();
			Vector3 zero = Vector3.zero;
			Vector3 eulerAngles = sourceObject.transform.eulerAngles;
			zero.z = 0f - eulerAngles.y;
			component.eulerAngles = zero;
		}

		public void ChangeMapSize(bool fullScreen)
		{
			IsFullScreen = fullScreen;
			if (IsFullScreen)
			{
				MiniMapCamera.orthographicSize = FullMapOrthographicSize;
				PlayerMark.transform.position = Target.transform.position;
				PlayerMark.GetComponent<MarkForMiniMap>().RotateIcon(Target.transform.eulerAngles);
			}
			else
			{
				MiniMapCamera.orthographicSize = MiniMapOrthographicSize;
			}
			PlayerMark.SetActive(IsFullScreen);
		}

		private void FullMapControll()
		{
			Vector3 a = new Vector3(0f - CrossPlatformInputManager.GetVirtualOnlyAxis("Horizontal_Map", raw: false), 0f, 0f - CrossPlatformInputManager.GetVirtualOnlyAxis("Vertical_Map", raw: false));
			Vector3 b = ScrollSpeed * MiniMapCamera.orthographicSize * a;
			float num = CrossPlatformInputManager.GetAxis("MapZoom") * ZoomSpeed;
			Vector3 vector = base.transform.position + b;
			vector.y = 0f;
			Vector3 position = WorldSpace.transform.position;
			position.y = 0f;
			float value = MiniMapCamera.orthographicSize + num;
			float num2 = WorldSpace.rect.width / 2f + 1000f;
			value = Mathf.Clamp(value, MinOrthographicSize, num2);
			if (Vector3.Distance(vector, position) > num2 - value)
			{
				vector = base.transform.position;
				if (num > 0f && value != num2)
				{
					vector = Vector3.MoveTowards(vector, position, num);
				}
			}
			base.transform.position = vector;
			MiniMapCamera.orthographicSize = value;
		}

		public void LocateUserMark(Vector3 pointerPosition)
		{
			RaycastHit hitInfo;
			Ray ray = new Ray(MiniMapCamera.transform.position + pointerPosition, Vector3.down);
			if (Physics.Raycast(ray, out hitInfo, MiniMapCamera.farClipPlane, MiniMapCamera.cullingMask))
			{
				UserMark.transform.position = hitInfo.point;
				UserMark.SetActive(value: true);
			}
		}

		public void MarkOnClick(Vector3 pointerPosition)
		{
			Ray ray = new Ray(MiniMapCamera.transform.position + pointerPosition, Vector3.down);
			MarkButton markButton = null;
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, MiniMapCamera.farClipPlane, MiniMapCamera.cullingMask))
			{
				markButton = hitInfo.collider.gameObject.GetComponent<MarkButton>();
			}
			if (markButton != null)
			{
				markButton.OnMarkClick();
			}
		}
	}
}
