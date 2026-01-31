using UnityEngine;

public class GasMask : Interactable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Interact(GameObject other = null)
    {
        Debug.Log("Picked up Gas Mask");
        // Add gas mask to player's inventory logic here

        // Destroy gas mask object after pickup
        Destroy(gameObject);
    }
}
