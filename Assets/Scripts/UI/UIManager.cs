using UnityEngine;

public class UIManager : MonoBehaviour
{
    public InventoryUI playerUI;
    public InventoryUI containerUI;

    public Inventory currentPlayerInventory;
    public Inventory currentContainerInventory;

    public bool IsOpen { get; private set; }

    void Awake()
    {
        playerUI.gameObject.SetActive(false);
        containerUI.gameObject.SetActive(false);

        SetCursor(false);
        IsOpen = false;
    }

    // ---------------- PLAYER ONLY ----------------
    public void TogglePlayerInventory(Inventory playerInventory)
    {
        if (IsOpen)
        {
            CloseAll();
        }
        else
        {
            OpenPlayerInventory(playerInventory);
        }
    }

    public void OpenPlayerInventory(Inventory playerInventory)
    {
        currentPlayerInventory = playerInventory;

        playerUI.Init(playerInventory);
        playerUI.gameObject.SetActive(true);

        containerUI.gameObject.SetActive(false);

        SetCursor(true);
        IsOpen = true;
    }

    // ---------------- CONTAINER ----------------
    public void OpenContainer(Inventory playerInventory, Inventory containerInventory)
    {
        currentPlayerInventory = playerInventory;
        currentContainerInventory = containerInventory;

        // Player UI
        playerUI.Init(playerInventory);
        playerUI.gameObject.SetActive(true);

        // Container UI
        containerUI.Init(containerInventory);
        containerUI.gameObject.SetActive(true);

        SetCursor(true);
        IsOpen = true;
    }

    // ---------------- CLOSE ----------------
    public void CloseAll()
    {
        ItemTransferHandler.Instance?.ClearSelection();
        playerUI.gameObject.SetActive(false);
        containerUI.gameObject.SetActive(false);

        currentContainerInventory = null;

        SetCursor(false);
        IsOpen = false;
    }

    void SetCursor(bool state)
    {
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = state;
    }

    public Inventory GetOtherInventory(Inventory source)
    {
        if (source == currentPlayerInventory)
            return currentContainerInventory;

        if (source == currentContainerInventory)
            return currentPlayerInventory;

        return null;
    }

    public void RefreshAll()
    {
        playerUI.UpdateUI();
        containerUI.UpdateUI();
    }
}