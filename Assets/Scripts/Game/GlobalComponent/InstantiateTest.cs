using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class InstantiateTest : PerformanceTest
	{
		public GameObject[] InstantiateObjects;

		public Transform[] InstantiatePoints;

		public float MaxObjectOnScene;

		public float InstantiateObjectsCount;

		private float timer;

		private List<GameObject> ObjectsInScene = new List<GameObject>();

		private float Counter;

		private int count;

		public override void Init()
		{
			IsEnd = false;
			timer = DetectingTime / InstantiateObjectsCount;
			Counter = 1f;
			count = 0;
			if (timer < Time.fixedDeltaTime)
			{
				Counter = Time.fixedDeltaTime / timer;
			}
		}

		private void FixedUpdate()
		{
			if (IsEnd)
			{
				return;
			}
			if (timer <= 0f)
			{
				for (int i = 0; i < (int)Counter; i++)
				{
					try
					{
						TryInstantiate();
					}
					catch (Exception)
					{
						CatchInstantiate();
					}
				}
			}
			timer -= Time.fixedDeltaTime;
			if ((float)count >= InstantiateObjectsCount)
			{
				EndTesting();
			}
		}

		private void TryInstantiate()
		{
			Transform transform = InstantiatePoints[UnityEngine.Random.Range(0, InstantiatePoints.Length)];
			GameObject gameObject = UnityEngine.Object.Instantiate(InstantiateObjects[UnityEngine.Random.Range(0, InstantiateObjects.Length)]);
			gameObject.transform.position = transform.position;
			gameObject.transform.rotation = transform.rotation;
			gameObject.name = count.ToString();
			count++;
			ObjectsInScene.Add(gameObject);
			if ((float)ObjectsInScene.Count > MaxObjectOnScene)
			{
				GameObject gameObject2 = ObjectsInScene.First();
				ObjectsInScene.Remove(gameObject2);
				UnityEngine.Object.Destroy(gameObject2);
			}
		}

		private void CatchInstantiate()
		{
			Result -= 10f;
		}

		public override void EndTesting()
		{
			CallEndTestingEvent(Result, this);
			MonoBehaviour.print(base.name + " " + Result);
		}
	}
}
