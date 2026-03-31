using UnityEngine;
using System.Collections.Generic;



[CreateAssetMenu(menuName = "Farming/StructureSet")]
public class StructureSet : ScriptableObject
{
    GameObject activeStructure;
    public List<GameObject> Structures = new List<GameObject>();
    public int x_rotation;
    private Dictionary<int, TileVisual> tileLookup;
    public void BuildTileLookup()
    {
        tileLookup = new Dictionary<int, TileVisual>();
        for (int i = 0; i < Structures.Count; i++)
        {
            tileLookup.Add(i+1, new TileVisual(Structures[i], Quaternion.Euler(x_rotation, 0, 0))); 
        }
    }

    public TileVisual GetStructureVisual(int index)
    {
        if (tileLookup.TryGetValue(index, out var visual))
            return visual;

        return new TileVisual(Structures[0], Quaternion.Euler(x_rotation, 0, 0)); // fallback
    }
    public GameObject[] GetAllPrefabs()
    {
        return Structures.ToArray();
    }
}
