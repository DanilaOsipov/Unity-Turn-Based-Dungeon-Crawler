﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private int damage = 5;

    private EnemyAnimation enemyAnimation;

    private int health;

    private void Start()
    {
        health = maxHealth;

        enemyAnimation = GetComponent<EnemyAnimation>();
    }

    public void Attack()
    {
        if (CanAttack(out PlayerCombat playerCombat))
        {
            Debug.Log("enemy atk");

            enemyAnimation.Attack(() => { playerCombat.TakeDamage(damage); });
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        Debug.Log("enemy hp " + health);

        if (health <= 0)
        {
            enemyAnimation.Die(() => { Die(); });
        }
        else
        {
            enemyAnimation.TakeDamage(() => { Messenger.Broadcast(GameEvent.ENEMY_TURN); });
        }
    }

    private void Die()
    {
        Pathfinding.Instance.RemoveObjectFromMap(transform);

        DungeonGenerator.EnemiesCount--;

        Debug.Log(DungeonGenerator.EnemiesCount);

        //Debug.Log(BattleSystem.Enemies.Count);

        Enemy enemy = GetComponent<Enemy>();
        BattleSystem.Enemies.Remove(enemy);

        Destroy(gameObject);

        Messenger.Broadcast(GameEvent.ENEMY_TURN);
    }

    public bool CanAttack()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, DungeonGenerator.MovementOffset))
        {
             return hit.transform.GetComponent<PlayerCombat>() != null;
        }

        return false;
    }

    private bool CanAttack(out PlayerCombat playerCombat)
    {
        playerCombat = null;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, DungeonGenerator.MovementOffset))
        {
            playerCombat = hit.transform.GetComponent<PlayerCombat>();

            return playerCombat != null;
        }

        return false;
    }
}
