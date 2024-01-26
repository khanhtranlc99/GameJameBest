using Game.Managers;
using UnityEngine;

public class TriggerableVFX : MonoBehaviour
{
	public bool ShowDebug;

	public ParticleSystem[] ParticleSystems;

	protected virtual void Start()
	{
		StopVFX();
		SetTriggerEvent();
	}

	private void OnDestroy()
	{
		StopVFX();
		UnsetTriggerEvent();
	}

	public virtual void SetTriggerEvent()
	{
	}

	public virtual void UnsetTriggerEvent()
	{
	}

	protected void StartVFX()
	{
		if (ShowDebug && GameManager.ShowDebugs)
		{
			UnityEngine.Debug.Log("We are starting..");
		}
		for (int i = 0; i < ParticleSystems.Length; i++)
		{
			ParticleSystems[i].Play();
		}
	}

	protected void StopVFX()
	{
		if (ShowDebug && GameManager.ShowDebugs)
		{
			UnityEngine.Debug.Log("Stop this shit!");
		}
		for (int i = 0; i < ParticleSystems.Length; i++)
		{
			ParticleSystems[i].Stop();
		}
	}
}
