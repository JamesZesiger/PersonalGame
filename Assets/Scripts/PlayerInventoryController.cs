using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    public Inventory playerInventory;
    public InventoryUI inventoryUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryUI.Init(playerInventory);
            inventoryUI.Toggle();
        }
    }
}