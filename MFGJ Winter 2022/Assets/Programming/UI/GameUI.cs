using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField] GameObject gameUI, pauseMenu;
    bool isPaused;

    AudioStation audioStation;

    [SerializeField] Slider cropsRemainingBar, cropEatersRemainingBar, playerStaminaBar;
    [SerializeField] TextMeshProUGUI currentWaveDisplay;

    public static GameUI Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        audioStation = AudioStation.Instance;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        gameUI.SetActive(!isPaused);
        Time.timeScale = isPaused ? 0 : 1;
        audioStation.SetAllPlayerPause(isPaused);
    }

    public void SetCropsRemainingBar(float value, float maxValue)
    {
        cropsRemainingBar.maxValue = maxValue;
        cropsRemainingBar.value = value;
    }

    public void SetCropEatersRemainingBar(float value, float maxValue)
    {
        cropEatersRemainingBar.maxValue = maxValue;
        cropEatersRemainingBar.value = value;
    }

    public void SetPlayerStaminaBar(float value, float maxValue)
    {
        playerStaminaBar.value = value;
        playerStaminaBar.maxValue = maxValue;
    }

    public void SetCurrentWaveDisplay(float value)
    {
        currentWaveDisplay.text = "Wave " + value;
    }
}