using UnityEngine;

public class Door : Interactable
{
    public enum DoorType
    {
        HasGauge,
        HasWindow,
        HasNone
    }
    
    
    public DoorType doorType;
    [Header("For Door with Key")]
    [SerializeField] private bool needsKey;
    
    [SerializeField] private int doorID;

    [Header("For Door with Gauge Only")]
    [SerializeField] private int gaugeReading;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override bool Interact(GameObject other)
    {
        
        Debug.Log("Trying to open door: " + doorID);
        PlayerController pc = other.GetComponent<PlayerController>();
        if (needsKey && !pc.HasKey(doorID))
        {
            //Locked; doesn't have key
            //Broadcast door failed open
            //EventManager.FailedOpenDoor
            return false;
        }
        Debug.Log("Opened door: " + doorID);
        //EventManager.SuccessfullyOpenedDoor
        
        
        //Play door open animation
        Destroy(transform.parent.gameObject);
        return true;
    }
}
