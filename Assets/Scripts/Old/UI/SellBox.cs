using UnityEngine;
using System.Collections.Generic;


public class SellBox : Container
{
    public Wallet wallet;
    private bool isActive;
    private int value;
    void Awake()
    {
        isActive = uiManager.containerUI.gameObject.activeSelf;
    }
    void Update()
    {
        if (isActive)
        {
            isActive = uiManager.containerUI.gameObject.activeSelf;
            if (!isActive)
            {
                value = 0;
                for (int i = 0; i < inventory.items.Count; i++)
                {
                    value+= inventory.items[i].value;
                }
                wallet.UpdateWallet(value);
                value = 0;
                inventory.items = new List<InventorySlot>();

            }
        }
        if (!isActive)
            isActive = uiManager.containerUI.gameObject.activeSelf;

    }
}
