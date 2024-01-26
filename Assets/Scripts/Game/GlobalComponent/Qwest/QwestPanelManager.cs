using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent.Qwest
{
	public class QwestPanelManager : MonoBehaviour
	{
		public Sprite markSprite;

		public Sprite defaultSprite;

		public List<QwestUIRecord> Records;

		public void OnPanelOpen()
		{
			try
			{
				if (GameEventManager.Instance == null)
				{
					return;
				}
			}
			catch (Exception)
			{
				return;
				IL_0020:;
			}
			ReloadRecords();
		}

		public void ReloadRecords()
		{
			foreach (QwestUIRecord record in Records)
			{
				record.gameObject.SetActive(value: false);
				record.Qwest = null;
			}
			int num = 0;
			foreach (Qwest activeQwest in GameEventManager.Instance.ActiveQwests)
			{
				QwestUIRecord qwestUIRecord = Records[num + 1];
				qwestUIRecord.gameObject.SetActive(value: true);
				qwestUIRecord.Qwest = activeQwest;
				Text componentInChildren = qwestUIRecord.GetComponentInChildren<Text>();
				componentInChildren.text = activeQwest.GetQwestStatus();
				num++;
			}
			if (GameEventManager.Instance.ActiveQwests.Count == 0)
			{
				QwestUIRecord qwestUIRecord2 = Records[0];
				qwestUIRecord2.gameObject.SetActive(value: true);
			}
			else
			{
				Records[0].gameObject.SetActive(value: false);
			}
			ReviewRecords();
		}

		public void ReviewRecords()
		{
			foreach (QwestUIRecord record in Records)
			{
				Image component = record.GetComponent<Image>();
				if (record.Qwest != null && record.Qwest.Equals(GameEventManager.Instance.MarkedQwest))
				{
					component.sprite = markSprite;
				}
				else if (record.Qwest != null)
				{
					component.sprite = defaultSprite;
				}
			}
		}

		private void OnEnable()
		{
			OnPanelOpen();
		}
	}
}
