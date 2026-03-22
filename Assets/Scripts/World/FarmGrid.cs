using System.Collections.Generic;
using UnityEngine;

public struct TileVisual
{
    public GameObject prefab;
    public Quaternion rotation;

    public TileVisual(GameObject p, Quaternion r)
    {
        prefab = p;
        rotation = r;
    }
}

public class FarmTile
{
    public bool isTilled = false;
    public GameObject visualObject; // The instantiated prefab
}

public class FarmChunk
{
    public Mesh mesh;
    public GameObject chunkObject;
    public List<CombineInstance> combineList = new List<CombineInstance>();
}

public class FarmGrid : MonoBehaviour
{
    [Header("Tile Prefabs")]
    public GameObject Till_solo;
    public GameObject Cap;
    public GameObject Tunnel;
    public GameObject Corner;
    public GameObject InnerCorner;
    public GameObject T;
    public GameObject Flat;
    public GameObject TranstionMid;
    public GameObject Intersection;
    public GameObject Edge;
    public GameObject TransitionL;
    public GameObject TransitionR;
    public int x_rotation = -90;
    [Header("Grid Settings")]
    public int width = 50;
    public int height = 50;
    public float cellSize = 1f;
    public Vector3 originPosition;

    [Header("Chunk Settings")]
    public int chunkSize = 10;
    public Transform chunkParent;

    [Header("Options")]
    public bool instantiateObjects = true; // Set true to create GameObjects per tile

    private FarmTile[,] grid;
    private FarmChunk[,] chunks;
    private Dictionary<int, TileVisual> tileLookup;

    void Awake()
    {
        // Initialize grid
        grid = new FarmTile[width, height];
        for (int x = 0; x < width; x++)
            for (int z = 0; z < height; z++)
                grid[x, z] = new FarmTile();

        InitChunks();
        BuildTileLookup();
    }

    // ----------------------------
    // Grid Utilities
    // ----------------------------
    public FarmTile GetTile(int x, int z)
    {
        if (x < 0 || z < 0 || x >= width || z >= height) return null;
        return grid[x, z];
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - originPosition.x) / cellSize);
        int z = Mathf.FloorToInt((worldPos.z - originPosition.z) / cellSize);
        return new Vector2Int(x, z);
    }

    public Vector3 GridToWorld(int x, int z)
    {
        Vector3 pos = new Vector3(originPosition.x + x * cellSize + cellSize / 2f,
                                  0,
                                  originPosition.z + z * cellSize + cellSize / 2f);
        pos.y = Terrain.activeTerrain.SampleHeight(pos);
        return pos;
    }

    // ----------------------------
    // Tilling
    // ----------------------------
    public void TillTile(int x, int z)
    {
        FarmTile tile = GetTile(x, z);
        if (tile == null || tile.isTilled) return;

        tile.isTilled = true;

        UpdateTileVisual(x, z);
        UpdateNeighborsPropagated(x, z);
    }

    void UpdateNeighborsPropagated(int x, int z)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(new Vector2Int(x, z));
        visited.Add(new Vector2Int(x, z));

        while(queue.Count > 0)
        {
            Vector2Int pos = queue.Dequeue();
            FarmTile tile = GetTile(pos.x, pos.y);
            if(tile == null || !tile.isTilled) continue;

            TileVisual oldVisual = tile.visualObject != null ? 
                new TileVisual(tile.visualObject, Quaternion.identity) : 
                new TileVisual(null, Quaternion.identity);

            UpdateTileVisual(pos.x, pos.y);

            // If the visual changed, add neighbors to queue
            if(tile.visualObject != oldVisual.prefab)
            {
                Vector2Int[] neighbors = new Vector2Int[]
                {
                    new Vector2Int(pos.x+1, pos.y),
                    new Vector2Int(pos.x-1, pos.y),
                    new Vector2Int(pos.x, pos.y+1),
                    new Vector2Int(pos.x, pos.y-1)
                };

                foreach(var n in neighbors)
                {
                    if(!visited.Contains(n))
                    {
                        queue.Enqueue(n);
                        visited.Add(n);
                    }
                }
            }
        }
    }

    // ----------------------------
    // Tile Visuals
    // ----------------------------
    int GetBitmask(int x, int z)
    {
        bool top = GetTile(x, z + 1)?.isTilled ?? false;
        bool right = GetTile(x + 1, z)?.isTilled ?? false;
        bool bottom = GetTile(x, z - 1)?.isTilled ?? false;
        bool left = GetTile(x - 1, z)?.isTilled ?? false;

        bool topRight = (GetTile(x + 1, z + 1)?.isTilled ?? false) && top && right;
        bool bottomRight = (GetTile(x + 1, z - 1)?.isTilled ?? false) && bottom && right;
        bool bottomLeft = (GetTile(x - 1, z - 1)?.isTilled ?? false) && bottom && left;
        bool topLeft = (GetTile(x - 1, z + 1)?.isTilled ?? false) && top && left;

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

    TileVisual GetTileVisual(int x, int z)
    {
        int mask = GetBitmask(x, z);
        Debug.Log($"{x},{z}:{mask}");
        if (tileLookup.TryGetValue(mask, out TileVisual visual))
            return visual;

        return new TileVisual(Till_solo, Quaternion.Euler(x_rotation, 0, 0));
    }

    void UpdateTileVisual(int x, int z)
    {
        FarmTile tile = GetTile(x, z);
        if (tile == null || !tile.isTilled) return;

        TileVisual visual = GetTileVisual(x, z);

        // Destroy old object if exists
        if (tile.visualObject != null)
            Destroy(tile.visualObject);

        // Instantiate new prefab if option is enabled
        if (instantiateObjects && visual.prefab != null)
        {
            Vector3 pos = GridToWorld(x, z);
            pos.y-=0.01f;
            tile.visualObject = Instantiate(visual.prefab, pos, visual.rotation, chunkParent);
            Debug.Log($"intantiated:{visual.prefab}");
        }

        // Always update chunk mesh
        UpdateChunkAtTile(x, z);
    }

    // ----------------------------
    // Bitmask Tile Lookup
    // ----------------------------
    void BuildTileLookup()
    {
        tileLookup = new Dictionary<int, TileVisual>();

        // Bit positions for reference
        // 128  1  2
        // 64   X  4
        // 32  16  8

        // ==========================
        // SOLO
        // ==========================
        tileLookup[0] = new TileVisual(Till_solo, Quaternion.Euler(x_rotation, 0, 0));

        // ==========================
        // CAPS (1 neighbor)
        // ==========================
        tileLookup[1] = new TileVisual(Cap, Quaternion.Euler(x_rotation, 180, 0));    // top
        tileLookup[4] = new TileVisual(Cap, Quaternion.Euler(x_rotation, -90, 0));    // right
        tileLookup[16] = new TileVisual(Cap, Quaternion.Euler(x_rotation, 0, 0));           // bottom
        tileLookup[64] = new TileVisual(Cap, Quaternion.Euler(x_rotation, 90, 0));     // left

        // ==========================
        // STRAIGHT TUNNELS (2 neighbors opposite)
        // ==========================
        tileLookup[1 | 16] = new TileVisual(Tunnel, Quaternion.Euler(x_rotation, 90, 0));     // top-bottom
        tileLookup[4 | 64] = new TileVisual(Tunnel, Quaternion.Euler(x_rotation, 0, 0)); // left-right

        // ==========================
        // OUTER CORNERS (2 neighbors adjacent)
        // ==========================
        tileLookup[1 | 2 | 4 ] = new TileVisual(Corner, Quaternion.Euler(x_rotation, 90, 0));
        tileLookup[4 | 8 | 16] = new TileVisual(Corner, Quaternion.Euler(x_rotation, 180, 0));
        tileLookup[16 | 32 | 64] = new TileVisual(Corner, Quaternion.Euler(x_rotation, -90, 0));
        tileLookup[64 | 128 | 1] = new TileVisual(Corner, Quaternion.Euler(x_rotation, 0, 0));

        // ==========================
        // T-SHAPES (3 neighbors)
        // ==========================
        tileLookup[1 | 4 | 16] = new TileVisual(T, Quaternion.Euler(x_rotation, -90, 0));       // missing left
        tileLookup[4 | 16 | 64] = new TileVisual(T, Quaternion.Euler(x_rotation, 0, 0)); // missing top
        tileLookup[16 | 64 | 1] = new TileVisual(T, Quaternion.Euler(x_rotation, 90, 0)); // missing right
        tileLookup[64 | 1 | 4] = new TileVisual(T, Quaternion.Euler(x_rotation, 180, 0));  // missing bottom

        // ==========================
        // CENTER (4 neighbors)
        // ==========================
        tileLookup[1 | 4 | 16 | 64] = new TileVisual(Intersection, Quaternion.Euler(x_rotation, 0, 0));

        // ==========================
        // INNER CORNERS
        // These occur when diagonal is missing but both adjacent cardinals exist
        // ==========================
        tileLookup[1 | 4 | 8 | 16 | 32 | 64 | 128] = new TileVisual(InnerCorner, Quaternion.Euler(x_rotation, 90, 0));   // top-right missing diag
        tileLookup[1 | 2 | 4 | 16 | 32 | 64 | 128] = new TileVisual(InnerCorner, Quaternion.Euler(x_rotation, 180, 0));        // right-bottom
        tileLookup[1 | 2 | 4 | 8 | 16 | 64 | 128] = new TileVisual(InnerCorner, Quaternion.Euler(x_rotation, -90, 0)); // bottom-left
        tileLookup[1 | 2 | 4 | 8 | 16 | 32 | 64 ] = new TileVisual(InnerCorner, Quaternion.Euler(x_rotation, 0, 0)); // left-top

        // ==========================
        // flat 
        // ==========================
        tileLookup[1 | 4 | 16 | 64 | 128 | 2 | 8 | 32] = new TileVisual(Flat, Quaternion.Euler(x_rotation*-1, 0, 0)); // all 8 neighbors

        // ==========================
        // END CAPS (2 neighbors diagonally, like a T->flat transition)
        // ==========================
        tileLookup[1 | 128] = new TileVisual(TranstionMid, Quaternion.Euler(x_rotation, 180, 0));  // top-left end
        tileLookup[1 | 2] = new TileVisual(TranstionMid, Quaternion.Euler(x_rotation, 0, 0));            // top-right end
        tileLookup[16 | 32] = new TileVisual(TranstionMid, Quaternion.Euler(x_rotation, 0, 0));          // bottom-left
        tileLookup[16 | 8] = new TileVisual(TranstionMid, Quaternion.Euler(x_rotation, -90, 0));    // bottom-right

        // ==========================
        // edges
        // ==========================
        tileLookup[1 | 16 | 2 | 4 | 8] = new TileVisual(Edge, Quaternion.Euler(x_rotation, -90, 0));
        tileLookup[1 | 64 | 2 | 4 | 128] = new TileVisual(Edge, Quaternion.Euler(x_rotation, 180, 0));
        tileLookup[1 | 16 | 32 | 64 | 128] = new TileVisual(Edge, Quaternion.Euler(x_rotation, 90, 0));
        tileLookup[4 | 8 | 64 | 32 | 16] = new TileVisual(Edge, Quaternion.Euler(x_rotation, 0, 0));


        // Bit positions for reference
                // 128  1  2
                // 64   X  4
                // 32  16  8
        tileLookup[1 | 128 | 64 | 16 ] = new TileVisual(TransitionR, Quaternion.Euler(x_rotation, -90, 0));
        tileLookup[1 | 2 | 4 | 16 ] = new TileVisual(TransitionL, Quaternion.Euler(x_rotation, -90, 0));
        tileLookup[16 | 8 | 4 | 1 ] = new TileVisual(TransitionR, Quaternion.Euler(x_rotation, 90, 0));
        tileLookup[16 | 32 | 64 | 1 ] = new TileVisual(TransitionL, Quaternion.Euler(x_rotation, 90, 0));

        tileLookup[64 | 128 | 1 | 4 ] = new TileVisual(TransitionL, Quaternion.Euler(x_rotation, 180, 0));
        tileLookup[64 | 32 | 16 | 4 ] = new TileVisual(TransitionR, Quaternion.Euler(x_rotation, 180, 0));
        tileLookup[4 | 2 | 1 | 64 ] = new TileVisual(TransitionR, Quaternion.Euler(x_rotation, 0, 0));
        tileLookup[4 | 8 | 16 | 64 ] = new TileVisual(TransitionL, Quaternion.Euler(x_rotation, 0, 0));



        
    }

    // ----------------------------
    // Chunks
    // ----------------------------
    void InitChunks()
    {
        int chunkX = Mathf.CeilToInt((float)width / chunkSize);
        int chunkZ = Mathf.CeilToInt((float)height / chunkSize);

        chunks = new FarmChunk[chunkX, chunkZ];

        for(int cx=0; cx<chunkX; cx++)
        for(int cz=0; cz<chunkZ; cz++)
        {
            GameObject obj = new GameObject($"Chunk_{cx}_{cz}");
            obj.transform.parent = chunkParent;

            MeshFilter mf = obj.AddComponent<MeshFilter>();
            MeshRenderer mr = obj.AddComponent<MeshRenderer>();

            FarmChunk chunk = new FarmChunk();
            chunk.mesh = new Mesh();
            chunk.chunkObject = obj;

            mf.mesh = chunk.mesh;

            chunks[cx,cz] = chunk;
        }
    }

    void UpdateChunkAtTile(int x, int z)
    {
        int cx = x / chunkSize;
        int cz = z / chunkSize;
        if(cx<0||cz<0||cx>=chunks.GetLength(0)||cz>=chunks.GetLength(1)) return;

        RebuildChunk(cx, cz);
    }

    void RebuildChunk(int cx, int cz)
    {
        FarmChunk chunk = chunks[cx, cz];
        chunk.combineList.Clear();

        for(int x=0; x<chunkSize; x++)
        for(int z=0; z<chunkSize; z++)
        {
            int gx = cx*chunkSize + x;
            int gz = cz*chunkSize + z;

            FarmTile tile = GetTile(gx, gz);
            if(tile==null || !tile.isTilled) continue;

            TileVisual visual = GetTileVisual(gx, gz);

            MeshFilter mf = visual.prefab.GetComponent<MeshFilter>();
            if(mf==null) continue;

            CombineInstance ci = new CombineInstance();
            ci.mesh = mf.sharedMesh;
            ci.transform = Matrix4x4.TRS(GridToWorld(gx,gz), visual.rotation, Vector3.one);
            chunk.combineList.Add(ci);
        }

        chunk.mesh.Clear();
        chunk.mesh.CombineMeshes(chunk.combineList.ToArray(), true, true);
    }
}