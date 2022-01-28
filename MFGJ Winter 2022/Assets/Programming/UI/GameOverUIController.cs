using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI WavesText;
    [SerializeField] TextMeshProUGUI coinsText;
    [SerializeField] GameObject deathUI;
    [SerializeField] GameObject[] UIToClose;


    // Start is called before the first frame update
    void Start()
    {
        AliveCheck.DeathTrigger += startDeath;
    }

    void startDeath()
    {
        deathUI.SetActive(true);
        foreach (var item in UIToClose)
        {
            item.SetActive(false);
        }
        var player = GameObject.FindGameObjectWithTag("Player");
        for (int i = 0; i < player.transform.childCount; i++)
        {

            if (!player.transform.GetChild(i).TryGetComponent<Camera>(out Camera cam))
            {
                player.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        int waves = EnemySpawnController.s.wave;
        int coins = ShopCurrencyController.instance.coins;
        if(waves == 1)
        {
            WavesText.text = "You survived for " + waves + " wave.";
        }
        else
        {
            WavesText.text = "You survived for " + waves + " waves.";
        }
        if(coins == 1)
        {
            coinsText.text = "You died with " + coins + " coin left.";
        }
        else
        {
            coinsText.text = "You died with " + coins + " coins left.";
        }
        


    }
}



