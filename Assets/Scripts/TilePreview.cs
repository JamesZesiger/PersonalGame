using UnityEngine;

public class TilePreview : MonoBehaviour
{
    public GameObject cam;
    public FarmGrid farmGrid;
    public LayerMask terrainMask;
    public float range = 10f;
    public float height = 0.36f;
    public bool isEnabled;
    private Renderer rend;
    private Material mat;
    private Color originalColor;


    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            mat = rend.material; // Use instance material
            originalColor = mat.color;
            SetAlpha(0f);
        }
        isEnabled = false;
    }

    void Update()
    {
        Vector3 dir = cam.transform.forward;
        dir.Normalize();

        bool hitInRange = Physics.Raycast(cam.transform.position, dir, out RaycastHit hit, range, terrainMask);

        if (hitInRange && hit.collider.gameObject.tag == "Ground")
        {
            SetAlpha(0.5f);
            Vector2Int gridPos = farmGrid.WorldToGrid(hit.point);
            Vector3 worldPos = farmGrid.GridToWorld(gridPos.x, gridPos.y);
            worldPos.y += height;

            transform.position = worldPos;

            // Fade in
            isEnabled = true;
        }
        else
        {
            // Fade out
            isEnabled = false;
            SetAlpha(0f);
        }
    }
    void SetAlpha(float alpha)
    {
        if (mat != null)
        {
            Color c = mat.color;
            c.a = alpha;
            mat.color = c;
        }
    }
}