using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public GameObject inventoryPanel;
    public Transform gridParent;

    private GameObject[] slots;
    private bool isOpen = false;

    private Inventory currentInventory;

    public void Init(Inventory inventory)
    {
        currentInventory = inventory;

        // Rebuild slots if needed
        if (slots != null)
        {
            foreach (var s in slots)
                Destroy(s);
        }

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
        if (currentInventory == null) return;

        for (int i = 0; i < slots.Length; i++)
        {
            var icon = slots[i].transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
            var text = slots[i].transform.GetChild(1).GetComponent<TMP_Text>();

            if (i < currentInventory.items.Count)
            {
                text.text = $"{currentInventory.items[i].quantity}";
                icon.sprite = currentInventory.items[i].item.icon;
                icon.enabled = true;
            }
            else
            {
                text.text = "";
                icon.enabled = false;
            }
        }
    }

    public void Toggle()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);
        HandleCursor();
    }

    void HandleCursor()
    {
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;
    }
}