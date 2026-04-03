using System.Collections.Generic;
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
    public List<Tool> toolPrefabs = new List<Tool>();
    public Transform toolHolder;
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
            NextTool();
        else if (scroll < 0)
            PreviousTool();
    }

    public void OnUse()
    {
        if (tilePreviewScript.isEnabled)
        {
            currentToolInstance?.Use();
            toolPrefabs[currentToolIndex].numUses = currentToolInstance.numUses;
            if (currentToolInstance.numUses < 1)
            {
                RemoveTool(currentToolIndex);
                EquipTool(0);
            }
        }
    }

    void EquipTool(int index)
    {
        currentToolIndex = index;

        if (currentToolInstance != null)
            Destroy(currentToolInstance.gameObject);

        currentToolInstance = Instantiate(toolPrefabs[index], toolHolder);
        currentToolInstance.Initialize(cam, grid, preview);

        if (currentToolInstance.toolType == ToolType.Hoe || currentToolInstance.toolType == ToolType.Hammer)
        {
            grid.SetTool(currentToolInstance.toolType);
        }
    }

    public void NextTool()
    {
        int nextIndex = (currentToolIndex + 1) % toolPrefabs.Count;
        selectionUI.setIcon(toolPrefabs[nextIndex].sprite);
        EquipTool(nextIndex);
    }

    void PreviousTool()
    {
        int prevIndex = currentToolIndex - 1;
        if (prevIndex < 0) prevIndex = toolPrefabs.Count - 1;
        selectionUI.setIcon(toolPrefabs[prevIndex].sprite);
        EquipTool(prevIndex);
    }

    public void AltUse()
    {
        currentToolInstance?.TryAlt();
    }

    public void AddTool(Tool tool)
    {
        toolPrefabs.Add(tool);
    }

    public void RemoveTool(int index)
    {
        toolPrefabs.RemoveAt(index);
    }
}