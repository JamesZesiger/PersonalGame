using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform cameraPivot;
    
    [Header("Mouse Look")]
    [SerializeField] float mouseSensitivity = 0.12f;
    [SerializeField] float verticalClamp = 80f;
    float cameraXRotation;
    public Camera cam;
    Vector2 lookInput;


    void Awake()
    {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cam = Camera.main;
    }

    void Update()
    {
        HandleLook();
    }

    void HandleLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        cameraXRotation -= mouseY;
        cameraXRotation = Mathf.Clamp(cameraXRotation, -verticalClamp, verticalClamp);

        cameraPivot.localRotation = Quaternion.Euler(cameraXRotation, 0, 0);
    }

    void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

}
