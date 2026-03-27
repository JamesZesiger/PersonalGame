using UnityEngine;

public class ShovelTool : Tool
{
    private FarmGrid grid;
    private GameObject preview;

    public override void Initialize(Camera cam, FarmGrid grid, GameObject preview)
    {
        this.grid = grid;
        this.preview = preview;

    }

    public override void Use()
    {
        if (grid == null || preview == null)
        {
            Debug.LogWarning("[Shovel] Missing references!");
            return;
        }

        Vector2Int gridPos = grid.WorldToGrid(preview.transform.position);
        Tile tile = grid.GetTile(gridPos.x, gridPos.y);

        if (tile != null && tile.type == TileType.Tilled)
        {
            grid.UntillTile(gridPos.x, gridPos.y);

            Debug.Log($"[Shovel] Cleared tile at {gridPos}");
        }
    }
}