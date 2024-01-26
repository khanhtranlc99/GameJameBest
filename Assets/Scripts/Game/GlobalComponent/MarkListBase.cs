using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalComponent
{
	[CreateAssetMenu(fileName = "MarkList", menuName = "Library/Create Mark List", order = 1)]
	public class MarkListBase : ScriptableObject
	{
		[SerializeField]
		private List<MarkDetails> m_Details;

		public MarkDetails this[int index] => m_Details[index];

		public int Count => m_Details.Count;

		public int GetIndexByType(string type)
		{
			int count = Count;
			for (int i = 0; i < count; i++)
			{
				MarkDetails markDetails = m_Details[i];
				if (markDetails.markType.Equals(type))
				{
					return i;
				}
			}
			return -1;
		}

		public MarkDetails GetMarkByType(string type)
		{
			return m_Details[GetIndexByType(type)];
		}
	}
}
