using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    public int index;
    public InventoryUI parentUI;

    [Header("Visuals")]
    public Image icon;
    public TextMeshProUGUI quantityText;
    public Image selectionBorder; // Optional: a border Image to show selection

    public void SetSlot(InventorySlot slot)
    {
        bool hasItem = slot != null && slot.item != null;

        if (icon != null)
        {
            icon.sprite = hasItem ? slot.item.icon : null;
            icon.enabled = hasItem;
        }

        if (quantityText != null)
        {
            quantityText.text = (hasItem && slot.quantity > 1) ? slot.quantity.ToString() : "";
        }

        // Refresh selection highlight state
        UpdateSelectionVisual(slot);
    }

    public void UpdateSelectionVisual(InventorySlot slot)
    {
        if (selectionBorder == null) return;
        bool selected = slot != null && ItemTransferHandler.Instance != null
                        && ItemTransferHandler.Instance.IsSelected(slot);
        selectionBorder.enabled = selected;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (parentUI == null) return;

        bool shiftHeld = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;

        if (shiftHeld)
        {
            parentUI.HandleShiftClick(index);
        }
        else
        {
            parentUI.HandleClick(index);
        }
    }
}