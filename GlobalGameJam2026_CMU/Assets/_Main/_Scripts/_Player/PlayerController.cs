// Commented by ChatGPT

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Input System Fields

    private PlayerInputAction playerInput;                 // Generated input wrapper from Unity Input System

    private Vector2 moveInput;                             // 2D movement input (x = strafe, y = forward)
    private Vector2 lookInput;                             // 2D look input (x = yaw, y = pitch)
    private float sprintInput;                             // Sprint value (button or trigger)

    private bool interactPressedThisFrame;                 // One-frame flag for interact input
    private bool attackPressedThisFrame;                   // One-frame flag for attack (mask toggle)

    #endregion

    #region Tunable Settings

    public float moveSpeed = 5f;                           // Base movement speed
    public float sprintSpeed = 2f;                         // Sprint speed multiplier
    public float interactDistance = 3f;                    // Raycast distance for interactions

    public float mouseSensitivity = 0.1f;                  // Mouse look sensitivity (raw delta)
    public float controllerSensitivity = 120f;             // Controller look sensitivity (scaled)

    #endregion

    #region Health / Vitals

    public int startingPlayerHealth = 100;                 // Initial player health
    public int startingGasMaskHealth = 100;                // Initial gas mask durability

    public int playerHealth { get; private set; }          // Current player health
    public int gasMaskHealth { get; private set; }         // Current gas mask health

    public bool maskEquipped { get; private set; }         // Whether the gas mask is currently equipped

    #endregion

    #region Internal State

    private float pitch;                                   // Vertical camera rotation (clamped)
    private Camera playerCamera;                           // Reference to FPS camera

    #endregion

    #region Unity Lifecycle

    // Initializes input system, camera reference, cursor state, and vitals
    void Awake()
    {
        playerInput = new PlayerInputAction();              // Instantiate input wrapper
        playerCamera = GetComponentInChildren<Camera>();    // Locate child camera

        Cursor.lockState = CursorLockMode.Locked;           // Lock cursor to center
        Cursor.visible = false;                             // Hide cursor

        playerHealth = startingPlayerHealth;                // Initialize player health
        gasMaskHealth = startingGasMaskHealth;              // Initialize mask health
        maskEquipped = false;                               // Mask starts unequipped
    }

    // Enables input map and subscribes to input events
    void OnEnable()
    {
        playerInput.Player.Enable();                        // Enable Player action map

        playerInput.Player.Interact.started += OnInteractPerformed; // Subscribe to interact press
        playerInput.Player.Attack.started += OnAttackPerformed;     // Subscribe to attack press
    }

    // Disables input map and unsubscribes from input events
    void OnDisable()
    {
        playerInput.Player.Interact.started -= OnInteractPerformed;
        playerInput.Player.Attack.started -= OnAttackPerformed;

        playerInput.Player.Disable();                       // Disable Player action map
    }

    // Polls continuous input and consumes one-frame actions
    void Update()
    {
        moveInput = playerInput.Player.Move.ReadValue<Vector2>();   // Read movement vector
        lookInput = playerInput.Player.Look.ReadValue<Vector2>();   // Read look vector
        sprintInput = playerInput.Player.Sprint.ReadValue<float>(); // Read sprint input

        HandleMove();                                     // Apply movement
        HandleLook();                                     // Apply camera rotation

        if (interactPressedThisFrame)
        {
            HandleInteract();                              // Process interaction
            interactPressedThisFrame = false;              // Consume flag
        }

        if (attackPressedThisFrame)
        {
            ToggleMaskEquip();                             // Equip / unequip mask
            attackPressedThisFrame = false;                // Consume flag
        }
    }

    #endregion

    #region Input Events

    // Sets interact flag when Interact action is pressed
    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        interactPressedThisFrame = true;
    }

    // Sets attack flag when Attack action is pressed
    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        attackPressedThisFrame = true;
    }

    #endregion

    #region Mask Equip

    // Toggles gas mask equip state (cannot equip if broken)
    private void ToggleMaskEquip()
    {
        if (!maskEquipped && gasMaskHealth <= 0)            // Prevent equipping broken mask
        {
            return;
        }

        maskEquipped = !maskEquipped;                       // Toggle state
    }

    #endregion

    #region Interaction

    // Performs a forward raycast from the camera to detect interactable objects
    void HandleInteract()
    {
        if (playerCamera == null)                           // Safety check
        {
            return;
        }

        // Creates a ray from the exact center of the screen (crosshair)
        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f));                   // Viewport space: (0,0)=bottom-left, (1,1)=top-right

        // Fires ray forward and stores hit info if something is hit within distance
        if (!Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            return;
        }

        // Attempts to retrieve Interactable component from hit object
        if (!hit.collider.TryGetComponent(out Interactable interactable))
        {
            return;
        }

        if (interactable.isGrabbable())
        {
            //Need to add logic where player discovers the type of object and interacts whichever way it needs to be interacted.
            //MAX
            //Yup right here
            //Hopefully you see this comment
            Debug.Log("I can grab this item!... well I just tried to");
        }
    }

    #endregion

    #region Movement

    // Moves the player relative to their facing direction
    void HandleMove()
    {
        Vector3 move =
            (transform.right * moveInput.x) +               // Left / right movement
            (transform.forward * moveInput.y);              // Forward / backward movement

        float speed = sprintInput > 0
            ? moveSpeed * sprintSpeed                       // Sprinting
            : moveSpeed;                                    // Walking

        transform.position += move * speed * Time.deltaTime; // Frame-rate independent movement
    }

    #endregion

    #region Look

    // Handles mouse / controller camera rotation
    void HandleLook()
    {
        bool usingGamepad =
            Gamepad.current != null &&
            Gamepad.current.wasUpdatedThisFrame;             // Detect active gamepad input this frame

        float sensitivity =
            usingGamepad ? controllerSensitivity             // Higher base sensitivity for sticks
                          : mouseSensitivity;                // Lower sensitivity for mouse deltas

        float yaw = lookInput.x * sensitivity;               // Horizontal rotation
        float lookY = lookInput.y * sensitivity;             // Vertical rotation

        if (usingGamepad)
        {
            yaw *= Time.deltaTime;                           // Normalize stick input
            lookY *= Time.deltaTime;
        }

        pitch -= lookY;                                      // Invert vertical look
        pitch = Mathf.Clamp(pitch, -89f, 89f);               // Prevent camera flip

        playerCamera.transform.localRotation =
            Quaternion.Euler(pitch, 0f, 0f);                 // Apply pitch to camera only

        transform.Rotate(Vector3.up * yaw);                  // Apply yaw to player body
    }

    #endregion

    #region Damage Routing

    // Entry point for damage that should respect gas mask shielding
    public void takeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }

        if (maskEquipped && gasMaskHealth > 0)               // Mask absorbs damage first
        {
            ApplyDamageToMaskThenSpillover(damage);
            return;
        }

        ApplyDamageToPlayer(damage);                          // Direct damage
    }

    // Applies damage to mask, then spills remaining damage to player if mask breaks
    private void ApplyDamageToMaskThenSpillover(int damage)
    {
        int maskBefore = gasMaskHealth;                      // Store pre-damage mask health

        gasMaskHealth -= damage;                             // Apply damage to mask

        if (gasMaskHealth > 0)
        {
            return;                                          // Mask absorbed everything
        }

        gasMaskHealth = 0;
        maskBroke();

        int leftoverDamage = damage - maskBefore;            // Remaining damage after mask depletion

        if (leftoverDamage > 0)
        {
            ApplyDamageToPlayer(leftoverDamage);
        }
    }

    // Applies damage directly to player health
    private void ApplyDamageToPlayer(int damage)
    {
        if (playerHealth > 1)
        {
            playerHealth -= damage;

            if (playerHealth < 0)
            {
                playerHealth = 0;
            }

            //EventManager.PlayerTookDamage();
        }

        if (playerHealth <= 0)
        {
            playerDied();
        }
    }

    #endregion

    #region Death / Mask Break

    // Handles player death
    private void playerDied()
    {
        //EventManager.PlayerDied();
        
    }

    // Handles gas mask destruction
    private void maskBroke()
    {
        if (maskEquipped)
        {
            maskEquipped = false;                            // Force unequip broken mask
        }

        //EventManager.EquippedMaskBroke();
    }

    #endregion

    #region Gizmos

    // Draws interaction ray in Scene view for debugging
    void OnDrawGizmos()
    {
        if (!Application.isPlaying || playerCamera == null)
        {
            return;
        }

        Gizmos.color = Color.red;

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f));

        Gizmos.DrawLine(
            ray.origin,
            ray.origin + ray.direction * interactDistance);
    }

    #endregion
}
