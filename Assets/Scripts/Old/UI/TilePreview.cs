using UnityEngine;
using UnityEngine.InputSystem; // Needed for Input System

public class TilePreview : MonoBehaviour
{
    public Camera mainCamera; // Assign in Inspector
    public GameObject cam;
    public FarmGrid farmGrid;
    public LayerMask terrainMask;
    public float range = 10f;
    public float height = 0.36f;

    public bool useMousePosition = false; // Toggle between camera and mouse
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
        useMousePosition = Cursor.visible;
        Vector3 targetPos = Vector3.zero;
        bool hitDetected = false;

       if (useMousePosition)
        {
            if (Mouse.current != null) // ✅ Make sure the mouse exists
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                if (mousePos != null)
                {
                   Ray ray = mainCamera.ScreenPointToRay(mousePos);

                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, terrainMask))
                    {
                        if (hit.collider.CompareTag("Ground"))
                        {
                            hitDetected = true;
                            targetPos = hit.point;
                        }
                    } 
                }
                
            }
        }
        else
        {
            // Camera forward method
            Vector3 dir = cam.transform.forward;
            dir.Normalize();

            if (Physics.Raycast(cam.transform.position, dir, out RaycastHit hit, range, terrainMask))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    hitDetected = true;
                    targetPos = hit.point;
                }
            }
        }

        if (hitDetected)
        {
            SetAlpha(0.5f);

            Vector2Int gridPos = farmGrid.WorldToGrid(targetPos);
            Vector3 worldPos = farmGrid.GridToWorld(gridPos.x, gridPos.y);
            worldPos.y += height;

            transform.position = worldPos;
            isEnabled = true;
        }
        else
        {
            SetAlpha(0f);
            isEnabled = false;
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