using UnityEngine;

public class HoeTool : Tool
{
    public Camera cam;
    public FarmGrid grid;
    public LayerMask terrainMask;
    public float range = 100f;
    public GameObject preview;

    public override void Initialize(Camera cam, FarmGrid grid, GameObject preview)
    {
        this.cam = cam;
        this.grid = grid;
        this.preview = preview;
    }
    public override void Use()
    {
        Debug.Log("hoe used");
        if (preview == null || grid == null) return;
        Debug.Log("next");
        Vector2Int gridPos = grid.WorldToGrid(preview.transform.position);
        FarmTile tile = grid.GetTile(gridPos.x, gridPos.y);

        if (tile != null && !tile.isTilled)
        {
            grid.TillTile(gridPos.x, gridPos.y);
            tile.isTilled = true;

            Debug.Log($"[Hoe] Tilled tile at {gridPos}");
        }
    }
}