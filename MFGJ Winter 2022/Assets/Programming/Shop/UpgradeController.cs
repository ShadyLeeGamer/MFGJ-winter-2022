using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeController : MonoBehaviour
{
    bool storeActive;

    ShopCurrencyController shop;
    ShopUIController ui;

    [Header("Recover values")]
    [SerializeField] AnimationCurve costPerRevive;
    [SerializeField] int reviveUIInt;
    int reviveLevel;
    KeyCode reviveInput = KeyCode.Z;
    [SerializeField] AudioClip[] gfSFX;
    [SerializeField] AudioClip gfSFXRare;
    AudioStation audioStation;

    bool shopIsOpen;

    private void Start()
    {
        shop = ShopCurrencyController.instance;
        ui = ShopUIController.instance;
        audioStation = AudioStation.Instance;
    }


    private void Update()
    {
        if (storeActive)
        {
            
            if (Input.GetKeyDown(reviveInput))
            {
                RevivePlant();
            }
        }
    }


    
    void RevivePlant()
    {
        if (EnemySpawnController.s.CheckForDeadPlants())
        {

            int cost = Mathf.FloorToInt(costPerRevive.Evaluate(reviveLevel));

            if (shop.purchase(cost))
            {

                reviveLevel++;
                ui.changeCost(reviveUIInt, Mathf.FloorToInt(costPerRevive.Evaluate(reviveLevel)));
                EnemySpawnController.s.RevivePlant();
            }
        }
    }




    #region opening and closing store
    void OpenStore()
    {
        if (shopIsOpen)
            return;

        storeActive = true;
        ui.ChangeShopUI(storeActive);
        if (Random.value < 0.1)
            audioStation.StartNewSFXPlayer(gfSFXRare, default, null, 1, 1, true);
        else
            audioStation.StartNewRandomSFXPlayer(gfSFX, default, null, 1, 1, true);
        shopIsOpen = true;
    }

    void CloseStore()
    {
        storeActive = false;
        ui.ChangeShopUI(storeActive);
        shopIsOpen = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OpenStore();
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CloseStore();
        }
    }

    #endregion
}
