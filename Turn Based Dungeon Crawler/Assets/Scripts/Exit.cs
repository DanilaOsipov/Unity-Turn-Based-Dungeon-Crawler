using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    [SerializeField] private string sceneName;

    public void ExitLevel()
    {
        SceneManager.LoadScene(sceneName);
    }
}
