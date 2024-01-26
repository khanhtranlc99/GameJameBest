using System;
using System.Collections;
using System.Collections.Generic;
using Game.Character;
using Game.Effects;
using Game.GlobalComponent;
using UnityEngine;

public class SkillTwo : BaseSkills
{
    [SerializeField] private IceWall objFX;
    [SerializeField] private float timelineStartFireball = .3f;
    private bool isSpawnBall;
    [SerializeField] private GameObject ExplosionPrefab;

    public override void StartSkill(Action onDoneAction)
    {
        base.StartSkill(onDoneAction);
        objFX.gameObject.SetActive(false);
        isSpawnBall = false;
    }
    

    protected override void UpdateSkill(float timeLine)
    {
        base.UpdateSkill(timeLine);
        if (timeLine > timelineStartFireball && !isSpawnBall)
        {
            isSpawnBall = true;
            objFX.gameObject.SetActive(true);
            objFX.StartWorking();
            DealDamageAround();
            //var damageBall = fromPool.GetComponent<DamageBall>();
            var player = PlayerInteractionsManager.Instance.Player;
            //damageBall.Init(player);
            var transform1 = objFX.transform;
            Vector3 playerPs = player.transform.position;
            transform1.position = playerPs;
            transform1.forward = player.transform.forward;
            //damageBall.StartWorking();
        }
    }

    private void DealDamageAround()
    {
        GameObject fromPool = PoolManager.Instance.GetFromPool(ExplosionPrefab);

        Explosion component = fromPool.GetComponent<Explosion>();
        var player = PlayerInteractionsManager.Instance.Player;
        fromPool.transform.position = player.transform.position;
        var playerIgnor = new GameObject[1];
        playerIgnor[0] = player.gameObject;
	    component.Init(player, 10000f, 35, 8,playerIgnor);
    }
}
