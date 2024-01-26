using System;
using RopeHeroViceCity.Scripts.Gampelay;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Character.Stats
{
	public class StatBar : MonoBehaviour
	{
		[Tooltip("Если текстура белая/бесцветная и хотим её красить в нужный цвет")]
		public bool DoSetColor;

		public string keyStat;

		public Color BarColor = Color.white;

		public float fillAmountPercent = 1f;

		private Image fillImage;

		[SerializeField]
		private Slider slider;

		public bool DoBlink = true;

		public Animator BlinkingAnimator;

		private Image blinkingImage;

		private bool delayedBlink;

		private float dx;

		private Slider Slider
		{
			get
			{
				if (slider == null)
				{
					slider = GetComponent<Slider>();
					if (slider == null)
					{
						UnityEngine.Debug.LogError("Can't find Slider");
						base.enabled = false;
					}
				}
				return slider;
			}
		}

		 void Awake()
		{
			if (DoSetColor)
			{
				if (Slider != null && Slider.fillRect != null)
				{
					fillImage = Slider.fillRect.GetComponent<Image>();
					if (fillImage == null)
					{
						UnityEngine.Debug.LogError("Can't set the color because can't find Image");
					}
					else
					{
						fillImage.color = BarColor;
					}
				}
				else
				{
					UnityEngine.Debug.LogError("Can't set the color because fillRect is null");
				}
			}
			if (BlinkingAnimator == null)
			{
				BlinkingAnimator = GetComponentInChildren<Animator>();
				if (BlinkingAnimator == null)
				{
					UnityEngine.Debug.LogError("Can't find blinkingAnimator");
				}
				else
				{
					blinkingImage = BlinkingAnimator.GetComponent<Image>();
					if (blinkingImage == null)
					{
						UnityEngine.Debug.LogError("Can't find blinkingImage");
					}
				}
			}
			else
			{
				blinkingImage = BlinkingAnimator.GetComponent<Image>();
				if (blinkingImage == null)
				{
					UnityEngine.Debug.LogError("Can't find blinkingImage");
				}
			}
		}

		void OnEnable()
		{
			EventManager.Instance.onChangePlayerStat.AddListener(UpdateDisplayValue);
			if (delayedBlink)
			{
				Blink(blinkingImage.color);
				delayedBlink = false;
			}
		}

		private void OnDisable()
		{
			EventManager.Instance.onChangePlayerStat.RemoveListener(UpdateDisplayValue);
		}

		public void Blink(Color blinkingColor)
		{
			if (!(BlinkingAnimator == null) && !(blinkingImage == null))
			{
				blinkingColor.a = 0f;
				if (DoSetColor)
				{
					blinkingImage.color = blinkingColor;
				}
				if (base.gameObject.activeInHierarchy && base.enabled)
				{
					BlinkingAnimator.SetTrigger("Blink");
				}
				else
				{
					delayedBlink = true;
				}
			}
		}

		public void BlinkRed()
		{
			Blink(Color.red);
		}

		public void BlinkGreen()
		{
			Blink(Color.green);
		}

		private  void UpdateDisplayValue(string nameStat,float dx)
		{
			if(!keyStat.Equals(nameStat)) return;
			if (Slider != null)
			{
				Slider.value = dx * fillAmountPercent;
			}
			if (DoBlink || dx<.15f)
			{
				Blink(BarColor);
			}
		}
	}
}
