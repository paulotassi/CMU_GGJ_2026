using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInputAction playerInput;

    Vector2 moveInput;
    Vector2 lookInput;

    public float moveSpeed = 5f;

    public float mouseSensitivity = 0.1f;
    public float controllerSensitivity = 120f;

    float pitch;

    Camera playerCamera;

    // Cache references and create the input wrapper
    void Awake()
    {
        playerInput = new PlayerInputAction();
        playerCamera = GetComponentInChildren<Camera>();

        // Cursor behavior for FPS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Enable action map
    void OnEnable()
    {
        playerInput.Player.Enable();
    }

    // Disable action map
    void OnDisable()
    {
        playerInput.Player.Disable();
    }

    // Read inputs and apply movement/look
    void Update()
    {
        moveInput = playerInput.Player.Move.ReadValue<Vector2>();
        lookInput = playerInput.Player.Look.ReadValue<Vector2>();

        HandleMove();
        HandleLook();
    }

    // Move in local space (forward/back/left/right)
    void HandleMove()
    {
        Vector3 move =
            (transform.right * moveInput.x) +
            (transform.forward * moveInput.y);

        transform.position += move * moveSpeed * Time.deltaTime;
    }

    // Yaw on player, pitch on camera
    void HandleLook()
    {
        bool usingGamepad = Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame;

        float sensitivity = usingGamepad ? controllerSensitivity : mouseSensitivity;

        float yaw = lookInput.x * sensitivity;
        float lookY = lookInput.y * sensitivity;

        if (usingGamepad)
        {
            yaw *= Time.deltaTime;
            lookY *= Time.deltaTime;
        }

        pitch -= lookY;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        transform.Rotate(Vector3.up * yaw);
    }
}
