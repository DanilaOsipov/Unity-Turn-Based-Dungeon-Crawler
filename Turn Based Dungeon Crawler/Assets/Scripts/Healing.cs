using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : MonoBehaviour
{
    [SerializeField] private int healingPoints = 25;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();  
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerCombat playerCombat = other.GetComponent<PlayerCombat>();

        if (playerCombat != null)
        {
            StartCoroutine(Heal(playerCombat));
        }
    }

    private IEnumerator Heal(PlayerCombat playerCombat)
    {
        audioSource.Play();

        playerCombat.Heal(healingPoints);

        yield return new WaitForSeconds(audioSource.clip.length);

        Destroy(gameObject);
    }
}
