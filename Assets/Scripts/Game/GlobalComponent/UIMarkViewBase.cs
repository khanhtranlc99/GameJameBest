using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent
{
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(RectTransform))]
	public class UIMarkViewBase : MonoBehaviour
	{
		[SerializeField]
		private RectTransform m_Rect;

		[SerializeField]
		private Image m_Image;

		[SerializeField]
		private CanvasGroup m_CanvasGroup;

		public RectTransform Rect
		{
			get
			{
				if (m_Rect == null)
				{
					m_Rect = GetComponent<RectTransform>();
				}
				return m_Rect;
			}
		}

		public Color IconColor
		{
			get
			{
				return m_Image.color;
			}
			set
			{
				m_Image.color = value;
			}
		}

		internal void SetIconSprite(Sprite pic)
		{
			m_Image.sprite = pic;
		}

		internal void Hide(bool value)
		{
			m_CanvasGroup.alpha = ((!value) ? 1f : 0f);
		}
	}
}
