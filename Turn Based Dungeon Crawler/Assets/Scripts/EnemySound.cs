using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySound : MonoBehaviour
{
    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip[] swordSounds;
    [SerializeField] private AudioClip[] dieSounds;
    [SerializeField] private AudioClip[] stepSounds;

    private AudioSource soundSource;

    private void Start()
    {
        soundSource = GetComponent<AudioSource>();
    }

    public void Step(System.Action callback)
    {
        AudioClip clip = stepSounds[Random.Range(0, stepSounds.Length)];

        StartCoroutine(PlaySound(callback, clip));
    }

    public void Die(System.Action callback)
    {
        AudioClip clip = dieSounds[Random.Range(0, dieSounds.Length)];

        StartCoroutine(PlaySound(callback, clip));
    }

    public void AttackMoan(System.Action callback)
    {
        AudioClip clip = attackSounds[Random.Range(0, attackSounds.Length)];

        StartCoroutine(PlaySound(callback, clip));
    }

    public void Sword(System.Action callback)
    {
        AudioClip clip = swordSounds[Random.Range(0, swordSounds.Length)];

        StartCoroutine(PlaySound(callback, clip));
    }

    private IEnumerator PlaySound(System.Action callback, AudioClip clip)
    {
        soundSource.PlayOneShot(clip);

        yield return new WaitForSeconds(clip.length);

        callback();
    }
}
