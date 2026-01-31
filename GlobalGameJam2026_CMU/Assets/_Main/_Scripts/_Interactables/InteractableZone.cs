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
    public Interactable interactable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerInZone)
        {
            
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            interactable.setGrabbable(true);
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            interactable.setGrabbable(false);
        }
    }
}
