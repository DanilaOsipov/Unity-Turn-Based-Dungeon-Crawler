using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : MonoBehaviour
{
    [SerializeField] private int healingPoints = 25;

    private void OnTriggerEnter(Collider other)
    {
        PlayerCombat playerCombat = other.GetComponent<PlayerCombat>();

        if (playerCombat != null)
        {
            playerCombat.Heal(healingPoints);

            Destroy(gameObject);
        }
    }
}
