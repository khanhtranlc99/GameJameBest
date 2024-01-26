using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.CameraEffects
{
	internal class EffectManager : MonoBehaviour
	{
		private static EffectManager instance;

		private List<CameraEffect> effects;

		public static EffectManager Instance => instance;

		private void Awake()
		{
			instance = this;
			effects = new List<CameraEffect>();
		}

		public void Register(CameraEffect cameraEffect)
		{
			if (cameraEffect != null)
			{
				effects.Add(cameraEffect);
			}
		}

		public void StopAll()
		{
			foreach (CameraEffect effect in effects)
			{
				effect.Stop();
			}
		}

		public T Create<T>() where T : CameraEffect
		{
			T val = base.gameObject.GetComponent<T>();
			if (!(Object)val)
			{
				val = base.gameObject.AddComponent<T>();
				if ((bool)(Object)val)
				{
					Register(val);
					val.Init();
				}
			}
			return val;
		}

		public CameraEffect Create(Type effectType)
		{
			switch (effectType)
			{
			case Type.Earthquake:
				return Create<Earthquake>();
			case Type.Explosion:
				return Create<ExplosionCFX>();
			case Type.No:
				return Create<No>();
			case Type.FireKick:
				return Create<FireKick>();
			case Type.Stomp:
				return Create<Stomp>();
			case Type.Yes:
				return Create<Yes>();
			case Type.SprintShake:
				return Create<SprintShake>();
			default:
				return null;
			}
		}

		public void Delete(CameraEffect cameraEffect)
		{
			if (effects.Contains(cameraEffect))
			{
				effects.Remove(cameraEffect);
			}
		}

		public void PostUpdate()
		{
			for (int i = 0; i < effects.Count; i++)
			{
				CameraEffect cameraEffect = effects[i];
				if (cameraEffect.Playing)
				{
					cameraEffect.PostUpdate();
				}
			}
		}

		private void OnGUI()
		{
		}
	}
}
