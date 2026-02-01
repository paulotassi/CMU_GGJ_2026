// Commented by ChatGPT

using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraManager : MonoBehaviour
{
    #region References

    [SerializeField] private PlayerController playerController;          // Player script that provides movement + sprint input

    [Header("Footstep Audio")]
    [SerializeField] private AudioSource footstepSource;                 // AudioSource on the camera (or assigned)
    [SerializeField] private AudioClip[] footstepClips;                  // Random pool of footstep clips
    [SerializeField] private float footstepVolume = 0.9f;                // Volume of each step

    #endregion

    #region Bounce Settings

    [Header("Bounce Settings")]
    [SerializeField] private float walkBounceFrequency = 6f;             // Walk bounce speed
    [SerializeField] private float sprintBounceFrequency = 9f;           // Sprint bounce speed
    [SerializeField] private float walkBounceAmplitude = 0.05f;          // Walk bounce height
    [SerializeField] private float sprintBounceAmplitude = 0.07f;        // Sprint bounce height
    [SerializeField] private float returnSpeed = 12f;                    // How fast the camera returns to center when not moving

    [Header("Movement Threshold")]
    [SerializeField] private float moveThreshold = 0.1f;                 // How much move input counts as moving

    #endregion

    #region Heavy Footfall Settings

    [Header("Heavy Footfalls")]
    [SerializeField] private float impactDipAmount = 0.02f;              // Extra dip at the bottom of each step (heavier feeling)
    [SerializeField] private float impactDipSharpness = 8f;              // Higher = punchier/shorter impact dip

    [Header("Footstep Timing")]
    [SerializeField] private float dipTriggerThreshold = 0.95f;          // 0..1; higher = closer to the very bottom

    #endregion

    #region Fade To Black (On Death)

    [Header("Fade To Black")]
    [SerializeField] private UnityEngine.UI.Image fadeImage;   // Fullscreen black image
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f); // tweak in inspector

    private Coroutine fadeCoroutine;

    #endregion

    #region Internal State

    private Vector3 startLocalPos;                                       // Camera rest position (local)
    private float bounceTimer;                                           // Sine wave timer
    private bool stepArmed = true;                                       // Prevents multiple sounds per dip

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        startLocalPos = transform.localPosition;                         // Cache local position for stable bounce

        if (footstepSource == null)
        {
            footstepSource = GetComponent<AudioSource>();                // Try to auto-grab a source on the camera
        }

        if (playerController == null)
        {
            playerController = GetComponentInParent<PlayerController>(); // Common setup: camera is child of player
        }


    }

    private void OnEnable()
    {
        EventManager.playerDeath += OnPlayerDeath;
    }

    private void OnDisable()
    {
        EventManager.playerDeath -= OnPlayerDeath;
    }

    private void Update()
    {
        if (playerController == null)
        {
            return;
        }

        Vector2 moveInput = playerController.getMoveInput();
        float sprintInput = playerController.getSprintInput();

        float moveMagnitude = moveInput.magnitude;
        bool isMoving = moveMagnitude > moveThreshold;

        if (!isMoving)
        {
            returnToCenter();
            return;
        }

        applyBounce(moveMagnitude, sprintInput);
        tryFootstepAtDip();
    }

    #endregion

    #region Bounce Logic

    // Moves the camera up/down with a sine wave while the player is moving.
    // Sprint increases bounce frequency and amplitude so the movement feels faster/heavier.
    private void applyBounce(float moveMagnitude, float sprintInput)
    {
        float frequency = Mathf.Lerp(walkBounceFrequency, sprintBounceFrequency, sprintInput);
        float amplitude = Mathf.Lerp(walkBounceAmplitude, sprintBounceAmplitude, sprintInput);

        amplitude *= Mathf.Lerp(0.6f, 1.0f, Mathf.Clamp01(moveMagnitude));

        bounceTimer += Time.deltaTime * frequency;

        float sine = Mathf.Sin(bounceTimer);
        float baseYOffset = sine * amplitude;

        float dip01 = Mathf.InverseLerp(0f, -1f, sine);
        float impactShape = Mathf.Pow(dip01, impactDipSharpness);
        float impactYOffset = -impactShape * impactDipAmount;

        transform.localPosition = startLocalPos + new Vector3(0f, baseYOffset + impactYOffset, 0f);
    }

    #endregion

    #region Footstep Logic

    private void tryFootstepAtDip()
    {
        float sine = Mathf.Sin(bounceTimer);
        float dip01 = Mathf.InverseLerp(0f, -1f, sine);

        if (dip01 < 0.2f)
        {
            stepArmed = true;
        }

        if (stepArmed && dip01 >= dipTriggerThreshold)
        {
            playFootstep();
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

    #region Return To Center

    private void returnToCenter()
    {
        bounceTimer = 0f;
        stepArmed = true;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            startLocalPos,
            Time.deltaTime * returnSpeed
        );
    }

    #endregion

    #region Death Fade

    private void OnPlayerDeath(float deathEventDuration)
    {
        
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(fadeToBlack(deathEventDuration));
    }

    private IEnumerator fadeToBlack(float deathEventDuration)
    {
        if (fadeImage == null)
        {
            yield break;
        }

        float timer = 0f;

        Color color = fadeImage.color;  // keep RGB as-is (your image is already black)
        float startAlpha = color.a;

        while (timer < deathEventDuration * 0.8f)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / (deathEventDuration * 0.8f));
            float curvedT = fadeCurve.Evaluate(t); // non-linear time

            color.a = Mathf.Lerp(startAlpha, 1f, curvedT);
            fadeImage.color = color;

            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;

        fadeCoroutine = null;
    }




    #endregion
}
