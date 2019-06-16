using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}