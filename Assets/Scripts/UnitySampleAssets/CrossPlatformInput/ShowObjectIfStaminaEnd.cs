using System;
using System.Collections;
using System.Collections.Generic;
using Game.Character.CharacterController;
using Game.Character.Stats;
using UnityEngine;

public class ShowObjectIfStaminaEnd : MonoBehaviour
{
    public GameObject objToShow;
    public float amountToShow;

    private void Awake()
    {

        var player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.stats.stamina.OnValueChange += OnStatChange;
        }
    }
    private void OnDestroy()
    {
        var player = FindObjectOfType<Player>();
        if (player != null)
            player.stats.stamina.OnValueChange -= OnStatChange;
    }

    private void OnStatChange(float currentAmountOfStat)
    {
        if (currentAmountOfStat < amountToShow && !objToShow.activeSelf)
            objToShow.SetActive(true);
        if (currentAmountOfStat >= amountToShow && objToShow.activeSelf)
            objToShow.SetActive(false);
    }

}
