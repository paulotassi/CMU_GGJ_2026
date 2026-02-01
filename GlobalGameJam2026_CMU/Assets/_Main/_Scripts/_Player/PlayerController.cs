// Commented by ChatGPT

using System.Collections.Generic;
using System.Numerics;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using NavKeypad;

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
    public float controllerSensitivity = 175f;             // Controller look sensitivity (scaled)
    private float verticalVelocity;
    [SerializeField] private LayerMask interactMask;


    [Header("Mask Info")]
    public int maxHeldMasks = 1000;
    public int currMaskAmount = 1;

    [Header("Key Info")]
    public int totalKeys = 0;
    public int maxHeldKeys = 3;

    #endregion

    #region Health / Mask / Gas
    [Header("Health Info")]
    public int startingPlayerHealth = 100;                 // Initial player health
    public int startingGasMaskHealth = 100;                // Initial gas mask durability

    public int playerHealth { get; private set; }          // Current player health

    public bool playerAlive = true;
    public int gasMaskHealth { get; private set; }         // Current gas mask health

    [SerializeField] private AudioClip deathSound;         // Death Sound

    public GasMaskEquip gmEquipAnim;

    public bool maskEquipped { get; private set; }         // Whether the gas mask is currently equipped
    public AudioSource breathingSound;

    [SerializeField] private float gasDamageIntervalSeconds = 1.0f;    // Damage Interval

    private int gasZoneCount = 0;                                      // How many gas triggers we're inside (prevents overlap bugs)
    private Coroutine gasDamageCoroutine;

    [SerializeField] private GameObject dotReticle;   // UI Image or GameObject
    private KeypadButton currentKeypadButton;         // What we're aiming at

    #endregion

    #region Internal State

    private float pitch;                                   // Vertical camera rotation (clamped)
    private Camera playerCamera;                           // Reference to FPS camera
    private CharacterController characterController;       // CharacterController for collision-based movement

    private int[] keyInventory;

    private bool maskAnimation = false;

    private InteractableZone currInteractableZone = null;

    #endregion

    #region Unity Lifecycle

    // Initializes input system, camera reference, cursor state, and vitals
    void Awake()
    {
        playerInput = new PlayerInputAction();              // Instantiate input wrapper
        playerCamera = GetComponentInChildren<Camera>();    // Locate child camera
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;           // Lock cursor to center
        Cursor.visible = false;                             // Hide cursor

        playerHealth = startingPlayerHealth;                // Initialize player health
        gasMaskHealth = startingGasMaskHealth;              // Initialize mask health
        maskEquipped = false;                               // Mask starts unequipped
        
        keyInventory = new int[maxHeldKeys];                // Initialize key inventory

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
            Debug.Log("Interact Pressed");
            HandleInteract();                              // Process interaction
            interactPressedThisFrame = false;              // Consume flag
        }

        if (attackPressedThisFrame && !maskAnimation)
        {
            ToggleMaskEquip();                             // Equip / unequip mask
            attackPressedThisFrame = false;                // Consume flag
            //Eventually needs cooldown
            
            
        }

        UpdateInteractRaycast();
        //Test Input logic remove before finalization
        if (playerInput.Player.Next.ReadValue<float>() > 0)
        {
            testInput();
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

    #region General Interactable Management

    public void setInteractableZone(InteractableZone iz)
    {
        currInteractableZone = iz;
    }
    public void removeInteractableZone()
    {
        setInteractableZone(null);
    }

    #endregion

    #region Mask Management

    // Toggles gas mask equip state (cannot equip if broken)
    private void ToggleMaskEquip()
    {
        // If it's currently equipped, we are always allowed to UNEQUIP.
        if (maskEquipped)
        {
            maskEquipped = false;                 // Unequip
            gmEquipAnim.UnequipMask();            // Play unequip anim
            if (breathingSound.isPlaying) breathingSound.Stop();
            EventManager.CurrentMaskHealth(gasMaskHealth);
            return;
        }

        // If it's NOT equipped, only allow EQUIP if player has a mask and it isn't broken.
        bool hasMaskAvailable = currMaskAmount > 0;
        bool maskIsUsable = gasMaskHealth > 0;

        if (!hasMaskAvailable || !maskIsUsable)
        {
            return;                               // Can't equip
        }

        maskEquipped = true;                      // Equip
        gmEquipAnim.EquipMask();                  // Play equip anim
        breathingSound.Play();

        EventManager.CurrentMaskHealth(gasMaskHealth);
    }


    public bool addMask()
    {
        if(currMaskAmount >= maxHeldMasks)
        {
            Debug.Log("Cannot pick up more gas masks");
            return false;
        }
        Debug.Log("Picked up gas mask");
        currMaskAmount++;
        return true;
    }

    public IEnumerator MaskAnimation()
    {
        if (maskEquipped)
        {
            //Play Animation
            //EventManager.EquipMask();
            yield return new WaitForSeconds(0.5f); //Equip Animation time
        }
        else
        {
            //Play Animation
            //EventManager.DequipMask();
            yield return new WaitForSeconds(0.5f); //Dequip Animation time
        }
        maskAnimation = false;

    }


    #endregion

    #region Key Management

    public void AddKeyToInventory(int keyID)
    {
        keyInventory[keyID]++;
    }

    public bool HasKey(int keyID)
    {
        return keyInventory[keyID] > 0;
    }

    public void UseKey(int keyID)
    {
        if (HasKey(keyID))
        {
            keyInventory[keyID]--;
        }
    }

    public int GetCurrKeys()
    {
        int count = 0;
        foreach (int keyCount in keyInventory)
        {
            count += keyCount;
        }
        return count;
    }
    #endregion

    #region Interaction

    // Performs a forward raycast from the camera to detect interactable objects
    void HandleInteract()
    {
        if (currentKeypadButton != null)
        {
            currentKeypadButton.PressButton();
            return;
        }

        if (playerCamera == null)
        {
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask))
        {
            if (hit.collider.CompareTag("Door"))
            {
                EventManager.TextTrigger("It can't be opened from this side.");
                return;
            }

            if (hit.collider.TryGetComponent(out InteractableZone interactableZone))
            {
                if (!interactableZone.interactPressed(gameObject))
                {
                    Debug.Log("Door was locked or too many masks");
                }
                else
                {
                    Debug.Log("Successful Interaction");
                }
            }
        }

    }
        private void UpdateInteractRaycast()
    {
        currentKeypadButton = null;
        dotReticle.SetActive(false);

        if (playerCamera == null)
        {
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask))
        {
            if (hit.collider.TryGetComponent(out KeypadButton keypadButton))
            {
                currentKeypadButton = keypadButton;
                dotReticle.SetActive(true);
            }
        }
    }


    #endregion

    #region Movement

    public Vector2 getMoveInput()
    {
        return moveInput;
    }

    public float getSprintInput()
    {
        return sprintInput;
    }
    // Moves the player relative to their facing direction
    void HandleMove()
    {
        Vector3 move =
            (transform.right * moveInput.x) +
            (transform.forward * moveInput.y);

        float speed = sprintInput > 0
            ? moveSpeed * sprintSpeed
            : moveSpeed;

        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += Physics.gravity.y * Time.deltaTime;

        Vector3 velocity = (move * speed) + Vector3.up * verticalVelocity;

        characterController.Move(velocity * Time.deltaTime);

        //Set player breathing speed Audio
    }

    #endregion

    #region Look

    // Handles mouse / controller camera rotation
    void HandleLook()
    {
        bool usingGamepad =
            Gamepad.current != null &&
            Gamepad.current.wasUpdatedThisFrame;

        float sensitivity =
            usingGamepad ? controllerSensitivity
                          : mouseSensitivity;

        float yaw = lookInput.x * sensitivity;
        float lookY = lookInput.y * sensitivity;

        if (usingGamepad)
        {
            yaw *= Time.deltaTime;
            lookY *= Time.deltaTime;
        }

        pitch -= lookY;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        playerCamera.transform.localRotation =
            Quaternion.Euler(pitch, 0f, 0f);

        transform.Rotate(Vector3.up * yaw);
    }

    #endregion

    #region Damage Routing

    public void takeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }

        if (maskEquipped && gasMaskHealth > 0)
        {
            ApplyDamageToMaskThenSpillover(damage);
            return;
        }

        ApplyDamageToPlayer(damage);
    }

    private void ApplyDamageToMaskThenSpillover(int damage)
    {
        int maskBefore = gasMaskHealth;

        gasMaskHealth -= damage;

        // Notify listeners that mask health changed
        Debug.Log("I am sending a message to Event for other listners");
        EventManager.CurrentMaskHealth(gasMaskHealth);

        if (gasMaskHealth > 0)
        {
            return;
        }

        gasMaskHealth = 0;

        // Notify again after clamping to zero
        EventManager.CurrentMaskHealth(gasMaskHealth);

        maskBroke();

        int leftoverDamage = damage - maskBefore;

        if (leftoverDamage > 0)
        {
            ApplyDamageToPlayer(leftoverDamage);
        }
    }

    private void ApplyDamageToPlayer(int damage)
    {
        
        if (playerHealth > 1)
        {
            playerAlive = true;
            playerHealth -= damage;

            // Add cough sounds here

            if (playerHealth < 0)
            {
                
                playerHealth = 0;
            }
        }

        if (playerHealth <= 0 && playerAlive)
        {
            playerAlive=false;
            playerDied();
        }
    }

    #endregion

    #region Gas Damage Toggle

    public void enterGasZone(int gasDamage)
    {
        gasZoneCount++;

        if (gasDamageCoroutine == null)
        {
            gasDamageCoroutine = StartCoroutine(gasDamageLoop(gasDamage));
        }
    }

    // Called by gas zones on exit
    public void exitGasZone()
    {
        gasZoneCount = Mathf.Max(0, gasZoneCount - 1);

        if (gasZoneCount == 0 && gasDamageCoroutine != null)
        {
            StopCoroutine(gasDamageCoroutine);
            gasDamageCoroutine = null;
        }
    }

    private IEnumerator gasDamageLoop(int gasDamage)
    {
        while (gasZoneCount > 0)
        {
            Debug.Log("Player is about to take Damage");
            takeDamage(gasDamage); // Routes through mask/player logic you already have
            yield return new WaitForSeconds(gasDamageIntervalSeconds);
        }

        gasDamageCoroutine = null;
    }

    #endregion

    #region Death / Mask Break

    private void playerDied()
    {
        SoundManager.sm.PlaySoundEffect(deathSound, transform.position, false, false);
        EventManager.PlayerDeath(5);
        playerInput.Player.Move.Disable();
        playerInput.Player.Look.Disable();
    }

    private void maskBroke()
    {
        ToggleMaskEquip();
        if(currMaskAmount > 0)
        {
            currMaskAmount--; 
        }

    }

    #endregion

    #region DebugTools

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

    #if UNITY_EDITOR
    private void testInput()
    {
        ApplyDamageToPlayer(25);
    }
    #endif

    #endregion
}
