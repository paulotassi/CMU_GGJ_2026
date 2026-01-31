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
    public override bool Interact(GameObject other)
    {
        
        // Add gas mask to player's inventory logic here
        if(!other.GetComponent<PlayerController>().addMask())
        {
            Debug.Log("Cannot pick up more gas masks");
            //EventManatger.FailedGasMask();
            return false;
        }
        else
        {
            //EventManager.PickedUpGasMask();
            Destroy(transform.parent.gameObject);
            return true;
        }
        // Destroy gas mask object after pickup

    }
}
