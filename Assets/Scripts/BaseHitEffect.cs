using UnityEngine;

public abstract class BaseHitEffect : MonoBehaviour
{
	protected ParticleSystem[] emmiters;

	protected abstract void Awake();

	public abstract void Emit(Vector3 pos);
}
