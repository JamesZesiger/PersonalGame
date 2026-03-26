using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public GameObject slotPrefab;
    public GameObject inventoryPanel;
    public Transform gridParent;

    private GameObject[] slots;
    private bool isOpen = false;
    void Start()
    {
        inventoryPanel.SetActive(isOpen);
        slots = new GameObject[inventory.size];

        for (int i = 0; i < inventory.size; i++)
        {
            GameObject slot = Instantiate(slotPrefab, gridParent);
            slots[i] = slot;
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
           
            var icon = slots[i].transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
            var text = slots[i].transform.GetChild(1).GetComponent<TMP_Text>();
            if (i < inventory.items.Count)
            {
                text.text = $"{inventory.items[i].quantity}";
                Debug.Log(inventory.items[i].item.icon);
                icon.sprite = inventory.items[i].item.icon;
                icon.enabled = true;
            }
            else
            {
                text.text = "";
                icon.enabled = false;
            }
        }
    }

    public void openInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        HandleCursor();
    }

    void HandleCursor()
    {
        if (isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void TryAdd(Item item)
    {
        if (item == null) return;
        inventory.AddItem(item);
        UpdateUI();
    }
}