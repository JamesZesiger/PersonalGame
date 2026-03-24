using UnityEngine;

public class TilePreview : MonoBehaviour
{
    public GameObject cam;
    public FarmGrid farmGrid;
    public LayerMask terrainMask;
    public float range = 10f;
    public float height = 0.36f;
    

    void Update()
    {
        Vector3 dir = cam.transform.forward;
        dir.Normalize();

        if (Physics.Raycast(cam.transform.position, dir, out RaycastHit hit, range, terrainMask))
        {
            Vector2Int gridPos = farmGrid.WorldToGrid(hit.point);

            Vector3 worldPos = farmGrid.GridToWorld(gridPos.x, gridPos.y);

            // Slight offset to avoid z-fighting
            worldPos.y += height;

            transform.position = worldPos;
        }
    }
}