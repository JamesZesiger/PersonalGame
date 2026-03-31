using UnityEngine;
[System.Serializable]
public class HammerSlot
{
    public int index;
    public Sprite sprite;

    public HammerSlot(int index, Sprite sprite)
    {
        this.index = index;
        this.sprite = sprite;
    }
}