using Unity.VisualScripting;
using UnityEngine;

public class Door : Interactable
{
    public enum DoorType
    {
        HasGauge,
        HasWindow,
        HasNone,
        HasKeyPad
    }
    
    
    public DoorType doorType;

    public Animator doorAnimator;

    public AudioClip doorOpenSound;

    public bool opened;
    [Header("For Door with Key")]
    [SerializeField] private bool needsKey;
    
    [SerializeField] private int doorID;

    [Header("For Door with Gauge Only")]
    [SerializeField] private int gaugeReading;

    [Header("For Door with Keypad")]
    [SerializeField] private bool keypadUnlocked = false;
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
        if(opened)
        {
            return true;
        }
        
        Debug.Log("Trying to open door: " + doorID);
        PlayerController pc = other.GetComponent<PlayerController>();
        if (needsKey && !pc.HasKey(doorID))
        {
            //Locked; doesn't have key
            //Broadcast door failed open
            //EventManager.FailedOpenDoor
            
            return false;
        }
        if(doorType == DoorType.HasKeyPad && !keypadUnlocked)
        {
            //Door locked; keypad not unlocked
            //EventManager.FailedOpenDoor
            return false;
        }
        Debug.Log("Opened door: " + doorID);
        //EventManager.SuccessfullyOpenedDoor
        
        
        //Play door open animation
        opened = true;
        doorAnimator.SetTrigger("DoorOpen");
        SoundManager.sm.PlaySoundEffect(doorOpenSound, transform.position, false, true);
        //Destroy(transform.parent.gameObject);
        return true;
    }

    public void KeypadSuccess()
    {
        keypadUnlocked = true;
        
    }
}