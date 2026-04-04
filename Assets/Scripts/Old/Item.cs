using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public bool isStackable;
    public int maxStack = 99;
    public Sprite icon;
    public int baseValue;
}