using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public InteractableZone interactableZone;
    private bool grabbable = false;
    public virtual void setGrabbable(bool isGrabbable)
    {
        this.grabbable = isGrabbable;
    }
    public bool isGrabbable()
    {
        return grabbable;
    }

    public abstract void Interact(GameObject other = null);

}
