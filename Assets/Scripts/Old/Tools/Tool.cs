using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    public string toolName;
    public Sprite sprite;
    public ToolType toolType = ToolType.Default;
    public int numUses = 1;
    public bool isConsumable = false;
    public abstract void Use();
    public virtual void Initialize(Camera cam, FarmGrid grid, GameObject preview) {}
    protected abstract void AltUse() ;

    public virtual void TryAlt(){ AltUse();}

}
