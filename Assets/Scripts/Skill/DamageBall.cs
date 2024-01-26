using System;
using System.Collections;
using System.Collections.Generic;
using Game.Character.CharacterController;
using Game.Effects;
using Game.GlobalComponent;
using UnityEngine;

public class DamageBall : Effect
{
    public LayerMask AffectedLayerMask;
    private HitEntity owner;
    public DamageType DamageType = DamageType.Instant;
    [Space(5f)]
    [SerializeField]
    protected float ExplosionRange = 10f;

    [SerializeField]
    protected float MaxDamage = 100000f;    
    [SerializeField]
    protected float damageInterval = 100000f;

    private float cachedTimeInterval;
    [SerializeField]
    protected float lifeTime = 100000f;    
    [SerializeField]
    protected float speed = 5f;

    [SerializeField] private GameObject ExplosionPrefab;

    private bool isDeInit;

    private float cachedSpawnTime;

    private bool isMove = false;
    [SerializeField] private ParticleSystem explosionFx;
    [SerializeField] private GameObject objFireBall;
    public void Init(HitEntity initiator)
    {
        owner = initiator;
        isMove = false;
        isDeInit = false;
    }

    private void Update()
    {
        Moving();
        DamageOverTime();
        var lineTime = Time.time - cachedSpawnTime;
        if (lineTime > lifeTime && !isDeInit)
        {
            isDeInit = true;
            DeInit();
        }
    }

    private void Moving()
    {
        if(isMove)
            transform.Translate(Vector3.forward*speed*Time.deltaTime);
    }

    private void DamageOverTime()
    {
        var lineTime = Time.time - cachedTimeInterval;
        if (lineTime > damageInterval)
        {
            cachedTimeInterval = Time.time;
            DealDamage();
        }
    }

    private void DealDamage()
    {
        Collider[] array = Physics.OverlapSphere(transform.position, ExplosionRange, AffectedLayerMask);
        foreach (Collider collider in array)
        {
            var hitted = collider.GetComponent<HitEntity>();
            if(hitted == null) continue;
            if(hitted == owner) continue;
            hitted.OnHit(DamageType, owner, MaxDamage, hitted.transform.position, Vector3.zero, 0f);
        }
    }

    public void StartWorking()
    {
        isMove = true;
        cachedSpawnTime = Time.time;
        cachedTimeInterval = Time.time;
        objFireBall.SetActive(true);
    }

    private void SpawnExplosion()
    {
        GameObject fromPool = PoolManager.Instance.GetFromPool(ExplosionPrefab);
        fromPool.transform.position = base.transform.position;
        Explosion component = fromPool.GetComponent<Explosion>();
        component.Init(owner, MaxDamage, ExplosionRange*1.5f, 150f);
    }

    public override void DeInit()
    {
        base.DeInit();
        isMove = false;
        explosionFx.Play();
        SpawnExplosion();
        objFireBall.SetActive(false);
        PoolManager.Instance.ReturnToPoolWithDelay(gameObject,1.35f);
    }
}
