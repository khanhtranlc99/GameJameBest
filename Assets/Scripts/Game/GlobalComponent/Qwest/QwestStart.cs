using Game.Character;
using Game.Character.CharacterController;
using RopeHeroViceCity.UI_Features.UI_InGame.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent.Qwest
{
    public class QwestStart : MonoBehaviour
    {
        public Qwest Qwest;
        private Image img;
        private Transform target;
        private Text meter;
        public Vector3 offset;
        public QwestMark qwestMarkPrefab;
        private QwestMark mark;
        private bool updateMark = true;
        private bool hasInit;
        private Transform player;
        private Transform mainCamera;

        private void Start()
        {

            if (UI_InGame.Instance != null)
            {
                Init();
            }
            else
            {
                EventDispatcher.EventDispatcher.Instance.RegisterListener(EventID.SHOW_MARK, Init);
            }

        }
        private void Init(object obj = null)
        {
            if (!hasInit)
            {
                mark = Instantiate(qwestMarkPrefab);
                mark.gameObject.transform.SetParent(UI_InGame.Instance.transform);
                mark.transform.SetAsFirstSibling();
                mark.gameObject.GetComponent<Image>().SetNativeSize();
                img = mark.GetComponent<Image>();
                target = this.transform;
                meter = mark.text;
                player = FindObjectOfType<Player>().transform;
                mainCamera = Camera.main.transform;
                hasInit = true;
            }
        }
        private void OnDestroy()
        {
            EventDispatcher.EventDispatcher.Instance.RemoveListener(EventID.SHOW_MARK, Init);
        }
        private void OnEnable()
        {
            if (hasInit)
            {
                updateMark = true;
                mark.gameObject.SetActive(true);
                mark.transform.SetAsFirstSibling();
            }
        }
        private void OnTriggerEnter(Collider col)
        {
            if (col.GetComponent<CharacterSensor>() == null)
                return;
            if (!GameEventManager.Instance.TimeQwestActive)
            {
                updateMark = false;
                mark.gameObject.SetActive(false);
                GameEventManager.Instance.StartQwest(Qwest);
                PoolManager.Instance.ReturnToPool(this);
            }
        }

        private void Update()
        {
            if (!updateMark) return;
            if (!mainCamera.gameObject.activeSelf) return;

            float minX = img.GetPixelAdjustedRect().width / 2;
            float maxX = Screen.width - minX;

            float minY = img.GetPixelAdjustedRect().height / 2;
            float maxY = Screen.height - minY;

            Vector2 pos = Camera.main.WorldToScreenPoint(target.position + offset);

            if (Vector3.Dot((target.position - player.position), mainCamera.forward) < 0)
            {
                if (pos.x < Screen.width / 2)
                {
                    pos.x = maxX;
                }
                else
                {
                    pos.x = minX;
                }
            }
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            img.transform.position = pos;
            meter.text = ((int)Vector3.Distance(target.position, player.position)).ToString() + " m";
        }
    }
}
