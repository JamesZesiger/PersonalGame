using UnityEngine;

public struct TileVisual
{
    public GameObject prefab;
    public Quaternion rotation;

    public TileVisual(GameObject prefab, Quaternion rotation)
    {
        this.prefab = prefab;
        this.rotation = rotation;
    }
}