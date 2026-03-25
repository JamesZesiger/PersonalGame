using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public FarmGrid grid;
    public GameObject preview;

    public void OnInteract()
    {
        Vector2Int pos = grid.WorldToGrid(preview.transform.position);

        grid.TryHarvest(pos.x, pos.y);
    }
}