using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] AudioClip mainMenuTrack;

    AudioStation audioStation;

    void Start()
    {
        audioStation = AudioStation.Instance;
        audioStation.StartNewMusicPlayer(mainMenuTrack, true);
    }


}