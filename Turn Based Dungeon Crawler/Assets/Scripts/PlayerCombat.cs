﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int damage = 5;

    private PlayerSound playerSound;
    private CameraBob cameraBob;
    private CameraShake cameraShake;

    private int health;

    private void Start()
    {
        health = maxHealth;

        playerSound = GetComponentInChildren<PlayerSound>();
        cameraBob = GetComponentInChildren<CameraBob>();
        cameraShake = GetComponentInChildren<CameraShake>();

        UIController.HealthBar.MaxHealth = maxHealth;
        UIController.HealthBar.Health = health;
    }

    public void Attack(Action callback)
    {
        if (CanAttack(out EnemyCombat enemyCombat))
        {
            enemyCombat.TakeDamage(damage);
        }
        else callback();
    }

    public void Heal(int hp)
    {
        health += hp;

        if (health > maxHealth)
            health = maxHealth;

        UIController.HealthBar.Health = health;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        UIController.HealthBar.Health = health;

        cameraBob.ResetSpeed();

        if (health <= 0)
        {
            Die();
        }
        else
        {
            playerSound.Hit(() => { });

            Vector2 direction = UnityEngine.Random.insideUnitCircle;
            cameraShake.ShakeRotateCamera(direction, () =>
            {
                cameraBob.SetIdleSpeed();

                Messenger.Broadcast(GameEvent.PLAYER_TURN);
            });
        }
    }

    private void Die()
    {
        playerSound.Die(() => { UIController.DeathPopup.Show(); });

        Vector2 direction = Vector2.up;
        cameraShake.ShakeRotateCamera(direction, () => { }, false);
    }

    public bool CanAttack()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, DungeonGenerator.MovementOffset))
        {
            return hit.transform.GetComponent<EnemyCombat>() != null;
        }

        return false;
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
