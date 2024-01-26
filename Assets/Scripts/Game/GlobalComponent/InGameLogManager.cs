using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent
{
    public class InGameLogManager : MonoBehaviour
    {
        private static InGameLogManager instance;

        public Text TextSample;

        public GameObject InGameLogPanel;

        public RectTransform StartTextPosition;

        public int MaxMessageCount = 4;

        public float StartedShowTime;

        public float MessageLifeTime;

        private float lastMessageShowedTime;

        private IDictionary<GameObject, Coroutine> coroutineList = new Dictionary<GameObject, Coroutine>();

        private readonly List<GameObject> messageStack = new List<GameObject>();
        //fix
        public List<GameObject> showedMessage = new List<GameObject>();

        private readonly IDictionary<MessageType, string> messagePresets = new Dictionary<MessageType, string>
        {
            {
                MessageType.Money,
                "{0}$"
            },
            {
                MessageType.HealthPack,
                "+{0}HP"
            },
            {
                MessageType.Item,
                "{0}"
            },
            {
                MessageType.Bullets,
                "{0} ammo"
            },
            {
                MessageType.QuestItem,
                "{0}"
            },
            {
                MessageType.QuestStart,
                "'{0}' starting."
            },
            {
                MessageType.QuestFinished,
                "'{0}' complete!"
            },
            {
                MessageType.QuestFailed,
                "'{0}' failed!"
            },
            {
                MessageType.SitInCar,
                "{0}"
            },
            {
                MessageType.AddQuestTime,
                "+{0} seconds"
            },
            {
                MessageType.RadioName,
                "{0}"
            },
            {
                MessageType.Enegry,
                "+{0} enegry"
            },
            {
                MessageType.Experience,
                "+{0} EXP"
            },
            {
                MessageType.Gems,
                "+{0} Gems"
            },
            {
                MessageType.Collect,
                "+{0}"
            },
            {
                MessageType.NegativeMoney,
                "{0}$"
            }
        };

        private readonly IDictionary<MessageType, Color> colorPresets = new Dictionary<MessageType, Color>
        {
            {
                MessageType.Money,
                new Color(0.8f, 1f, 0.8f)
            },
            {
                MessageType.HealthPack,
                new Color(1f, 0.8f, 0.8f)
            },
            {
                MessageType.Item,
                new Color(0.7f, 0.4f, 0.7f)
            },
            {
                MessageType.Bullets,
                new Color(0.8f, 0.8f, 0.8f)
            },
            {
                MessageType.QuestItem,
                new Color(0.8f, 0.8f, 1f)
            },
            {
                MessageType.QuestStart,
                new Color(1f, 1f, 0.8f)
            },
            {
                MessageType.QuestFinished,
                new Color(1f, 1f, 0.8f)
            },
            {
                MessageType.QuestFailed,
                new Color(1f, 0f, 0f)
            },
            {
                MessageType.SitInCar,
                new Color(1f, 0.8f, 0.1f)
            },
            {
                MessageType.AddQuestTime,
                new Color(1f, 0.5f, 0.5f)
            },
            {
                MessageType.RadioName,
                new Color(0f, 0.5f, 1f)
            },
            {
                MessageType.Enegry,
                new Color(1f, 0.5f, 1f)
            },
            {
                MessageType.Experience,
                new Color(0f, 0.8f, 0f)
            },
            {
                MessageType.Gems,
                new Color(1f, 1f, 1f)
            },
            {
                MessageType.Collect,
                new Color(1f, 1f, 1f)
            },
            {
                MessageType.NegativeMoney,
                new Color(1f, 0f, 0f)
            }
        };

        public static InGameLogManager Instance
        {
            get
            {
                if (instance == null)
                {
                    //throw new Exception("InGameLogManager is not initialized");
                }
                return instance;
            }
        }

        public bool LogFree => Time.time > lastMessageShowedTime + StartedShowTime;

        private void Awake()
        {
            instance = this;
        }

        public void RegisterNewMessage(MessageType messageType, string specificString)
        {
            string format = messagePresets[messageType];
            Color color = colorPresets[messageType];
            GameObject fromPool = PoolManager.Instance.GetFromPool(TextSample.gameObject);
            RectTransform rectTransform = fromPool.transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.SetParent(InGameLogPanel.transform, worldPositionStays: false);
            }
            else
            {
                fromPool.transform.parent = InGameLogPanel.transform;
            }
            fromPool.transform.localScale = Vector3.one;
            Text component = fromPool.GetComponent<Text>();
            component.text = string.Format(format, specificString);
            component.color = color;
            fromPool.SetActive(value: false);
            messageStack.Add(fromPool);
        }

        private void Update()
        {
            if (!StartTextPosition.gameObject.activeInHierarchy)
            {
                return;
            }
            if (messageStack.Count > 0 && LogFree)
            {
                GameObject gameObject = messageStack[0];
                gameObject.GetComponent<RectTransform>().anchoredPosition = StartTextPosition.anchoredPosition;
                gameObject.SetActive(value: true);
                messageStack.Remove(gameObject);
                showedMessage.Add(gameObject);
                Coroutine value = StartCoroutine(RemoveMessageFromLog(gameObject, MessageLifeTime));
                if (coroutineList.ContainsKey(gameObject))
                {
                    UnityEngine.Debug.LogWarning("InGameLogManager: Unexpected logic, skipping message: " + gameObject.GetComponent<Text>().text);
                }
                else
                {
                    coroutineList.Add(gameObject, value);
                    lastMessageShowedTime = Time.time;
                }
            }
            if (showedMessage.Count >= MaxMessageCount)
            {
                StopCoroutine(coroutineList[showedMessage[0]]);
                RemoveMessageImmediate(showedMessage[0]);
            }
        }

        private IEnumerator RemoveMessageFromLog(GameObject message, float delay)
        {
            if (showedMessage.Contains(message))
            {
                yield return new WaitForSeconds(delay);
                RemoveMessageImmediate(message);
            }
        }

        private void RemoveMessageImmediate(GameObject message)
        {
            showedMessage.Remove(message);
            coroutineList.Remove(message);
            PoolManager.Instance.ReturnToPool(message);
        }
        private void RemoveAllMessage()
        {
            StopAllCoroutines();
            for (int i = 0; i < showedMessage.Count; i++)
            {
                coroutineList.Remove(showedMessage[i]);
                PoolManager.Instance.ReturnToPool(showedMessage[i]);
                showedMessage.Remove(showedMessage[i]);
            }
        }
        private void OnDisable()
        {
            RemoveAllMessage();
        }
    }
}
