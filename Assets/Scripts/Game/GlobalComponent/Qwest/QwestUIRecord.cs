using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class QwestUIRecord : MonoBehaviour
	{
		public Qwest Qwest;

		public void CancelQwest()
		{
			if (Qwest != null)
			{
				QwestTimerManager.Instance.QwestCanceled(Qwest);
				GameEventManager.Instance.QwestFailed(Qwest);
			}
		}

		public void RecordClick()
		{
			if (Qwest != null && GameEventManager.Instance.TaskSelectionAvailable)
			{
				GameEventManager.Instance.MarkedQwest = Qwest;
				GameEventManager.Instance.RefreshQwestArrow();
			}
		}
	}
}
