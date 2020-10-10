using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvent : MonoBehaviour
{
    private PlayerSound playerSound;
    private EnemySound enemySound;
    private PlayerCombat playerCombat;

    private void Start()
    {
        Transform cam = Camera.main.transform;

        playerSound = cam.GetComponent<PlayerSound>();
        playerCombat = cam.GetComponentInParent<PlayerCombat>();

        enemySound = GetComponentInParent<EnemySound>();
    }

    private void Step()
    {
        enemySound.Step(() => { });
    }

    private void Hit()
    {
        playerSound.Attack(() => { });
    }

    private void Die()
    {
        enemySound.Die(() => { });
    }

    private void PlayerReact()
    {
        playerCombat.TakeDamage(EnemyCombat.Damage);

    }

    private void Sword()
    {
        enemySound.Sword(() => { });
    }
}
