using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElement : MonoBehaviour
{
    public virtual bool IsVisible => isActiveAndEnabled; 

    public virtual void Show()
    {
        gameObject.SetActive(true); 
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
