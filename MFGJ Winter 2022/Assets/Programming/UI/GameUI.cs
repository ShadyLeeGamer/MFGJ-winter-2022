using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] GameObject gameUI, pauseMenu;
    bool isPaused;

    AudioStation audioStation;

    void Start()
    {
        audioStation = AudioStation.Instance;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
        audioStation.SetAllPlayerPause(isPaused);
    }
}