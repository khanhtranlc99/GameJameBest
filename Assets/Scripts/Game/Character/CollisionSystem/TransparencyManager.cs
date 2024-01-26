using Game.Character.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.CollisionSystem
{
	public class TransparencyManager : MonoBehaviour
	{
		private class TransObject
		{
			public float originalAlpha;

			public bool fadeIn;

			public float fadeoutTimer;
		}

		private const float fadeoutTimerMax = 0.1f;

		private static TransparencyManager instance;

		public float TransparencyMax = 0.5f;

		public float TransparencyFadeOut = 0.2f;

		public float TransparencyFadeIn = 0.1f;

		private float fadeVelocity;

		private Dictionary<GameObject, TransObject> objects;

		public static TransparencyManager Instance
		{
			get
			{
				if (!instance)
				{
					instance = CameraInstance.CreateInstance<TransparencyManager>("TransparencyManager");
				}
				return instance;
			}
		}

		private void Awake()
		{
			instance = this;
			objects = new Dictionary<GameObject, TransObject>();
		}

		private void Update()
		{
			Dictionary<GameObject, TransObject>.Enumerator enumerator = objects.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<GameObject, TransObject> current = enumerator.Current;
				current.Value.fadeoutTimer += Time.deltaTime;
				if (current.Value.fadeoutTimer > 0.1f)
				{
					current.Value.fadeIn = false;
				}
				float alpha = GetAlpha(current.Key);
				bool flag = false;
				if (current.Value.fadeIn)
				{
					alpha = Mathf.SmoothDamp(alpha, TransparencyMax, ref fadeVelocity, TransparencyFadeIn);
				}
				else
				{
					alpha = Mathf.SmoothDamp(alpha, current.Value.originalAlpha, ref fadeVelocity, TransparencyFadeOut);
					if (Mathf.Abs(alpha - current.Value.originalAlpha) < Mathf.Epsilon)
					{
						flag = true;
						alpha = current.Value.originalAlpha;
					}
				}
				SetAlpha(current.Key, alpha);
				if (flag)
				{
					objects.Remove(current.Key);
					break;
				}
			}
			enumerator.Dispose();
		}

		public void UpdateObject(GameObject obj)
		{
			TransObject value = null;
			if (objects.TryGetValue(obj, out value))
			{
				value.fadeIn = true;
				value.fadeoutTimer = 0f;
			}
			else
			{
				objects.Add(obj, new TransObject
				{
					originalAlpha = GetAlpha(obj),
					fadeIn = true,
					fadeoutTimer = 0f
				});
			}
		}

		private static void SetAlpha(GameObject obj, float alpha)
		{
			MeshRenderer component = obj.GetComponent<MeshRenderer>();
			if ((bool)component)
			{
				Material sharedMaterial = component.sharedMaterial;
				if ((bool)sharedMaterial)
				{
					Color color = sharedMaterial.color;
					color.a = alpha;
					sharedMaterial.color = color;
				}
			}
		}

		private static float GetAlpha(GameObject obj)
		{
			MeshRenderer component = obj.GetComponent<MeshRenderer>();
			if ((bool)component)
			{
				Material sharedMaterial = component.sharedMaterial;
				if ((bool)sharedMaterial)
				{
					Color color = sharedMaterial.color;
					return color.a;
				}
			}
			return 1f;
		}

		private void OnApplicationQuit()
		{
			foreach (KeyValuePair<GameObject, TransObject> @object in objects)
			{
				SetAlpha(@object.Key, @object.Value.originalAlpha);
			}
		}
	}
}
