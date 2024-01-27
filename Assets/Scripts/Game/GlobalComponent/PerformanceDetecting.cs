using Common.Analytics;
using Game.GlobalComponent.Quality;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace Game.GlobalComponent
{
	public class PerformanceDetecting : MonoBehaviour
	{
		public List<Sprite> lsData;
		public Image imageScene;
		[Serializable] 
		public class TestGroup
		{
			public string Name;

			public PerformanceTest[] PerformanceTests;
		}

		[Separator("Test result")]
		public float ResultToUltraQuality = 95f;

		public float ResultToHighQuality = 70f;

		public float ResultToMidQuality = 50f;

		public float ResultToLowQuality = 20f;

		[Separator("Minimum Requirements")]
		public float MinRam = 600f;

		[Separator("Test Group")]
		public TestGroup[] TestGroups;

		[Separator("All setup")]
		public GameObject ResultPanel;

		public LoadSceneController MainMenuLoader;

		public LoadSceneController BonusGameLoader;

		public Slider ProgressBar;

		public Text ProgressText;

		public Text Test;

		public bool IsDebug;

		private float detectingTime;

		private int index;

		private Dictionary<PerformanceTest, float> testResults = new Dictionary<PerformanceTest, float>();

		private bool testEnd;

		private bool startTesting;
		Coroutine temp;
		public void PlayBonusGame()
		{
			ResultPanel.SetActive(value: false);
			//AdsManager.ShowFullscreenAd();
			BonusGameLoader.Load();
		}

		private void Start()
		{
			Init();

			if (temp != null)
			{
				StopCoroutine(temp);
				temp = null;
			}
			temp = StartCoroutine(ChangeSprite());
		}

		private bool MinimumRequirements()
		{
			return (float)SystemInfo.systemMemorySize < MinRam;
		}

		private void Init()
		{
			startTesting = true;
			if(Test!=null)
				Test.text = SystemInfo.systemMemorySize.ToString();
			if (MinimumRequirements())
			{
				Сonclude(0f);
				return;
			}
			index = 0;
			StartTestGrop();
			detectingTime = 0f;
			for (int i = 0; i < TestGroups.Length; i++)
			{
				float num = TestGroups[i].PerformanceTests[0].DetectingTime;
				for (int j = 0; j < TestGroups[i].PerformanceTests.Length; j++)
				{
					if (TestGroups[i].PerformanceTests[j].DetectingTime > num)
					{
						num = TestGroups[i].PerformanceTests[j].DetectingTime;
					}
				}
				detectingTime += num;
			}
			ProgressBar.maxValue = detectingTime;
		}

		int ran;
		private IEnumerator ChangeSprite()
		{

			Debug.LogError("ChangeSprite");
			yield return new WaitForSeconds(1.5f);
			imageScene.DOColor(new Color32(0, 0, 0, 0), 1).OnComplete(delegate {

				ran = UnityEngine.Random.Range(0, lsData.Count);
				imageScene.sprite = lsData[ran];
				imageScene.DOColor(new Color32(255, 255, 255, 255), 1).OnComplete(delegate {

					if (temp != null)
					{
						StopCoroutine(temp);
						temp = null;
					}
					temp = StartCoroutine(ChangeSprite());


				});
			});


		}
       


        private void FixedUpdate()
		{
			if (startTesting)
			{
				ShowProgress();
			}
		}

		private void ShowProgress()
		{
			detectingTime -= Time.deltaTime;
			if (detectingTime <= 0f)
			{
				ProgressText.text = "100%";
				ProgressBar.value = ProgressBar.maxValue;
				ProgressBar.gameObject.SetActive(value: false);
			}
			else
			{
				ProgressBar.value = (1f - detectingTime / ProgressBar.maxValue) * ProgressBar.maxValue;
				ProgressText.text = ((int)((1f - detectingTime / ProgressBar.maxValue) * 100f)).ToString() + "%";
			}
		}

		private void StartTestGrop()
		{
			if (index >= TestGroups.Length || CalcResult() < ResultToMidQuality)
			{
				Сonclude(CalcResult());
				return;
			}
			PerformanceTest[] performanceTests = TestGroups[index].PerformanceTests;
			foreach (PerformanceTest performanceTest in performanceTests)
			{
				performanceTest.Init();
				performanceTest.EndTestingEvent += EndTest;
			}
		}

		private void EndTest(float result, PerformanceTest test)
		{
			if (testEnd)
			{
				return;
			}
			if (!test.IsNotReturnResults)
			{
				if (testResults.Count > 0 || !testResults.ContainsKey(test))
				{
					testResults.Add(test, result);
				}
				else
				{
					testResults[test] = result;
				}
			}
			if (TestGroups[index].PerformanceTests.All((PerformanceTest t) => t.IsEnd) || CalcResult() < ResultToMidQuality)
			{
				index++;
				StartTestGrop();
			}
		}

		private float CalcResult()
		{
			float num = 0f;
			ICollection<float> values = testResults.Values;
			foreach (float item in values)
			{
				float num2 = item;
				num += num2;
			}
			return num / (float)values.Count;
		}

		private void Сonclude(float res)
		{
			testEnd = true;
			detectingTime = 0f;
			bool flag = false;
			if (res > ResultToUltraQuality)
			{
				QualityManager.ChangeQuality(9);
				//BaseAnalyticsManager.Instance.SendEvent(EventsActionsTypes.P, "QUltra", 4);\
				Debug.Log("Old Log Show ");
			}
			else if (res > ResultToHighQuality)
			{
				QualityManager.ChangeQuality(8);
				//BaseAnalyticsManager.Instance.SendEvent(EventsActionsTypes.P, "QHigh", 3);
				Debug.Log("Old Log Show ");
			}
			else if (res > ResultToMidQuality)
			{
				QualityManager.ChangeQuality(7);
				Debug.Log("Old Log Show ");
				//BaseAnalyticsManager.Instance.SendEvent(EventsActionsTypes.P, "QMid", 2);
			}
			else if (res > ResultToLowQuality)
			{
				QualityManager.ChangeQuality(6);
				Debug.Log("Old Log Show ");
				//BaseAnalyticsManager.Instance.SendEvent(EventsActionsTypes.P, "QLow", 1);
			}
			else
			{
				ResultPanel.SetActive(value: true);
				QualityManager.ChangeQuality(6);
				Debug.Log("Old Log Show ");
				//BaseAnalyticsManager.Instance.SendEvent(EventsActionsTypes.P, "QToSlow", 0);
				flag = true;
			}
			if (!flag)
			{
				BaseProfile.StoreValue(true, SystemSettingsList.PerformanceDetected.ToString());
				ResultPanel.SetActive(false);
				MainMenuLoader.Load();
			}
		}

		public void PlayAnyway()
		{
			ResultPanel.SetActive(false);
			BaseProfile.StoreValue(true, SystemSettingsList.PerformanceDetected.ToString());
			//AdsManager.ShowFullscreenAd();
			MainMenuLoader.Load();
		}

		private void OnGUI()
		{
			if (IsDebug)
			{
				GUIStyle gUIStyle = new GUIStyle();
				GUIStyle gUIStyle2 = new GUIStyle();
				gUIStyle.fontSize = 60;
				gUIStyle2.fontSize = 62;
				gUIStyle.normal.textColor = Color.white;
				gUIStyle2.normal.textColor = Color.black;
				GUI.backgroundColor = Color.gray;
				GUI.Box(new Rect(15f, 60f, 500f, 200f), string.Empty);
				GUI.Label(new Rect(22f, 60f, 100f, 100f), "Result: " + CalcResult(), gUIStyle2);
				GUI.Label(new Rect(20f, 60f, 100f, 100f), "Result: " + CalcResult(), gUIStyle);
				GUI.Label(new Rect(22f, 120f, 100f, 100f), "Test ended: " + testResults.Count, gUIStyle2);
				GUI.Label(new Rect(20f, 120f, 100f, 100f), "Test ended: " + testResults.Count, gUIStyle);
				if (Test != null)
				{
					GUI.Label(new Rect(20f, 180f, 100f, 100f), "RAM: " + Test.text, gUIStyle2);
					GUI.Label(new Rect(22f, 180f, 100f, 100f), "RAM: " + Test.text, gUIStyle);
				}
			}
		}
	}
}
