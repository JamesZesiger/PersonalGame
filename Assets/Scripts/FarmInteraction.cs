using UnityEngine;

public class FarmInteraction : MonoBehaviour
{
    public Camera cam;
    public FarmGrid grid;
    public LayerMask terrainMask;
    public float range = 100f;
    public GameObject preview;

    void Update()
    {
        
    }

    public void Interact()
    {


        Vector2Int gridPos = grid.WorldToGrid(preview.transform.position);
        Tile tile = grid.GetTile(gridPos.x, gridPos.y);
        Debug.Log(tile);
        if (tile != null)
            {
                grid.TillTile(gridPos.x,gridPos.y);
                tile.type = TileType.Tilled;
                Debug.Log($"Tilled tile at {gridPos}");
            }
    }
}