using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI count;

    #region singleton
    public static ShopUIController instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion



    public void UpdateCoinCount(int amount)
    {
        count.text = amount.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
