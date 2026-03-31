using UnityEngine;

public class Tile
{
    public bool active;
    public GameObject instance;
    public GameObject sourcePrefab;
    public TileType type = TileType.Empty;

    // Crop data
    public CropInstance crop;
    public float cropTimer;
    public GameObject cropInstance;

    public bool isWatered;
    
    public float waterTimer = 0f;

    public TileSet tileSet;
    public StructureSet structureSet;

    public int? structureIndex = 0;
}