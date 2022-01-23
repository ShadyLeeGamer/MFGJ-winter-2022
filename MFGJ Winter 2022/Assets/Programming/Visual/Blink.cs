using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    [SerializeField] float blinkPsec;
    [SerializeField] GameObject target;
    float timer;
    private void Start()
    {
        timer = 1 / blinkPsec;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            timer = 1 / blinkPsec;
            target.SetActive(!target.activeSelf);
        }
    }
}
