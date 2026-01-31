using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public enum InteractableType
    {
        Key,
        Door,
        GasMask
    }   
    [SerializeField] private InteractableType interactableType;
    
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

    public InteractableType getInteractableType()
    {
        return interactableType;
    }

    public abstract bool Interact(GameObject other = null);

    public virtual void OnDestroy()
    {
        if(interactableZone != null)
        {
            interactableZone = null;
        }
        //interactableZone.
    }

}
