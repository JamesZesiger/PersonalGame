using UnityEngine;
public class CropInstance
{
    public CropData data;
    public CropProgressUI progressUI;
    public CropState state = CropState.Seed;
    public float timer = 0f;

    public GameObject visual;
    public GameObject sourcePrefab;

    public CropInstance(CropData data)
    {
        this.data = data;
    }

    public bool IsReady()
    {
        return state == CropState.Ready;
    }
}