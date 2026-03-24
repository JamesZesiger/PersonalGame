using UnityEngine;

public class CropInstance
{
    public CropData data;
    public int growthStage = 0;
    public float timer = 0f;

    public GameObject visual;

    public CropInstance(CropData data)
    {
        this.data = data;
    }

    public bool IsFullyGrown()
    {
        return growthStage >= data.stagePrefabs.Length - 1;
    }
}