using UnityEngine;
using System.Collections.Generic;



[CreateAssetMenu(menuName = "Farming/StructureSet")]
public class StructureSet : ScriptableObject
{
    GameObject activeStructure;
    List<GameObject> Structures = new List<GameObject>();
    private Dictionary<int, TileVisual> tileLookup;
    public void BuildTileLookup()
    {
        for (int i; i< Structures.Count(); i++)
        {
            
        }
    }

    public TileVisual GetTileVisual(int bitmask)
    {
        if (tileLookup.TryGetValue(bitmask, out var visual))
            return visual;

        return new TileVisual(Default, Quaternion.Euler(x_rotation, 0, 0)); // fallback
    }
    public GameObject[] GetAllPrefabs()
    {
        return Structures.ToArray();
    }
}
