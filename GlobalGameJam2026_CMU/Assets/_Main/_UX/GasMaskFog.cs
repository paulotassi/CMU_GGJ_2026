using System;
using UnityEngine;

public class GasMaskFog : MonoBehaviour
{
    [SerializeField] private MeshRenderer glassRenderer;

    
    /// <summary>
    /// Just for testing
    /// </summary>
    private void Start()
    {
        SetBreathingSpeed(2f);
    }

    /// <summary>
    /// Sets the frequency at which the glass fogs up. Stay within a range of 0-1
    /// </summary>
    /// <param name="newBreathingSpeed"></param>
    public void SetBreathingSpeed(float newBreathingSpeed)
    {
        glassRenderer.material.SetFloat("_BreathingSpeed", newBreathingSpeed);
    }
}
