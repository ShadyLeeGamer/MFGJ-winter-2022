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
        cutscenePlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "introcutscene.mp4");
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