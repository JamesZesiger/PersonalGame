using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    public int index;
    public InventoryUI parentUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (parentUI == null) return;

        // SHIFT CLICK (Input System)
        if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed)
        {
            parentUI.HandleShiftClick(index);
        }
    }
}