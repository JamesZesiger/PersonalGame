using UnityEngine;

public class SeedTool : Tool
{
    private FarmGrid grid;
    private GameObject preview;

    public CropData cropToPlant;

    public override void Initialize(Camera cam, FarmGrid grid, GameObject preview)
    {
        this.grid = grid;
        this.preview = preview;
    }

    public override void Use()
    {
        Vector2Int pos = grid.WorldToGrid(preview.transform.position);

        bool sucsess = grid.PlantCrop(pos.x, pos.y, cropToPlant);
        if (isConsumable && sucsess) numUses -= 1;
        Debug.Log(numUses);
    }
    protected override void AltUse(){}
}