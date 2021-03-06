﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public static List<Enemy> Enemies { get; private set; }
    public static bool BattleInProcess { get; private set; }

    private bool enemyTurn;

    private void Start()
    {
        Messenger.AddListener(GameEvent.ENEMY_TURN, OnEnemyTurn);

        Enemies = new List<Enemy>();
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.ENEMY_TURN, OnEnemyTurn);
    }

    private Enemy GetClosestEnemy()
    {
        Enemy enemy = Enemies[0];

        if (Enemies.Count == 1)
            return enemy;

        float minDistance = Vector3.Distance(enemy.transform.position, enemy.Movement.target.transform.position);

        for (int i = 1; i < Enemies.Count; i++)
        {
            float distance = Vector3.Distance(Enemies[i].transform.position, Enemies[i].Movement.target.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                enemy = Enemies[i];
            }
        }

        return enemy;
    }

    private void OnEnemyTurn()
    {
        Debug.Log("Enemy turn.");

        enemyTurn = true;
    }

    private void Update()
    {
        if (Enemies.Count > 0)
        {
            if (!BattleInProcess)
                BattleInProcess = true;

            if (enemyTurn)
            {
                enemyTurn = false;

                Enemy enemy = GetClosestEnemy();

                if (enemy.Combat.CanAttack())
                {
                    enemy.Combat.Attack();
                }
                else enemy.Movement.Chase();
            }
        }
        else
        {
            if (BattleInProcess)
            {
                BattleInProcess = false;

                Messenger.Broadcast(GameEvent.PLAYER_TURN);
            }
        }
    }
}
