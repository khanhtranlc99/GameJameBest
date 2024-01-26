using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent.Training
{
	public class TrainingManager : MonoBehaviour
	{
		public delegate void SkipTrainingDelegate(int completeTrainingCount);

		public const string CompletedTrainingsArrayName = "CompletedTrainingsArray";

		private static SkipTrainingDelegate SkipTrainingEvent;

		private static TrainingManager instance;

		public TrainingBase[] StartTrainings;

		public TrainingBase[] OnActiveTrainings;

		public float TimeToStartTrainings;

		public float TimeBetweenTrainings;

		[Separator("UI Links")]
		public GameObject TrainingPanel;

		public Text MessageText;

		public Text ToContinueMessageText;

		public RectTransform[] Pointers;

		public Canvas RootCanvas;

		public RectTransform EmptyHelper;

		private static List<string> completedTrainings = new List<string>();

		private readonly List<TrainingBase> trainingsQueue = new List<TrainingBase>();

		private readonly Dictionary<GameObject, TrainingBase> trackedObjects = new Dictionary<GameObject, TrainingBase>();

		private TrainingBase currentTraining;

		private Image pointerImage;

		private float lastTrainingEndTime;

		 private bool trainingSkipped;

		public static TrainingManager Instance => instance;

		public static int CompleteTrainingsCount => completedTrainings.Count;

		public static void ClearCompletedTrainingsInfo()
		{
			BaseProfile.ClearArray<string>("CompletedTrainingsArray");
		}

		public void InitOffSceneTraining(TrainingBase training)
		{
			WatchedAtObject(training.ObjectForActive.gameObject, training);
		}

		public void ClearLocalCompletedTrainingsInfo()
		{
			completedTrainings.Clear();
		}

		public void SkipTraining()
		{
			trainingSkipped = true;
			BaseProfile.SkipTraining = true;
			currentTraining.EndTraining();
			trainingsQueue.Clear();
			SkipTrainingEvent?.Invoke(CompleteTrainingsCount);
		}

		public void CompleteThisTraining()
		{
			currentTraining.EndTraining();
		}

		public void SetTrainingStatus(bool activate)
		{
			trainingSkipped = !activate;
		}

		public void TrackedObjectActivated(TrainingObjectTracker tracker)
		{
			if (!trainingSkipped && trackedObjects.ContainsKey(tracker.gameObject))
			{
				lastTrainingEndTime = DateTime.Now.Second;
				AddTrainingInQueue(trackedObjects[tracker.gameObject]);
			}
		}

		public void TrackedObjectDestroyed(TrainingObjectTracker tracker)
		{
			if (trackedObjects.ContainsKey(tracker.gameObject))
			{
				TrainingBase item = trackedObjects[tracker.gameObject];
				if (trainingsQueue.Contains(item))
				{
					trainingsQueue.Remove(item);
				}
				trackedObjects.Remove(tracker.gameObject);
			}
		}

		public void TrainingEnd()
		{
			BaseProfile.StoreLastElementOfArray(currentTraining.TrainingName, "CompletedTrainingsArray");
			completedTrainings.Add(currentTraining.TrainingName);
			trainingsQueue.Remove(currentTraining);
			currentTraining.gameObject.SetActive(value: false);
			currentTraining = null;
			TrainingPanel.SetActive(value: false);
			lastTrainingEndTime = DateTime.Now.Second;
			BackButton.ChangeBackButtonsStatus(active: true);
		}

		public void ClickOnBlockerPanel()
		{
			currentTraining.ClickOnPanel();
		}

		private void AddTrainingInQueue(TrainingBase training)
		{
			if (!trainingsQueue.Contains(training))
			{
				trainingsQueue.Add(training);
			}
		}

		private void StartTraining(TrainingBase training)
		{
			if (trainingSkipped)
			{
				return;
			}
			if (!training.ObjectForActive.gameObject.activeInHierarchy || completedTrainings.Contains(training.TrainingName))
			{
				trainingsQueue.Remove(training);
				return;
			}
			currentTraining = training;
			TrainingPanel.SetActive(value: true);
			currentTraining.gameObject.SetActive(value: true);
			MessageText.text = currentTraining.ObjectDescription;
			ToContinueMessageText.text = currentTraining.GetContinueMessage();
			ConfigureHelper(training.ObjectForActive);
			RectTransform[] pointers = Pointers;
			foreach (RectTransform rectTransform in pointers)
			{
				EmptyHelper.SetParent(rectTransform.parent, worldPositionStays: true);
				rectTransform.anchorMin = EmptyHelper.anchorMin;
				rectTransform.anchorMax = EmptyHelper.anchorMax;
				rectTransform.anchoredPosition = EmptyHelper.anchoredPosition;
				rectTransform.sizeDelta = EmptyHelper.sizeDelta * (1f + currentTraining.AdditionalPointerScalling);
				rectTransform.pivot = EmptyHelper.pivot;
			}
			Image component = currentTraining.ObjectForActive.GetComponent<Image>();
			pointerImage.sprite = ((!component) ? null : component.sprite);
			pointerImage.preserveAspect = (!component || component.preserveAspect);
			pointerImage.type = (component ? component.type : Image.Type.Simple);
			currentTraining.StartTraining();
			BackButton.ChangeBackButtonsStatus(active: false);
		}

		private void Awake()
		{
			instance = this;
			pointerImage = Pointers[0].GetComponent<Image>();
			trainingSkipped = true;
			string[] array = BaseProfile.ResolveArray<string>("CompletedTrainingsArray");
			if (array != null)
			{
				completedTrainings = array.ToList();
			}
		}

		private IEnumerator Start()
		{
			yield break;
			yield return new WaitForSeconds(TimeToStartTrainings);
			TrainingBase[] startTrainings = StartTrainings;
			foreach (TrainingBase training in startTrainings)
			{
				AddTrainingInQueue(training);
				WatchedAtObject(training.ObjectForActive.gameObject, training);
			}
			TrainingBase[] onActiveTrainings = OnActiveTrainings;
			foreach (TrainingBase training2 in onActiveTrainings)
			{
				WatchedAtObject(training2.ObjectForActive.gameObject, training2);
			}
		}

		private void Update()
		{
			if (!trainingSkipped && trainingsQueue.Count != 0 && !(currentTraining != null) && !(Mathf.Abs((float)DateTime.Now.Second - lastTrainingEndTime) < TimeBetweenTrainings))
			{
				StartTraining(trainingsQueue[0]);
			}
		}

		private void WatchedAtObject(GameObject obj, TrainingBase training)
		{
			if (!trackedObjects.ContainsKey(obj))
			{
				obj.AddComponent<TrainingObjectTracker>();
				trackedObjects.Add(obj, training);
			}
		}

		private void ConfigureHelper(RectTransform element)
		{
			EmptyHelper.SetParent(element.parent, worldPositionStays: true);
			RectTransform rectTransform = (RectTransform)element.transform;
			EmptyHelper.localScale = Vector3.one;
			EmptyHelper.anchorMin = rectTransform.anchorMin;
			EmptyHelper.anchorMax = rectTransform.anchorMax;
			EmptyHelper.anchoredPosition = rectTransform.anchoredPosition;
			EmptyHelper.sizeDelta = rectTransform.sizeDelta;
			EmptyHelper.pivot = rectTransform.pivot;
		}
	}
}
