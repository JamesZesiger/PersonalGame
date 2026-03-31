using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class HammerSlotUI : MonoBehaviour, IPointerClickHandler
{
    public int index;
    public HammerUI parentUI;

    [Header("Visuals")]
    public Image icon;
    public Image selectionBorder;

    public void SetSlot(HammerSlot slot)
    {

        if (icon != null)
        {
            icon.sprite = slot.sprite;
            icon.enabled = true;
        }


        // Refresh selection highlight state
        //UpdateSelectionVisual(slot);
    }

    // public void UpdateSelectionVisual(HammerSlot slot)
    // {
    //     if (selectionBorder == null) return;
    //     bool selected = slot != null && ItemTransferHandler.Instance != null
    //                     && ItemTransferHandler.Instance.IsSelected(slot);
    //     selectionBorder.enabled = selected;
    // }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (parentUI == null) return;
        parentUI.selectedIndex = index;
    }
}