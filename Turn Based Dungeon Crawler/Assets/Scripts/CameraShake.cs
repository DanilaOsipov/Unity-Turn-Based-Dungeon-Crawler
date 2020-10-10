using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float speed = 8.0f;

    private Quaternion startRotation;

    private void Start()
    {
        startRotation = transform.localRotation;
    }

    public void ShakeRotateCamera(Vector2 direction, Action callback, bool shakeAndReturn = true)
    {
        StartCoroutine(ShakeRotateCameraInternal(direction, callback, shakeAndReturn));
    }

    private IEnumerator ShakeRotateCameraInternal(Vector2 direction, Action callback, bool shakeAndReturn)
    {
        direction = direction.normalized;
        Vector3 resDirection = ((Vector3)direction + Vector3.forward).normalized;
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.forward, resDirection);

        for (float t = 0; t < 1; t += speed * Time.deltaTime)
        {
            transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);

            yield return null;
        }

        if (shakeAndReturn)
        {
            for (float t = 0; t < 1; t += speed * Time.deltaTime)
            {
                transform.localRotation = Quaternion.Lerp(targetRotation, startRotation, t);

                yield return null;
            }

            yield return null;

            transform.localRotation = startRotation;
        }

        callback();
    }
}
