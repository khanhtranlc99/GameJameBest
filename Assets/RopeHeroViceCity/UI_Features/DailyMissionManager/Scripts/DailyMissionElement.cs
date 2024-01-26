using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.DailyMission
{
    public class DailyMissionElement : MonoBehaviour
    {
        [SerializeField] private Toggle tglTaskDone;
        [SerializeField] private TextMeshProUGUI txtDescription,txtProcess;

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public void SetProcess(string process)
        {
            txtProcess.text = process;
        }

        public void SetDone(bool isDone)
        {
            tglTaskDone.isOn = isDone;
        }

        public void SetDecription(string description)
        {
            txtDescription.text = description;
        }
    }
}
