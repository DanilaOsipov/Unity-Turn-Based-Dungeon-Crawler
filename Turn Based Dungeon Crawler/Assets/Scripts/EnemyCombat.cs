using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private int damage = 5;

    private EnemyAnimation enemyAnimation;
    private EnemySound enemySound;

    private int health;

    private EnemyHealthBar healthBar;

    public static int Damage { get; private set; }

    private void Start()
    {
        health = maxHealth;

        enemyAnimation = GetComponent<EnemyAnimation>();

        enemySound = GetComponent<EnemySound>();

        healthBar = GetComponentInChildren<EnemyHealthBar>();
        healthBar.MaxHealth = maxHealth;
        healthBar.Health = health;

        Damage = damage;
    }

    public void Attack()
    {
        if (CanAttack(out PlayerCombat playerCombat))
        {
            enemySound.AttackMoan(() => { });

            enemyAnimation.Attack(() => { });
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        healthBar.Health = health;

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
