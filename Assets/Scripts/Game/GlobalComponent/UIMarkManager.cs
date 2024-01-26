using Game.Character;
using Game.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class UIMarkManager : MonoBehaviour
	{
		private static UIMarkManager instance;

		private List<MarkContainer> m_DinamicMarks = new List<MarkContainer>();

		[Separator("Settings")]
		public Camera m_Camera;

		[SerializeField]
		private MarkContainer m_StaticMark;

		public Rect m_RectView = new Rect(0.1f, 0.1f, 0.8f, 0.8f);

		[Separator("Blocked windows")]
		public GameObject m_DialogWindow;

		private Vector3 viewportPos;

		private readonly Vector3 CenterViewPort = new Vector3(0.5f, 0.5f, 0f);

		[SerializeField]
		private MarkListBase m_MarksData;

		[SerializeField]
		private Transform m_MarkParent;

		[SerializeField]
		private UIMarkViewBase m_UIMarkSimplePrefab;

		public static UIMarkManager Instance => instance;

		public static bool InstanceExist => (!(instance == null)) ? true : false;

		private bool IsCanViewMark
		{
			get
			{
				if (m_DialogWindow != null && m_DialogWindow.activeSelf)
				{
					return false;
				}
				return true;
			}
		}

		public Transform TargetStaticMark
		{
			get
			{
				return m_StaticMark.Target;
			}
			set
			{
				m_StaticMark.Target = value;
			}
		}

		public void ActivateStaticMark(bool value)
		{
			if (value != m_StaticMark.mark.gameObject.activeSelf)
			{
				m_StaticMark.mark.gameObject.SetActive(value);
			}
		}

		private void HideMark(MarkContainer markContainer, bool value)
		{
			markContainer.mark.Hide(value);
		}

		private void UpdateMarkDistanceLabel(MarkContainer markContainer, float dist)
		{
			UIMarkView uIMarkView = markContainer.mark as UIMarkView;
			if (uIMarkView != null)
			{
				uIMarkView.UpdateDistanceLabel(dist);
			}
		}

		private void UpdateMarkPosition(MarkContainer markContainer)
		{
			if (markContainer.Target == null)
			{
				return;
			}
			viewportPos = m_Camera.WorldToViewportPoint(markContainer.Target.position + markContainer.offset);
			bool flag = viewportPos.z < 0f;
			bool flag2 = FastMath.PointInRect(viewportPos, m_RectView);
			if (!flag && flag2)
			{
				viewportPos.z = 0f;
				markContainer.mark.transform.position = m_Camera.ViewportToScreenPoint(viewportPos);
				return;
			}
			if (flag)
			{
				Vector3 b = m_Camera.WorldToViewportPoint(markContainer.Target.position);
				b.z = 0f;
				Vector3 vector = CenterViewPort - b;
				vector = FastMath.SetVectorLength(vector, 20f);
				viewportPos = CenterViewPort + vector;
			}
			viewportPos = FastMath.LineIntersectionRect(CenterViewPort, viewportPos, m_RectView);
			viewportPos.z = 0f;
			markContainer.mark.transform.position = m_Camera.ViewportToScreenPoint(viewportPos);
		}

		private float GetDistance(MarkContainer markContainer)
		{
			if (markContainer.Target == null)
			{
				return 0f;
			}
			if (PlayerInteractionsManager.Instance == null)
			{
				return Vector3.Distance(m_Camera.ViewportToWorldPoint(Vector3.one * 0.5f), markContainer.Target.position);
			}
			return Vector3.Distance(PlayerInteractionsManager.Instance.GetPlayerPosition(), markContainer.Target.position);
		}

		public MarkContainer AddDinamicMark(Transform target, string typeMark)
		{
			MarkDetails markByType = m_MarksData.GetMarkByType(typeMark);
			UIMarkViewBase markView = Instantiate(m_UIMarkSimplePrefab, m_MarkParent, worldPositionStays: false) as UIMarkViewBase;
			MarkContainer markContainer = new MarkContainer(markView, target, markByType);
			m_DinamicMarks.Add(markContainer);
			return markContainer;
		}

		public void RemoveDinamicMarks(MarkContainer mark)
		{
			if (mark != null && m_DinamicMarks != null && m_DinamicMarks.Count > 0)
			{
				int num = m_DinamicMarks.IndexOf(mark);
				if (num >= 0)
				{
					mark.FreeResources();
					m_DinamicMarks[num] = null;
				}
			}
		}

		private void RemoveNullMarks()
		{
			if (m_DinamicMarks != null)
			{
				m_DinamicMarks.RemoveAll((MarkContainer x) => x == null);
			}
		}

		private void UpdateStaticMark()
		{
			if (IsCanViewMark)
			{
				float distance = GetDistance(m_StaticMark);
				if (distance < m_StaticMark.MinDistanceView && m_StaticMark.MinDistanceView > 0f)
				{
					HideMark(m_StaticMark, value: true);
					return;
				}
				HideMark(m_StaticMark, value: false);
				UpdateMarkPosition(m_StaticMark);
				UpdateMarkDistanceLabel(m_StaticMark, distance);
			}
			else
			{
				HideMark(m_StaticMark, value: true);
			}
		}

		private void UpdateDinamicMarks()
		{
			int count = m_DinamicMarks.Count;
			for (int i = 0; i < count; i++)
			{
				MarkContainer markContainer = m_DinamicMarks[i];
				if (markContainer == null)
				{
					continue;
				}
				if (IsCanViewMark && markContainer.Target != null && markContainer.Target.gameObject.activeInHierarchy)
				{
					float distance = GetDistance(markContainer);
					if (distance < markContainer.MinDistanceView && markContainer.MinDistanceView > 0f)
					{
						HideMark(markContainer, value: true);
						continue;
					}
					HideMark(markContainer, value: false);
					UpdateMarkPosition(markContainer);
				}
				else
				{
					HideMark(markContainer, value: true);
				}
			}
		}

		private void Awake()
		{
			instance = this;
			m_Camera = Camera.main;
		}

		private void Start()
		{
		}

		private void Update()
		{
			UpdateStaticMark();
			UpdateDinamicMarks();
			RemoveNullMarks();
		}
	}
}
