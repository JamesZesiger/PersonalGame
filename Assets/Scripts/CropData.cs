using UnityEngine;

[CreateAssetMenu(menuName = "Farming/Crop Data")]
public class CropData : ScriptableObject
{
    public string cropName;

    [Header("Growth")]
    public float[] growthTimes; // time per stage

    [Header("Visuals")]
    public GameObject[] stagePrefabs; // one per stage
}