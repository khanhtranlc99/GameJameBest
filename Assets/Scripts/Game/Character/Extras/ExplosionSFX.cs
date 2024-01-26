using Game.GlobalComponent;
using System.Collections;
using UnityEngine;

namespace Game.Character.Extras
{
	public class ExplosionSFX : MonoBehaviour
	{
		public static ExplosionSFX Instance;

		public float PrefabDestructTime = 3f;

		private ParticleSystem[] emmiters;

		private GameObject ExplosionPrefab;

		private void Awake()
		{
			emmiters = GetComponentsInChildren<ParticleSystem>();
			Instance = this;
		}

		public void Emit(Vector3 pos, GameObject prefub)
		{
			Emit(pos, prefub, null, 1f);
		}

		public void Emit(Vector3 pos, GameObject prefub, AudioClip customSound, float customVolume = 1f)
		{
			ExplosionPrefab = prefub;
			if ((bool)customSound)
			{
				PointSoundManager.Instance.PlaySoundAtPoint(pos, customSound, customVolume);
			}
			else
			{
				PointSoundManager.Instance.PlaySoundAtPoint(pos, TypeOfSound.Explosion);
			}
			if (ExplosionPrefab == null)
			{
				base.transform.position = pos;
				ParticleSystem[] array = emmiters;
				foreach (var particleEmitter in array)
				{
					var emit = particleEmitter.emission;
					emit.enabled = true;
				}
			}
			else
			{
				StartCoroutine(EmitExplosionPrefab(pos));
			}
		}

		private IEnumerator EmitExplosionPrefab(Vector3 pos)
		{
			GameObject ExplosionPrefabClone = PoolManager.Instance.GetFromPool(ExplosionPrefab);
			ExplosionPrefabClone.transform.position = pos;
			yield return new WaitForSeconds(PrefabDestructTime);
			PoolManager.Instance.ReturnToPool(ExplosionPrefabClone);
		}
	}
}
