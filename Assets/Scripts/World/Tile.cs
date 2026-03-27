using UnityEngine;

public class Tile
{
    public bool active;
    public GameObject instance;

    public TileType type = TileType.Empty;

    // Crop data
    public CropInstance crop;
    public float cropTimer;
    public GameObject cropInstance;
}