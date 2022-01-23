using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BasicUIFunctions : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadSceneAtIndex(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void ActivateGameObject(GameObject target)
    {
        target.SetActive(true);
    }

    public void DisableGameObject(GameObject target)
    {
        target.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
