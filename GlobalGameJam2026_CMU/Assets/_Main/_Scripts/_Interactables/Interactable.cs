using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public InteractableZone interactableZone;
    private bool isGrabbable = false;
    public virtual void setGrabbable(bool isGrabbable)
    {
        this.isGrabbable = isGrabbable;
    }


}
