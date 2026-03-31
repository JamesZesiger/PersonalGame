using UnityEngine;

public class HammerUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public GameObject structPanel;
    public Transform gridParent;
    public StructureSet structureSet;
    private GameObject[] slots;
    private bool isOpen = true;
    public int selectedIndex = 0;
    public void Init(StructureSet set)
    {
        structureSet = set;

        // Rebuild slots if needed
        if (slots != null)
        {
            foreach (var s in slots)
                Destroy(s);
        }

        slots = new GameObject[structureSet.Structures.Count];

        for (int i = 0; i < structureSet.Structures.Count; i++)
        {
            GameObject slot = Instantiate(slotPrefab, gridParent);
            var slotUI = slot.GetComponent<HammerSlotUI>();
            slotUI.parentUI = this;
            slotUI.index = i;
            slots[i] = slot;
        }

        UpdateUI();
    }
    public void UpdateUI()
    {
        if (structureSet == null) return;

        for (int i = 0; i < slots.Length; i++)
        {
            var icon = slots[i].transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        }
    }

    public void Toggle()
    {
        isOpen = !isOpen;
        structPanel.SetActive(isOpen);
        HandleCursor();
    }

    void HandleCursor()
    {
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;
    }


}
