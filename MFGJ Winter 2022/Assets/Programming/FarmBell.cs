using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmBell : MonoBehaviour
{
    [SerializeField] AudioClip ringSFX;
    [SerializeField] GameObject text;

    Animator animator;

    AudioStation audioStation;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        audioStation = AudioStation.Instance;
    }

    public void Ring()
    {
        if (EnemySpawnController.s.waveOver == true)
        {
            EnemySpawnController.s.waveOver = false;
            animator.Play("Farm Bell Ring");
            audioStation.StartNewSFXPlayer(ringSFX, default, null, 1, 1, true);
            SetTextActive(false);
        }
    }

    public void SetTextActive(bool value)
    {
        text.SetActive(value);
    }
}