using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour
{
    VideoPlayer cutscenePlayer;

    private void Awake()
    {
        cutscenePlayer = GetComponent<VideoPlayer>();
    }

    void Start()
    {
        StartCoroutine(CutsceneTransitionToGame());
    }

    IEnumerator CutsceneTransitionToGame()
    {
        yield return new WaitForSeconds(1);

        while (cutscenePlayer.isPlaying)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(3);
    }
}