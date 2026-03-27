using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public void Interact(PlayerInteraction player)
    {
        Debug.Log("Chest opened!");
        // Open UI, give items, etc.
    }
}