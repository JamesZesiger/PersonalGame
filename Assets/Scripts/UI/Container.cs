using UnityEngine;

public class Container : MonoBehaviour, IInteractable
{
    public Inventory inventory;
    public UIManager uiManager;
    public Inventory playerInventory;

    public void Interact(PlayerInteraction player)
    {
        uiManager.OpenContainer(playerInventory, inventory);
    }
}
