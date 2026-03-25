using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int size = 20;
    public List<Item> items = new List<Item>();

    public void AddItem(Item item)
    {
        if (items.Count >= size) return;

        items.Add(item);
    }
}