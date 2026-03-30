using UnityEngine;
[System.Serializable]
public class InventorySlot
{
    public Item item;
    public int quantity;
    public int value;

    public InventorySlot(Item item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
        this.value = this.item.baseValue * this.quantity;
    }
}