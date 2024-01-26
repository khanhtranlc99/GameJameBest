using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class DangerIndicator : MonoBehaviour
	{
		private static DangerIndicator instance;

		public GameObject Panel;

		public Text Text;

		public float Duration = 1f;

		[Range(0f, 1f)]
		public float MaxAlpha = 1f;

		[Range(0f, 1f)]
		public float MinAlpha;

		[HideInInspector]
		public bool IsActive;

		private Image panelImage;

		private bool increase = true;

		public static DangerIndicator Instance
		{
			get
			{
				if (instance == null)
				{
					instance = UnityEngine.Object.FindObjectOfType<DangerIndicator>();
				}
				return instance;
			}
		}

		private void Awake()
		{
			if (!instance)
			{
				instance = this;
			}
			panelImage = Panel.GetComponent<Image>();
			if (!panelImage)
			{
				UnityEngine.Debug.LogError("Set panel for danger indicator and check that panel have component 'Image'");
			}
			Deactivate();
		}

		private void Update()
		{
			if (IsActive)
			{
				if (panelImage.canvasRenderer.GetAlpha() >= MaxAlpha && increase)
				{
					increase = false;
					panelImage.CrossFadeAlpha(MinAlpha, Duration, ignoreTimeScale: false);
				}
				else if (panelImage.canvasRenderer.GetAlpha() <= MinAlpha && !increase)
				{
					increase = true;
					panelImage.CrossFadeAlpha(MaxAlpha, Duration, ignoreTimeScale: false);
				}
			}
		}

		public void Activate(string message)
		{
			if (!IsActive)
			{
				Text.text = message;
				Panel.SetActive(value: true);
				IsActive = true;
			}
		}

		public void Deactivate()
		{
			if (IsActive)
			{
				Panel.SetActive(value: false);
				IsActive = false;
			}
		}
	}
}
