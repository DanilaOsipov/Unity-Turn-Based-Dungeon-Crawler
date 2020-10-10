using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] hitSounds;
    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip[] stepSounds;
    [SerializeField] private AudioClip[] deathSounds;

    private AudioSource soundSource;

    private void Start()
    {
        soundSource = GetComponent<AudioSource>();
    }

    public void Die(System.Action callback)
    {
        AudioClip clip = deathSounds[Random.Range(0, deathSounds.Length)];

        StartCoroutine(PlaySound(callback, clip));
    }

    public void Step(System.Action callback)
    {
        AudioClip clip = stepSounds[Random.Range(0, stepSounds.Length)];

        StartCoroutine(PlaySound(callback, clip));
    }

    public void Hit(System.Action callback)
    {
        AudioClip clip = hitSounds[Random.Range(0, hitSounds.Length)];

        StartCoroutine(PlaySound(callback, clip));
    }

    public void Attack(System.Action callback)
    {
        AudioClip clip = attackSounds[Random.Range(0, attackSounds.Length)];

        StartCoroutine(PlaySound(callback, clip));
    }

    private IEnumerator PlaySound(System.Action callback, AudioClip clip)
    {
        soundSource.PlayOneShot(clip);

        yield return new WaitForSeconds(clip.length);

        callback();
    }
}
