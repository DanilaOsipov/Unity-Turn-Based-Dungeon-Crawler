using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : UIElement
{
    protected Slider slider;

    public int MaxHealth
    {
        get
        {
            return MaxHealth;
        }
        set
        {
            slider.maxValue = value;
        }
    }

    public int Health
    {
        get
        {
            return Health;
        }
        set
        {
            slider.value = value;
        }
    }

    private void Awake()
    {
        slider = GetComponent<Slider>();    
    }
}
