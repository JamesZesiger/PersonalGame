using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(menuName = "Farming/TileSet")]
public class TileSet : ScriptableObject
{
    public GameObject Default;
    public GameObject Cap;
    public GameObject Tunnel;
    public GameObject Corner;
    public GameObject InnerCorner;
    public GameObject L;
    public GameObject T;
    public GameObject Flat;
    public GameObject TranstionMid;
    public GameObject Intersection;
    public GameObject Edge;
    public GameObject TransitionL;
    public GameObject TransitionR;
    public GameObject TransitionTwo;
    public GameObject SquareConnection;
    public TileType[] type ;

    public int x_rotation = -90;
    private Dictionary<int, TileVisual> tileLookup;

    public void BuildTileLookup()
    {
        tileLookup = new Dictionary<int, TileVisual>();

        // Bit positions for reference
        // 128  1  2
        // 64   X  4
        // 32  16  8

        // ==========================
        // SOLO
        // ==========================
        tileLookup[0] = new TileVisual(Default, Quaternion.Euler(x_rotation, 0, 0));

        // ==========================
        // CAPS (1 neighbor)
        // ==========================
        tileLookup[1] = new TileVisual(Cap, Quaternion.Euler(x_rotation, 180, 0));    // top
        tileLookup[4] = new TileVisual(Cap, Quaternion.Euler(x_rotation, -90, 0));    // right
        tileLookup[16] = new TileVisual(Cap, Quaternion.Euler(x_rotation, 0, 0));           // bottom
        tileLookup[64] = new TileVisual(Cap, Quaternion.Euler(x_rotation, 90, 0));     // left

        // ==========================
        // STRAIGHT TUNNELS (2 neighbors opposite)
        // ==========================
        tileLookup[1 | 16] = new TileVisual(Tunnel, Quaternion.Euler(x_rotation, 90, 0));     // top-bottom
        tileLookup[4 | 64] = new TileVisual(Tunnel, Quaternion.Euler(x_rotation, 0, 0)); // left-right

        // ==========================
        // OUTER CORNERS (2 neighbors adjacent)
        // ==========================
        tileLookup[1 | 2 | 4 ] = new TileVisual(Corner, Quaternion.Euler(x_rotation, 90, 0));
        tileLookup[4 | 8 | 16] = new TileVisual(Corner, Quaternion.Euler(x_rotation, 180, 0));
        tileLookup[16 | 32 | 64] = new TileVisual(Corner, Quaternion.Euler(x_rotation, -90, 0));
        tileLookup[64 | 128 | 1] = new TileVisual(Corner, Quaternion.Euler(x_rotation, 0, 0));

        // ==========================
        // T-SHAPES (3 neighbors)
        // ==========================
        tileLookup[1 | 4 | 16] = new TileVisual(T, Quaternion.Euler(x_rotation, -90, 0));       // missing left
        tileLookup[4 | 16 | 64] = new TileVisual(T, Quaternion.Euler(x_rotation, 0, 0)); // missing top
        tileLookup[16 | 64 | 1] = new TileVisual(T, Quaternion.Euler(x_rotation, 90, 0)); // missing right
        tileLookup[64 | 1 | 4] = new TileVisual(T, Quaternion.Euler(x_rotation, 180, 0));  // missing bottom

        // ==========================
        // CENTER (4 neighbors)
        // ==========================
        tileLookup[1 | 4 | 16 | 64] = new TileVisual(Intersection, Quaternion.Euler(x_rotation, 0, 0));

        // ==========================
        // INNER CORNERS
        // These occur when diagonal is missing but both adjacent cardinals exist
        // ==========================
        tileLookup[1 | 4 | 8 | 16 | 32 | 64 | 128] = new TileVisual(InnerCorner, Quaternion.Euler(x_rotation, -90, 0));   // top-right missing diag
        tileLookup[1 | 2 | 4 | 16 | 32 | 64 | 128] = new TileVisual(InnerCorner, Quaternion.Euler(x_rotation, 0, 0));        // right-bottom
        tileLookup[1 | 2 | 4 | 8 | 16 | 64 | 128] = new TileVisual(InnerCorner, Quaternion.Euler(x_rotation, 90, 0)); // bottom-left
        tileLookup[1 | 2 | 4 | 8 | 16 | 32 | 64 ] = new TileVisual(InnerCorner, Quaternion.Euler(x_rotation, 180, 0)); // left-top

        // ==========================
        // flat 
        // ==========================
        tileLookup[1 | 4 | 16 | 64 | 128 | 2 | 8 | 32] = new TileVisual(Flat, Quaternion.Euler(x_rotation*-1, 0, 0)); // all 8 neighbors

        // ==========================
        // END CAPS (2 neighbors diagonally, like a T->flat transition)
        // ==========================
        tileLookup[1 | 2 | 4 | 16 | 64 | 128] = new TileVisual(TranstionMid, Quaternion.Euler(x_rotation, 180, 0));  // top-left end
        tileLookup[1 | 4 | 16 | 32 | 64 | 128] = new TileVisual(TranstionMid, Quaternion.Euler(x_rotation, 90, 0));            // top-right end
        tileLookup[ 4 | 8 | 16 | 32 | 64 | 1] = new TileVisual(TranstionMid, Quaternion.Euler(x_rotation, 0, 0));          // bottom-left
        tileLookup[1 | 2 | 4 | 16 | 8 | 64] = new TileVisual(TranstionMid, Quaternion.Euler(x_rotation, -90, 0));    // bottom-right

        // ==========================
        // edges
        // ==========================
        tileLookup[1 | 16 | 2 | 4 | 8] = new TileVisual(Edge, Quaternion.Euler(x_rotation, -90, 0));
        tileLookup[1 | 64 | 2 | 4 | 128] = new TileVisual(Edge, Quaternion.Euler(x_rotation, 180, 0));
        tileLookup[1 | 16 | 32 | 64 | 128] = new TileVisual(Edge, Quaternion.Euler(x_rotation, 90, 0));
        tileLookup[4 | 8 | 64 | 32 | 16] = new TileVisual(Edge, Quaternion.Euler(x_rotation, 0, 0));


        // Bit positions for reference
                // 128  1  2
                // 64   X  4
                // 32  16  8
        tileLookup[1 | 128 | 64 | 16 ] = new TileVisual(TransitionR, Quaternion.Euler(x_rotation, 180, 0));
        tileLookup[1 | 2 | 4 | 16 ] = new TileVisual(TransitionL, Quaternion.Euler(x_rotation, 180, 0));
        tileLookup[16 | 8 | 4 | 1 ] = new TileVisual(TransitionR, Quaternion.Euler(x_rotation, 0, 0));
        tileLookup[16 | 32 | 64 | 1 ] = new TileVisual(TransitionL, Quaternion.Euler(x_rotation, 0, 0));

        tileLookup[64 | 128 | 1 | 4 ] = new TileVisual(TransitionL, Quaternion.Euler(x_rotation, 90, 0));
        tileLookup[64 | 32 | 16 | 4 ] = new TileVisual(TransitionR, Quaternion.Euler(x_rotation, 90, 0));
        tileLookup[4 | 2 | 1 | 64 ] = new TileVisual(TransitionR, Quaternion.Euler(x_rotation, -90, 0));
        tileLookup[4 | 8 | 16 | 64 ] = new TileVisual(TransitionL, Quaternion.Euler(x_rotation, -90, 0));

        tileLookup[1 | 4 ] = new TileVisual(L, Quaternion.Euler(x_rotation, 90, 0));
        tileLookup[1 | 64 ] = new TileVisual(L, Quaternion.Euler(x_rotation, 0, 0));
        tileLookup[64 | 16 ] = new TileVisual(L, Quaternion.Euler(x_rotation, 270, 0));
        tileLookup[16 | 4 ] = new TileVisual(L, Quaternion.Euler(x_rotation, 180, 0));

        tileLookup[1|2|4|16|64 ] = new TileVisual(TransitionTwo, Quaternion.Euler(x_rotation, 270, 0));
        tileLookup[1|64|16|4|8 ] = new TileVisual(TransitionTwo, Quaternion.Euler(x_rotation, 0, 0));
        tileLookup[4|16|1|128|64 ] = new TileVisual(TransitionTwo, Quaternion.Euler(x_rotation, -180, 0));
        tileLookup[1|4|16|32|64] = new TileVisual(TransitionTwo, Quaternion.Euler(x_rotation, 90, 0));

        tileLookup[1 | 4 | 8| 16 | 64 | 128] = new TileVisual(SquareConnection, Quaternion.Euler(x_rotation, 0, 0));
        tileLookup[1 | 2 | 4 | 16 | 32 | 64 ] = new TileVisual(SquareConnection, Quaternion.Euler(x_rotation, 90, 0));
    }

    public TileVisual GetTileVisual(int bitmask)
    {
        if (tileLookup.TryGetValue(bitmask, out var visual))
            return visual;

        return new TileVisual(Default, Quaternion.Euler(x_rotation, 0, 0)); // fallback
    }
        public GameObject[] GetAllPrefabs()
    {
        return new GameObject[]
        {
            Default,
            Cap,
            Tunnel,
            Corner,
            InnerCorner,
            L,
            T,
            Flat,
            TranstionMid, Intersection,
            Edge,
            TransitionL,     
            TransitionR,
            TransitionTwo,
            SquareConnection
        };
    }
}