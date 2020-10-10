using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : HealthBar
{
    private Transform cam;

    private void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }

    private void Awake()
    {
        slider = GetComponent<Slider>();

        cam = Camera.main.transform;
    }
}
