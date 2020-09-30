using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int damage = 5;

    private int health;

    private void Start()
    {
        health = maxHealth;
    }

    public void Attack(Action callback)
    {
        if (CanAttack(out EnemyCombat enemyCombat))
        {
            Debug.Log("player atk");

            enemyCombat.TakeDamage(damage);
        }
        else callback();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        Debug.Log("player health " + health);

        if (health <= 0)
        {
            Die();
        }
        else Messenger.Broadcast(GameEvent.PLAYER_TURN);
    }

    private void Die()
    {
        //Destroy(gameObject);

        health = maxHealth;
    }

    private bool CanAttack(out EnemyCombat enemyCombat)
    {
        enemyCombat = null;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, DungeonGenerator.MovementOffset))
        {
            enemyCombat = hit.transform.GetComponent<EnemyCombat>();

            return enemyCombat != null;
        }

        return false;
    }
}
