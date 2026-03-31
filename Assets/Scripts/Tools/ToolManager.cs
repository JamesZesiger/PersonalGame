using UnityEngine;
using UnityEngine.InputSystem;

public class ToolManager : MonoBehaviour
{
    [Header("Shared References")]
    public Camera cam;
    public FarmGrid grid;
    public GameObject preview;
    public ItemSelectionUI selectionUI;

    [Header("Tool Setup")]
    public Tool[] toolPrefabs;   // PREFABS, not scene objects
    public Transform toolHolder; // where the tool spawns (hand)
    public TilePreview tilePreviewScript;

    private Tool currentToolInstance;
    private int currentToolIndex = 0;

    void Start()
    {
        EquipTool(0);
    }

    void Update()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll > 0)
        {
            NextTool();
        }   

        else if (scroll < 0)
        {
            PreviousTool();
        }
    }

    public void OnUse()
    {
        if (tilePreviewScript.isEnabled)
            currentToolInstance?.Use();
        
    }

    void EquipTool(int index)
    {

        currentToolIndex = index;

        // Destroy old tool
        if (currentToolInstance != null)
        {
            Destroy(currentToolInstance.gameObject);
        }
        
        // Spawn new tool
        currentToolInstance = Instantiate(toolPrefabs[index], toolHolder);

        currentToolInstance.Initialize(cam, grid, preview);

        if(currentToolInstance.toolType == ToolType.Hoe || currentToolInstance.toolType == ToolType.Hammer)
        {
            Debug.Log("tool change");
            grid.SetTool(currentToolInstance.toolType);
        }

    }

    public void NextTool()
    {
        int nextIndex = (currentToolIndex + 1) % toolPrefabs.Length;
        selectionUI.setIcon(toolPrefabs[nextIndex].sprite);
        EquipTool(nextIndex);
    }

    void PreviousTool()
    {
        int prevIndex = currentToolIndex - 1;
        if (prevIndex < 0) prevIndex = toolPrefabs.Length - 1;
        selectionUI.setIcon(toolPrefabs[prevIndex].sprite);
        EquipTool(prevIndex);
    }

    public void AltUse()
    {
        currentToolInstance?.TryAlt();
    }
}