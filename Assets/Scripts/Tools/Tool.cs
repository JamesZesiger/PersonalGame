using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    public string toolName;

    public abstract void Use();
    public virtual void Initialize(Camera cam, FarmGrid grid, GameObject preview) {}
}
