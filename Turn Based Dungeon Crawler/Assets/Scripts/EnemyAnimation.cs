using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] private float startWalkingSpeed = 6.0f;
    [SerializeField] private float stopWalkingSpeed = 6.0f;

    private Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void StartWalking(Action callback)
    {
        StartCoroutine(StartWalkingInternal(callback));
    }

    private IEnumerator StartWalkingInternal(Action callback)
    {
        for (float t = 0; t < 1; t += startWalkingSpeed * Time.deltaTime)
        {
            animator.SetFloat("speed", t);

            yield return null;
        }

        animator.SetFloat("speed", 1.0f);

        callback();
    }

    public void TakeDamage(Action callback)
    {
        StartCoroutine(TakeDamageInternal(callback));
    }

    private IEnumerator TakeDamageInternal(Action callback)
    {
        animator.SetTrigger("hit");

        yield return null;

        float length = animator.GetNextAnimatorClipInfo(0)[0].clip.length;

        yield return new WaitForSeconds(length);

        callback();
    }

    public void Die(Action callback)
    {
        StartCoroutine(DieInternal(callback));
    }

    private IEnumerator DieInternal(Action callback)
    {
        animator.SetTrigger("die");

        yield return null;

        float length = animator.GetNextAnimatorClipInfo(0)[0].clip.length;

        yield return new WaitForSeconds(length);

        callback();
    }

    public void Attack(Action callback)
    {
        StartCoroutine(AttackInternal(callback));
    }

    private IEnumerator AttackInternal(Action callback)
    {
        animator.SetTrigger("atk");

        yield return null;

        float length = animator.GetNextAnimatorClipInfo(0)[0].clip.length;

        yield return new WaitForSeconds(length);

        callback();
    }

    public void StopWalking(Action callback)
    {
        StartCoroutine(StopWalkingInternal(callback));
    }

    private IEnumerator StopWalkingInternal(Action callback)
    {
        for (float t = 1; t >= 0; t -= stopWalkingSpeed * Time.deltaTime)
        {
            animator.SetFloat("speed", t);

            yield return null;
        }

        animator.SetFloat("speed", 0);

        callback();
    }
}
