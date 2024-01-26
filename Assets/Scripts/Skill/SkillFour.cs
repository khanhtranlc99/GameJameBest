using System;
using System.Collections;
using System.Collections.Generic;
using Game.Character;
using Game.GlobalComponent;
using UnityEngine;

public class SkillFour : BaseSkills
{
    [SerializeField] private GameObject damageBallObj;
    [SerializeField] private float timelineStartFireball = .3f;
    private bool isSpawnBall;


    public override void StartSkill(Action onDoneAction)
    {
        base.StartSkill(onDoneAction);
        isSpawnBall = false;
    }

    protected override void UpdateSkill(float timeLine)
    {
        base.UpdateSkill(timeLine);
        if (timeLine > timelineStartFireball && !isSpawnBall)
        {
            isSpawnBall = true;
            GameObject fromPool = PoolManager.Instance.GetFromPool(damageBallObj);
            var damageBall = fromPool.GetComponent<DamageBall>();
            var player = PlayerInteractionsManager.Instance.Player;
            damageBall.Init(player);
            var transform1 = damageBall.transform;
            Vector3 playerPs = player.transform.position;
            playerPs.y += 2f;
            transform1.position = playerPs;
            transform1.forward = player.transform.forward;
            damageBall.StartWorking();
        }

    }
}
