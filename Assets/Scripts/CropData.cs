using UnityEngine;

[CreateAssetMenu(menuName = "Farming/Crop Data")]
public class CropData : ScriptableObject
{
    public string cropName;

    [Header("Prefabs")]
    public GameObject seedPrefab;
    public GameObject growingPrefab;
    public GameObject readyPrefab;
    public Item item;

    [Header("Timing")]
    public float seedToGrowingTime = 5f;
    public float growingToReadyTime = 10f;

    [Header("Behavior")]
    public bool regrowable = false;
    public float regrowTime = 8f; // ready → ready again after harvest
    public GameObject[] GetAllPrefabs()
    {
        return new GameObject[]
        {
            seedPrefab,
            growingPrefab,
            readyPrefab
        };
    }
}
public enum CropState
{
    Seed,
    Growing,
    ReGrowing,
    Ready
}