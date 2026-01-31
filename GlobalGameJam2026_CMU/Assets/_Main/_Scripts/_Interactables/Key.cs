using UnityEngine;

public class Key : Interactable
{
    [SerializeField] private int keyID;
    [SerializeField] private Collider pickUpZone;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(pickUpZone)
    }

    public override void Interact(GameObject other = null)
    {
        Debug.Log("Picked up key: " + keyID);
        // Add key to player's inventory logic here

        // Destroy key object after pickup
        Destroy(gameObject);
    }
}
