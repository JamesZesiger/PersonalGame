using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance { get; private set; }
    [Header("References")]
    [SerializeField] Transform cameraPivot;
    
    [Header("Mouse Look")]
    [SerializeField] float mouseSensitivity = 0.12f;
    [SerializeField] float verticalClamp = 80f;
    float cameraXRotation;
    public Camera cam;
    Vector2 lookInput;
    private bool isCameraLocked = false;


    void Awake()
    {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cam = Camera.main;

        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        HandleLook();
    }

    float yaw; // horizontal rotation

    void HandleLook()
    {
        if (isCameraLocked) return;
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        // Horizontal rotation (orbit around player)
        yaw += mouseX;

        // Vertical rotation (up/down)
        cameraXRotation -= mouseY;
        cameraXRotation = Mathf.Clamp(cameraXRotation, -verticalClamp, verticalClamp);

        // Apply rotations
        cameraPivot.localRotation = Quaternion.Euler(cameraXRotation, yaw, 0);
    }

    void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    public void ToggleCameraLock(bool lockState)
    {
        isCameraLocked = lockState;
    }

}
