using NUnit.Framework;
using UnityEngine;

public class InteractableZone : MonoBehaviour
{
    public enum ZoneType
    {
        Key,
        Door,
        GasMask
    }

    [SerializeField] private ZoneType zoneType;
    
    private bool isPlayerInZone = false;
    private PlayerController playerController;
    public Interactable interactable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            
            Debug.Log("Player entered");
            Debug.Log(other.GetType().ToString());
            playerController = other.GetComponent<PlayerController>();
            playerController.setInteractableZone(this);
            isPlayerInZone = true;
            interactable.setGrabbable(true);
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerController.removeInteractableZone();
            playerController = null;
            isPlayerInZone = false;
            interactable.setGrabbable(false);
        }
    }

    public bool interactPressed()
    {
        if(isPlayerInZone && interactable.isGrabbable())
        {   
            return interactable.Interact(playerController.gameObject);
        }

        //Should never occur
        Debug.Log("How did we even get here");
        return false;
    }

    public ZoneType getZoneType()
    {
        return zoneType;
    }
    void OnDestroy()
    {
        if(playerController != null)
        {
            playerController.removeInteractableZone();
        }
    }
}
