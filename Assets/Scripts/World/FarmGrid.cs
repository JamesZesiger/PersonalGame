using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class FarmGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 50;
    public int height = 50;
    public float tileSize = 1f;

    [Header("Tile Sets (per tool)")]
    public TileSet hoeTileSet;
    public TileSet hammerTileSet;

    [Header("References")]
    public Transform tileParent;
    public Vector3 originPosition;
    public GameObject progressUIPrefab;
    public InventoryUI inventoryUI;
    public Transform player;
    public Camera cam;

    private Tile[,] tiles;
    private ToolType currentTool = ToolType.Hoe;
    private TileSet activeTileSet;


    // ================================
    // PREFAB POOLING
    // ================================
    private Dictionary<GameObject, Queue<GameObject>> pool = new Dictionary<GameObject, Queue<GameObject>>();


     void Start()
    {
        
        if (hoeTileSet != null) hoeTileSet.BuildTileLookup();
        if (hammerTileSet != null) hammerTileSet.BuildTileLookup();

        foreach (var cropData in Resources.LoadAll<CropData>(""))
        {
            foreach (var prefab in cropData.GetAllPrefabs())
            {
                if (prefab != null)
                    PrewarmPool(prefab, 20);
            }
        }

        foreach (var TileSet in Resources.LoadAll<TileSet>(""))
        {
            foreach (var prefab in TileSet.GetAllPrefabs())
            {
                if (prefab != null)
                    PrewarmPool(prefab, 20);
            }
        }
    }

    void Awake()
    {
        tiles = new Tile[width, height];

        activeTileSet = hoeTileSet;

        InitializeGrid();
    }

    void InitializeGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = new Tile();
            }
        }
    }

    void Update()
    {
        GrowCrops(Time.deltaTime);
    }

    // ================================
    // TOOL SWITCHING
    // ================================
    public void SetTool(ToolType tool)
    {
        currentTool = tool;
        activeTileSet = GetTileSetForTool(tool);
    }

    TileSet GetTileSetForTool(ToolType tool)
    {
        switch (tool)
        {
            case ToolType.Hoe: return hoeTileSet;
            case ToolType.Hammer: return hammerTileSet;
            default: return hoeTileSet;
        }
    }

    // ================================
    // PLACE TILE
    // ================================
    public void PlaceTile(int x, int y)
    {
        if (!InBounds(x, y)) return;

        tiles[x, y].active = true;
        UpdateTileAndNeighbors(x, y);
    }

    void UpdateTileAndNeighbors(int x, int y)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int nx = x + dx;
                int ny = y + dy;

                if (InBounds(nx, ny) && tiles[nx, ny].active)
                    UpdateTileVisual(nx, ny);
            }
        }
    }

    // ================================
    // VISUAL UPDATE WITH POOLING
    // ================================
    void UpdateTileVisual(int x, int y)
    {
        int mask = GetBitmask(x, y,activeTileSet);
        TileVisual visual = activeTileSet.GetTileVisual(mask);
        Tile tile = tiles[x, y];

        // Return previous instance to pool
        if (tile.instance != null)
        {
            ReturnToPool(tile.instance);
            tile.instance = null;
        }

        // Spawn new prefab from pool
        Vector3 pos = GridToWorld(x, y);
        pos.y -= 0.1f;
        tile.instance = SpawnFromPool(visual.prefab, pos, visual.rotation, tileParent);
    }

    // ================================
    // BITMASKING
    // ================================
    int GetBitmask(int x, int z,TileSet tileSet)
    {
        bool top = tileSet.type.Contains(GetTile(x, z + 1)?.type ?? TileType.Empty);
        bool right = tileSet.type.Contains(GetTile(x + 1, z)?.type ?? TileType.Empty);
        bool bottom = tileSet.type.Contains(GetTile(x, z - 1)?.type ?? TileType.Empty);
        bool left = tileSet.type.Contains(GetTile(x - 1, z)?.type ?? TileType.Empty);

        bool topRight = (tileSet.type.Contains(GetTile(x + 1, z + 1)?.type ?? TileType.Empty)) && top && right;
        bool bottomRight = (tileSet.type.Contains(GetTile(x + 1, z - 1)?.type ?? TileType.Empty)) && bottom && right;
        bool bottomLeft = (tileSet.type.Contains(GetTile(x - 1, z - 1)?.type ?? TileType.Empty)) && bottom && left;
        bool topLeft = (tileSet.type.Contains(GetTile(x - 1, z + 1)?.type ?? TileType.Empty)) && top && left;

        int mask = 0;
        if (top) mask |= 1;
        if (topRight) mask |= 2;
        if (right) mask |= 4;
        if (bottomRight) mask |= 8;
        if (bottom) mask |= 16;
        if (bottomLeft) mask |= 32;
        if (left) mask |= 64;
        if (topLeft) mask |= 128;
        return mask;
    }

    bool HasTile(int x, int y, TileType type)
    {
        return InBounds(x, y) && tiles[x, y].type == type;
    }

    bool InBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    // ================================
    // TILE TYPE MODIFIERS
    // ================================
    public void SetTileType(int x, int y, TileType type)
    {
        if (!InBounds(x, y)) return;

        Tile tile = GetTile(x, y);
        tile.type = type;
        tile.active = type != TileType.Empty;
        UpdateTileAndNeighbors(x, y);
    }

    public void TillTile(int x, int y)
    {
        if (!InBounds(x, y)) return;

        Tile tile = GetTile(x, y);

        if (tile.type == TileType.Empty)
        {
            tile.type = TileType.Tilled;
            tile.active = true;

            UpdateTileAndNeighbors(x, y);
        }
    }

    public void UntillTile(int x, int y)
    {
        if (!InBounds(x, y)) return;

        Tile tile = tiles[x, y];
        tile.type = TileType.Empty;
        tile.active = false;

        if (tile.instance != null) ReturnToPool(tile.instance);
        if (tile.cropInstance != null) ReturnToPool(tile.cropInstance);

        UpdateTileAndNeighbors(x, y);
    }

    // ================================
    // CROP SYSTEM
    // ================================
    public void PlantCrop(int x, int z, CropData cropData)
    {
        Tile tile = GetTile(x, z);

        if (tile == null || tile.type !=TileType.Tilled || tile.crop != null)
            return;

        CropInstance crop = new CropInstance(cropData);
        tile.crop = crop;
        tile.type = TileType.Planted;
        SpawnCropVisual(x, z);
    }

    void SpawnCropVisual(int x, int z)
    {
        Tile tile = GetTile(x, z);
        if (tile?.crop == null) return;

        CropInstance crop = tile.crop;

        // Return old
        if (crop.visual != null)
        {
            ReturnToPool(crop.visual);
            crop.visual = null;
            crop.sourcePrefab = null;
        }

        GameObject prefab = null;

        switch (crop.state)
        {
            case CropState.Seed:
                prefab = crop.data.seedPrefab;
                break;
            case CropState.Growing:
                prefab = crop.data.growingPrefab;
                break;
            case CropState.ReGrowing:
            prefab = crop.data.growingPrefab;
            break;
            case CropState.Ready:
                prefab = crop.data.readyPrefab;
                break;
        }

        Vector3 pos = GridToWorld(x, z);
        pos.y += 0.1f;

        GameObject obj = GetFromPool(prefab);
        obj.transform.SetPositionAndRotation(pos, Quaternion.identity);

        crop.visual = obj;
        crop.sourcePrefab = prefab;


        if (crop.state == CropState.Growing)
            obj.transform.localScale = Vector3.one * 0.5f;
        else
            obj.transform.localScale = Vector3.one;

        // After spawning crop.visual

        if (crop.progressUI != null)
        {
            ReturnToPoolPrefab(crop.progressUI.gameObject, progressUIPrefab);
            crop.progressUI = null;
        }

        // Spawn UI
        GameObject uiObj = GetFromPool(progressUIPrefab);
        CropProgressUI ui = uiObj.GetComponent<CropProgressUI>();

        ui.Initialize(crop.visual.transform, cam, player);

        crop.progressUI = ui;
    }

    void GrowCrops(float deltaTime)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Tile tile = tiles[x, z];
                if (tile.crop == null) continue;

                CropInstance crop = tile.crop;

                crop.timer += deltaTime;


                float t = 0f;
                

                switch (crop.state)
                {
                    case CropState.Seed:
                        t = crop.timer / crop.data.seedToGrowingTime;
                        if (crop.timer >= crop.data.seedToGrowingTime)
                        {
                            crop.timer = 0f;
                            crop.state = CropState.Growing;
                            SpawnCropVisual(x, z);
                        }
                        break;

                    case CropState.Growing:
                        
                        t = crop.timer / crop.data.growingToReadyTime;
                        if (crop.visual != null)
                        {
                            float scale = Mathf.Lerp(0.5f, 1f, Mathf.SmoothStep(0, 1, t));
                            crop.visual.transform.localScale = Vector3.one * scale;
                        }

                        if (crop.timer >= crop.data.growingToReadyTime)
                        {
                            crop.timer = 0f;
                            crop.state = CropState.Ready;
                            
                            SpawnCropVisual(x, z);
                        }
                        break;
                    case CropState.ReGrowing:
                        t = crop.timer / crop.data.growingToReadyTime;

                        if (crop.timer >= crop.data.growingToReadyTime)
                        {
                            crop.timer = 0f;
                            crop.state = CropState.Ready;
                            
                            SpawnCropVisual(x, z);
                        }
                        break;
                    case CropState.Ready:
                         t = 1f;
                        
                        break;
                }

                if (crop.progressUI != null && crop.progressUI.canvasGroup.alpha > 0.01f)
                    crop.progressUI.SetProgress(Mathf.Clamp01(t));

            }
        }
    }

    public bool TryHarvest(int x, int z)
    {
        Tile tile = GetTile(x, z);
        if (tile?.crop == null) return false;

        CropInstance crop = tile.crop;

        if (!crop.IsReady()) return false;
        if (crop.data.item != null)
            inventoryUI.TryAdd(crop.data.item);

        Debug.Log($"Harvested {crop.data.cropName}");

        if (crop.data.regrowable)
        {
            crop.state = CropState.ReGrowing;
            crop.timer = 0f;

            SpawnCropVisual(x, z);

            if (crop.visual != null)
                crop.visual.transform.localScale = Vector3.one;
        }
        else
        {
            if (crop.visual != null)
                ReturnToPoolPrefab(crop.visual, crop.sourcePrefab);

            if (crop.progressUI != null)
            {
                ReturnToPoolPrefab(crop.progressUI.gameObject, progressUIPrefab);
                crop.progressUI = null;
            }

            tile.crop = null;
            tile.type = TileType.Tilled;
        }

        return true; 
    }

    // ================================
    // POOLING METHODS
    // ================================
    GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        if (!pool.TryGetValue(prefab, out Queue<GameObject> q))
        {
            q = new Queue<GameObject>();
            pool[prefab] = q;
        }

        GameObject obj;
        if (q.Count > 0)
        {
            obj = q.Dequeue();
            obj.SetActive(true);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.parent = parent;
        }
        else
        {

            obj = Instantiate(prefab, position, rotation, parent);
        }

        return obj;
    }
    GameObject GetFromPool(GameObject prefab)
    {
        if (pool.TryGetValue(prefab, out Queue<GameObject> queue))
        {
            if (queue.Count > 0)
            {
                GameObject obj = queue.Dequeue();
                obj.SetActive(true);
                return obj;
            }
        }

        // Nothing available → create new
        GameObject newObj = Instantiate(prefab);
        return newObj;
    }

    void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        if (!pool.TryGetValue(obj, out Queue<GameObject> q))
        {
            q = new Queue<GameObject>();
            pool[obj] = q;
        }
        q.Enqueue(obj);
    }
    void ReturnToPoolPrefab(GameObject obj, GameObject prefab)
    {
        obj.SetActive(false);

        if (!pool.ContainsKey(prefab))
            pool[prefab] = new Queue<GameObject>();

        pool[prefab].Enqueue(obj);
    }

    void PrewarmPool(GameObject prefab, int count)
    {
        if (!pool.ContainsKey(prefab))
            pool[prefab] = new Queue<GameObject>();

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool[prefab].Enqueue(obj);
        }
    }


    Vector3 GridToWorldFromTile(Tile tile)
    {
        // Find grid coordinates of tile
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (tiles[x, y] == tile)
                    return GridToWorld(x, y);

        return Vector3.zero;
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - originPosition.x) / tileSize);
        int z = Mathf.FloorToInt((worldPos.z - originPosition.z) / tileSize);
        return new Vector2Int(x, z);
    }

    public Vector3 GridToWorld(int x, int z)
    {
        Vector3 pos = new Vector3(originPosition.x + x * tileSize + tileSize / 2f,
                                  0,
                                  originPosition.z + z * tileSize + tileSize / 2f);
        if (Terrain.activeTerrain != null)
            pos.y = Terrain.activeTerrain.SampleHeight(pos);
        return pos;
    }

    public Tile GetTile(int x, int z)
    {
        if (x < 0 || z < 0 || x >= width || z >= height) return null;
        return tiles[x, z];
    }
}