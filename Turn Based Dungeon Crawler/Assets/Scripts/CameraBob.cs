using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBob : MonoBehaviour
{
    [SerializeField] private float bobbingAmount = 0.05f;
    [SerializeField] private float walkingSpeed = 20.0f;
    [SerializeField] private float idleSpeed = 2.0f;

    private float timer = 0;
    private float speed;

    private float DefaultY { get; set; }

    private void Start()
    {
        DefaultY = transform.localPosition.y;

        SetIdleSpeed();
    }

    private void Update()
    {
        if (speed != 0)
        {
            timer += Time.deltaTime * speed;
            transform.localPosition = new Vector3(transform.localPosition.x, 
                                                  DefaultY + Mathf.Sin(timer) * bobbingAmount, 
                                                  transform.localPosition.z);
        }
        else
        {
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, 
                                                  Mathf.Lerp(transform.localPosition.y, DefaultY, Time.deltaTime * speed), 
                                                  transform.localPosition.z);
        }
    }

    public void ResetSpeed()
    {
        speed = 0;
    }

    public void SetIdleSpeed()
    {
        speed = idleSpeed;
    }

    public void SetWalkingSpeed()
    {
        speed = walkingSpeed;
    }
}
