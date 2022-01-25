using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI count;
    [SerializeField] TextMeshProUGUI[] costs;
    [SerializeField] GameObject shopUI;
    #region singleton
    public static ShopUIController instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion


    public void ChangeShopUI(bool state)
    {
        shopUI.SetActive(state);
    }


    public void changeCost(int target, int amount)
    {
        costs[target].text = amount.ToString();
    }

    public void UpdateCoinCount(int amount)
    {
        count.text = amount.ToString();
    }

   
}





