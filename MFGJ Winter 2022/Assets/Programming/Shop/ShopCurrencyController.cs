using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCurrencyController : MonoBehaviour
{
    public int coins { get; private set; }


    ShopUIController ui;
    #region singleton
    public static ShopCurrencyController instance;
    private void Awake()
    {
        
        instance = this;
    }

    #endregion

    private void Start()
    {
        ui = ShopUIController.instance;
        ui.UpdateCoinCount(coins);
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        ui.UpdateCoinCount(coins);
    }

    public bool purchase(int cost)
    {
       if(coins >= cost)
        {
            coins -= cost;
            ui.UpdateCoinCount(coins);
            return true;
        }
        else
        {
            return false;
        }
    }
}