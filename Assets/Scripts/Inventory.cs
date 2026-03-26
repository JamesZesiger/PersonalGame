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
                Debug.Log($"Stacked {item.name}, now {slot.quantity}");
                return;
            }
        }

        // Add new stack if space
        if (items.Count >= size)
        {
            Debug.Log("Inventory full");
            return;
        }

        items.Add(new InventorySlot(item, 1));
        Debug.Log($"{item.name} added as new stack");
    }
}