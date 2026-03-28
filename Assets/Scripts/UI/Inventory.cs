using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int size = 20;
    public List<InventorySlot> items = new List<InventorySlot>();

    public int AddItem(Item item, int amount = 1)
    {
        if (item == null) return amount;

        int remaining = amount;

        // 1. Fill existing stacks
        if (item.isStackable)
        {
            foreach (var slot in items)
            {
                if (slot.item == item && slot.quantity < item.maxStack)
                {
                    int space = item.maxStack - slot.quantity;
                    int toAdd = Mathf.Min(space, remaining);

                    slot.quantity += toAdd;
                    remaining -= toAdd;

                    if (remaining <= 0)
                        return 0;
                }
            }
        }

        // 2. Create new stacks
        while (remaining > 0 && items.Count < size)
        {
            int toAdd = item.isStackable
                ? Mathf.Min(item.maxStack, remaining)
                : 1;

            items.Add(new InventorySlot(item, toAdd));
            remaining -= toAdd;
        }

        return remaining; // leftover if inventory is full
    
    
    }

    public int TransferTo(Inventory target, InventorySlot slot)
    {
        if (slot == null || slot.item == null) return 0;
        
        int amount = slot.quantity;
        int leftover = target.AddItem(slot.item, amount);
        int moved = amount - leftover;
        slot.quantity -= moved;
        if (slot.quantity <= 0)
        {
            items.Remove(slot);
        }

        return moved;
    }
}