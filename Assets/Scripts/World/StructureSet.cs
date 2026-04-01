using UnityEngine;
using System.Collections.Generic;



[CreateAssetMenu(menuName = "Farming/StructureSet")]
public class StructureSet : ScriptableObject
{
    GameObject activeStructure;
    public List<GameObject> Structures = new List<GameObject>();
    public List<Sprite> Sprites = new List<Sprite>();
    public int x_rotation;
    private Dictionary<int, TileVisual> tileLookup;
    private Dictionary<int, Sprite> spriteLookup;
    public void BuildTileLookup()
    {
        tileLookup = new Dictionary<int, TileVisual>();
        for (int i = 0; i < Structures.Count; i++)
        {
            tileLookup.Add(i, new TileVisual(Structures[i], Quaternion.Euler(x_rotation, 0, 0))); 
        }

        spriteLookup = new Dictionary<int, Sprite>();
        for (int i = 0; i < Sprites.Count; i++)
        {
            spriteLookup.Add(i, Sprites[i]); 
        }
    }

    public TileVisual GetStructureVisual(int? index)
    {
        if (index != null)
            if (tileLookup.TryGetValue(index??0, out var visual))
                return visual;

        return new TileVisual(Structures[1], Quaternion.Euler(x_rotation, 0, 0)); // fallback
    }
    public GameObject[] GetAllPrefabs()
    {
        return Structures.ToArray();
    }

    public Sprite GetStructureSprite(int? index)
    {
        if (index != null)
            if (spriteLookup.TryGetValue(index??0, out var sprite))
                return sprite;

        return null; // fallback
    }
}
