using UnityEngine;

public class Key : Interactable
{
    [SerializeField] private int keyID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(pickUpZone)
    }

    public override bool Interact(GameObject other)
    {
        Debug.Log("Picked up key: " + keyID);
        // Add key to player's inventory logic here
        
        other.GetComponent<PlayerController>().AddKeyToInventory(keyID);
        // Destroy key object after pickup
        Destroy(transform.parent.gameObject);
        return true;
    }
}
