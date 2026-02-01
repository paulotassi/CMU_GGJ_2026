using UnityEngine;

public class CreditCameraBounce : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float bounceIntensity = 0.05f;   // How far the camera moves
    public float bounceSpeed = 8f;           // How fast it bounces

    [Header("Control")]
    public bool enableBounce = true;         // Toggle bounce on/off

    Vector3 startLocalPosition;
    float time;

    void Awake()
    {
        startLocalPosition = transform.localPosition;
    }

    void LateUpdate()
    {
        if (!enableBounce)
        {
            transform.localPosition = startLocalPosition;
            return;
        }

        time += Time.deltaTime * bounceSpeed;

        float yOffset = Mathf.Sin(time) * bounceIntensity;

        transform.localPosition = startLocalPosition + new Vector3(0f, yOffset, 0f);
    }

    // Call this to dynamically scale intensity (ex: sprinting, damage, fear)
    public void SetBounceIntensity(float intensity)
    {
        bounceIntensity = intensity;
    }
}
