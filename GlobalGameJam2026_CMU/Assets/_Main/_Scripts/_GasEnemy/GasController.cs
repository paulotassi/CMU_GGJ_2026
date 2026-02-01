using Unity.VisualScripting;
using UnityEngine;

public class GasController : MonoBehaviour
{
    [SerializeField] private float gasStrength = 0;
    [SerializeField] private float gasChangeRate = 0;
    [SerializeField] private int gasDamageValue = 1;
    [SerializeField] private ParticleSystem gasParticle;

    private SphereCollider gasTrigger; // assign the trigger collider here

    public void setGasDensity(float gasStrength)
    {
        if (gasParticle == null) return;

        ParticleSystem.EmissionModule emission = gasParticle.emission;
        emission.rateOverTime = gasStrength * 20;
    }

    public void setGasSize(float gasRadius)
    {
        if (gasParticle == null) return;

        if (gasTrigger == null)
        {
            gasTrigger = GetComponent<SphereCollider>();
            if (gasTrigger == null) return;
        }

        // Set collider radius (local space)
        gasTrigger.radius = gasRadius;

        // Match particle shape radius to collider radius (also local space)
        ParticleSystem.ShapeModule shape = gasParticle.shape;
        shape.radius = gasRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;
        Debug.Log("Player entered a Gas Zone");
        player.enterGasZone(gasDamageValue);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;
        Debug.Log("Player left a Gas Zone");
        player.exitGasZone();
    }
}
