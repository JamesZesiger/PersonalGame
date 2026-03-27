using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int size = 20;
    public List<InventorySlot> items = new List<InventorySlot>();

    public void AddItem(Item item)
    {
        // Try to fill existing stacks first
        foreach (var slot in items)
        {
            if (slot.item == item && item.isStackable && slot.quantity < item.maxStack)
            {
                slot.quantity++;
                return;
            }
        }

        // Add new stack if space
        if (items.Count >= size)
        {
            return;
        }

        items.Add(new InventorySlot(item, 1));
    }
}