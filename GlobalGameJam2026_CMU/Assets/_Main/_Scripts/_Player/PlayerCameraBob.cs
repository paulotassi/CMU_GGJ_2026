// Commented by ChatGPT

using UnityEngine;

public class PlayerCameraBob : MonoBehaviour
{
    #region References

    [SerializeField] private PlayerController playerController;          // Reference to the player controller (source of movement + sprint)

    [Header("Footstep Audio")]
    [SerializeField] private AudioSource footstepSource;                 // AudioSource on the camera (or assigned)
    [SerializeField] private AudioClip[] footstepClips;                  // Random pool of footstep clips
    [SerializeField] private float footstepVolume = 0.9f;                // Volume of each step

    #endregion

    #region Bob Settings

    [Header("Bob Settings")]
    [SerializeField] private float walkBobFrequency = 6f;                // Walk bob speed (cycles per second-ish)
    [SerializeField] private float sprintBobFrequency = 9f;              // Sprint bob speed
    [SerializeField] private float walkBobAmplitude = 0.05f;             // Walk bob height
    [SerializeField] private float sprintBobAmplitude = 0.07f;           // Sprint bob height
    [SerializeField] private float returnSpeed = 12f;                    // How fast the camera returns to center when not moving

    [Header("Movement Threshold")]
    [SerializeField] private float moveThreshold = 0.1f;                 // How much input magnitude counts as moving

    #endregion

    #region Heavy Footfall Settings

    [Header("Heavy Footfalls")]
    [SerializeField] private float impactDipAmount = 0.02f;              // Extra dip applied at the bottom of each step (feels heavier)
    [SerializeField] private float impactDipSharpness = 8f;              // How tight the impact is (higher = punchier, shorter)
    [SerializeField] private float impactKickDegrees = 0.35f;            // Tiny rotational kick on impact (0 to disable)
    [SerializeField] private float impactKickReturnSpeed = 18f;          // How fast the kick rotation returns

    [Header("Footstep Timing")]
    [SerializeField] private float dipTriggerThreshold = 0.95f;          // 0..1; higher = closer to the very bottom

    #endregion

    #region Internal State

    private Vector3 startLocalPos;                                       // Camera rest position (local)
    private float bobTimer;                                              // Sine wave timer
    private bool stepArmed = true;                                       // Gate to prevent multiple sounds per dip

    private Quaternion startLocalRot;                                    // Camera rest rotation (local)
    private float currentKick;                                           // Current kick rotation amount

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        startLocalPos = transform.localPosition;                         // Cache local position for stable bob
        startLocalRot = transform.localRotation;                         // Cache local rotation for stable kick

        if (footstepSource == null)
        {
            footstepSource = GetComponent<AudioSource>();                // Try to auto-grab a source on the camera
        }

        if (playerController == null)
        {
            playerController = GetComponentInParent<PlayerController>(); // Common setup: camera is child of player
        }
    }

    private void Update()
    {
        if (playerController == null)
        {
            return;
        }

        Vector2 moveInput = playerController.getMoveInput();             // Read move input from controller
        float sprintInput = playerController.getSprintInput();           // Read sprint value from controller

        float moveMagnitude = moveInput.magnitude;                       // 0..1-ish depending on bindings
        bool isMoving = moveMagnitude > moveThreshold;

        if (!isMoving)
        {
            returnToCenter();
            return;
        }

        applyBob(moveMagnitude, sprintInput);
        tryFootstepAtDip();
        applyImpactKickReturn();
    }

    #endregion

    #region Bob Logic

    private void applyBob(float moveMagnitude, float sprintInput)
    {
        // Blend between walk and sprint settings based on sprint input (0 = walk, 1 = sprint)
        float frequency = Mathf.Lerp(walkBobFrequency, sprintBobFrequency, sprintInput);
        float amplitude = Mathf.Lerp(walkBobAmplitude, sprintBobAmplitude, sprintInput);

        // Increase bob slightly if the stick is fully pushed (feels more responsive)
        amplitude *= Mathf.Lerp(0.6f, 1.0f, Mathf.Clamp01(moveMagnitude));

        bobTimer += Time.deltaTime * frequency;

        float sine = Mathf.Sin(bobTimer);                                // -1..1
        float baseYOffset = sine * amplitude;                            // Normal bob up/down

        // "Heavy footfall" extra dip only near the bottom of the step (sine near -1)
        float dip01 = Mathf.InverseLerp(0f, -1f, sine);                   // 0 away from dip, 1 at dip
        float impactShape = Mathf.Pow(dip01, impactDipSharpness);         // Makes the dip sharp/punchy
        float impactYOffset = -impactShape * impactDipAmount;             // Extra downward pulse

        Vector3 targetPos = startLocalPos + new Vector3(0f, baseYOffset + impactYOffset, 0f);
        transform.localPosition = targetPos;
    }

    #endregion

    #region Footstep Logic

    private void tryFootstepAtDip()
    {
        float sine = Mathf.Sin(bobTimer);
        float dip01 = Mathf.InverseLerp(0f, -1f, sine);                   // 0..1 (1 near bottom)

        // Rearm once we've lifted away from the dip
        if (dip01 < 0.2f)
        {
            stepArmed = true;
        }

        if (stepArmed && dip01 >= dipTriggerThreshold)
        {
            playFootstep();
            applyImpactKick();                                            // Optional: tiny camera kick on impact
            stepArmed = false;
        }
    }

    private void playFootstep()
    {
        if (footstepSource == null) return;
        if (footstepClips == null || footstepClips.Length == 0) return;

        int index = Random.Range(0, footstepClips.Length);
        AudioClip clip = footstepClips[index];
        if (clip == null) return;

        footstepSource.PlayOneShot(clip, footstepVolume);
    }

    #endregion

    #region Kick Logic (Optional)

    private void applyImpactKick()
    {
        if (impactKickDegrees <= 0f)
        {
            return;
        }

        // A small downward pitch kick feels like "weight" at impact.
        currentKick = -impactKickDegrees;
        transform.localRotation = startLocalRot * Quaternion.Euler(currentKick, 0f, 0f);
    }

    private void applyImpactKickReturn()
    {
        if (impactKickDegrees <= 0f)
        {
            return;
        }

        // Smoothly return kick back to 0 over time
        currentKick = Mathf.Lerp(currentKick, 0f, Time.deltaTime * impactKickReturnSpeed);
        transform.localRotation = startLocalRot * Quaternion.Euler(currentKick, 0f, 0f);
    }

    #endregion

    #region Return To Center

    private void returnToCenter()
    {
        bobTimer = 0f;
        stepArmed = true;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            startLocalPos,
            Time.deltaTime * returnSpeed
        );

        currentKick = Mathf.Lerp(currentKick, 0f, Time.deltaTime * impactKickReturnSpeed);
        transform.localRotation = startLocalRot * Quaternion.Euler(currentKick, 0f, 0f);
    }

    #endregion
}
