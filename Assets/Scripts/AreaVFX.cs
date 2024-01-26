using Game.Managers;
using System.Collections.Generic;
using UnityEngine;

public class AreaVFX : BaseFX
{
	[Separator("AreaVFX parameters")]
	public bool DebugLog_AreaVFX;

	[Space(10f)]
	public GameObject[] GameObjects;

	[Tooltip("Множитель позволяет настроить масштаб эффекта для соответствия реальным размерам пространства. 0 означает, что по этой ветке изменений не нужно.")]
	[Space(5f)]
	public Vector3 ScaleMultiplier = Vector3.one;

	[Space(10f)]
	[Tooltip("Если есть образец - игнорирует остальные параметры.")]
	public SphereCollider SampleCollider;

	[Space(5f)]
	public float MinRadius = 1f;

	public float MaxRadius = 1f;

	public float GrowPerSecond;

	[Space(5f)]
	public bool StartFromMin = true;

	private ParticleSystem ps;

	private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

	private float defaultRadius;

	private float currRadius;

	private void Start()
	{
		if (SampleCollider == null)
		{
			defaultRadius = ((!StartFromMin) ? MaxRadius : MinRadius);
		}
		else
		{
			defaultRadius = SampleCollider.radius;
		}
		currRadius = defaultRadius;
		particleSystems.Capacity = 10;
		GameObject[] gameObjects = GameObjects;
		foreach (GameObject gameObject in gameObjects)
		{
			ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
			if (component != null)
			{
				particleSystems.Add(component);
			}
		}
		if (DebugLog_AreaVFX && GameManager.ShowDebugs)
		{
			UnityEngine.Debug.Log("AreaVFX " + base.gameObject.name + " started");
		}
		SetAreaRadius(currRadius);
		effectIsActive = true;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (effectIsActive)
		{
			AutoChangeSize();
		}
	}

	private void AutoChangeSize()
	{
		if (SampleCollider == null)
		{
			if (GrowPerSecond == 0f || currRadius < MinRadius || currRadius > MaxRadius)
			{
				return;
			}
			currRadius += GrowPerSecond * Time.deltaTime;
		}
		else
		{
			currRadius = SampleCollider.radius;
		}
		SetAreaRadius(currRadius);
	}

	private void StopAndClearParticleSystems()
	{
		foreach (ParticleSystem particleSystem in particleSystems)
		{
			particleSystem.Stop();
			particleSystem.Clear();
		}
	}

	private void StopParticleSystems()
	{
		foreach (ParticleSystem particleSystem in particleSystems)
		{
			particleSystem.Stop();
		}
	}

	private void SetAreaRadius(float radius)
	{
		foreach (ParticleSystem particleSystem in particleSystems)
		{
			particleSystem.transform.localScale = new Vector3((ScaleMultiplier.x != 0f) ? (radius * ScaleMultiplier.x) : 1f, (ScaleMultiplier.y != 0f) ? (radius * ScaleMultiplier.y) : 1f, (ScaleMultiplier.z != 0f) ? (radius * ScaleMultiplier.z) : 1f);
		}
	}

	public override void ActivateFX()
	{
		currRadius = defaultRadius;
		SetAreaRadius(currRadius);
		base.ActivateFX();
	}

	public override void DeactivateFX()
	{
		StopParticleSystems();
		base.DeactivateFX();
	}
}
