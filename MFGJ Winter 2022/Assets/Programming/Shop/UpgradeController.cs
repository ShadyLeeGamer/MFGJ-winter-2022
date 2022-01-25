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


    private void Start()
    {
        shop = ShopCurrencyController.instance;
        ui = ShopUIController.instance;
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
        storeActive = true;
        ui.ChangeShopUI(storeActive);
    }

    void CloseStore()
    {
        storeActive = false;
        ui.ChangeShopUI(storeActive);
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
