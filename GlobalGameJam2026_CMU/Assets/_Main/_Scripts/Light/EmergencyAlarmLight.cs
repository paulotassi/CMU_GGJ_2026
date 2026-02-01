using UnityEngine;

// emergency rotating / blinking light for submarine alarm stuff
// put this on the same object as the spotlight

[DisallowMultipleComponent]
public class EmergencyAlarmLight : MonoBehaviour
{
    public enum PulseMode
    {
        None,
        Sine,   // smooth pulse
        Blink   // hard on/off
    }

    [Header("Light")]
    public Light alarmLight;

    [Header("Rotation")]
    public bool rotate = true;
    public Vector3 rotationAxis = Vector3.up;
    // degrees per second
    public float rotationSpeed = 180f;

    [Header("Intensity")]
    public float baseIntensity = 6f;

    [Header("Pulse")]
    public PulseMode pulseMode = PulseMode.Sine;
    // how fast it pulses
    public float pulseRate = 1.5f;
    // how strong the pulse is
    [Range(0f, 1f)] public float pulseAmount = 0.6f;
    // only used for Blink mode
    [Range(0.05f, 0.95f)] public float blinkDuty = 0.5f;

    [Header("Flicker / Glitch")]
    // small random wobble so it doesn't feel perfect
    [Range(0f, 1f)] public float microFlickerAmount = 0.08f;
    public float microFlickerSpeed = 12f;

    // chance per second for a glitch
    [Range(0f, 5f)] public float glitchChancePerSecond = 0.15f;
    // how long a glitch lasts
    public Vector2 glitchDuration = new Vector2(0.05f, 0.2f);
    // rotation speed multiplier during glitch
    public Vector2 glitchRotationMultiplier = new Vector2(-0.5f, 2.5f);
    // intensity multiplier during glitch
    public Vector2 glitchIntensityMultiplier = new Vector2(0.2f, 1.3f);

    float t;
    float glitchTimer;
    float glitchRotMul = 1f;
    float glitchIntMul = 1f;

    void Reset()
    {
        // auto grab the light if possible
        alarmLight = GetComponent<Light>();
    }

    void Awake()
    {
        if (!alarmLight)
            alarmLight = GetComponent<Light>();
    }

    void Update()
    {
        if (!alarmLight) return;

        t += Time.deltaTime;

        HandleGlitch();

        // rotate the light like an alarm beacon
        if (rotate)
        {
            float rot = rotationSpeed * glitchRotMul * Time.deltaTime;
            transform.Rotate(rotationAxis.normalized, rot, Space.Self);
        }

        float intensity = baseIntensity;

        // pulse behavior
        if (pulseMode == PulseMode.Sine)
        {
            float wave = 0.5f + 0.5f * Mathf.Sin(t * Mathf.PI * 2f * pulseRate);
            float pulse = Mathf.Lerp(1f - pulseAmount, 1f, wave);
            intensity *= pulse;
        }
        else if (pulseMode == PulseMode.Blink)
        {
            float cycle = Mathf.Repeat(t * pulseRate, 1f);
            intensity *= (cycle < blinkDuty) ? 1f : (1f - pulseAmount);
        }

        // small flicker so it feels unstable
        if (microFlickerAmount > 0f)
        {
            float n = Mathf.PerlinNoise(t * microFlickerSpeed, 0.13f);
            float flick = Mathf.Lerp(1f - microFlickerAmount, 1f + microFlickerAmount, n);
            intensity *= flick;
        }

        intensity *= glitchIntMul;

        alarmLight.intensity = Mathf.Max(0f, intensity);
    }

    void HandleGlitch()
    {
        // already glitching
        if (glitchTimer > 0f)
        {
            glitchTimer -= Time.deltaTime;

            if (glitchTimer <= 0f)
            {
                glitchRotMul = 1f;
                glitchIntMul = 1f;
            }

            return;
        }

        // roll chance every frame
        float chanceThisFrame = glitchChancePerSecond * Time.deltaTime;
        if (Random.value < chanceThisFrame)
        {
            glitchTimer = Random.Range(glitchDuration.x, glitchDuration.y);
            glitchRotMul = Random.Range(glitchRotationMultiplier.x, glitchRotationMultiplier.y);
            glitchIntMul = Random.Range(glitchIntensityMultiplier.x, glitchIntensityMultiplier.y);
        }
    }
}
