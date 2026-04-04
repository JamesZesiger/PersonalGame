using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class GridPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float sprintSpeed = 9f;
    public float acceleration = 14f;
    public float rotationSpeed = 10f;
    public float gridSize = 1f;

    [Header("Jumping")]
    public float jumpHeight = 1.6f;
    public float gravity = -20f;



    private CharacterController _controller;
    private Vector3 _targetPosition;
    private Vector2 _moveInput;

    private bool _isMoving;
    private bool _isSprinting;
    public bool isGrounded;
    public float verticalVelocity;
    private Vector3 _bufferedDirection = Vector3.zero;
    private Vector3 _moveDirection;
    Vector3 _currentGridPosition;

    void Start()
    {
        _currentGridPosition = SnapToGrid(transform.position);
        transform.position = _currentGridPosition;
        _targetPosition = _currentGridPosition;
    }

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        //GroundCheck();
        HandleMovement();
       //ApplyGravity();
    }

    // INPUT SYSTEM
    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }
    void OnSprint(InputValue value)
    {
        _isSprinting = value.isPressed;
    }
    void OnJump(InputValue value)
    {
        if (value.isPressed)
            Jump();
    }

    void HandleMovement()
    {
        Vector3 direction = GetSnappedDirection(_moveInput);
        if (direction != Vector3.zero)
            _bufferedDirection = direction;

        if (!_isMoving && _bufferedDirection != Vector3.zero)
        {
            Vector3 snappedDir = _bufferedDirection * gridSize;
            _targetPosition = _currentGridPosition + snappedDir;
            _isMoving = true;

            _moveDirection = snappedDir.normalized;

            _bufferedDirection = Vector3.zero;
        }

        if (_moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        float speed = _isSprinting ? sprintSpeed : moveSpeed;
        transform.position = Vector3.MoveTowards(
            transform.position,
            _targetPosition,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, _targetPosition) <= speed * Time.deltaTime)
        {
            transform.position = _targetPosition;

            _currentGridPosition = _targetPosition;

            _isMoving = false;
        }
    }

    Vector3 GetDirection()
    {
        float x = _moveInput.x;
        float z = _moveInput.y;

        if (Mathf.Abs(x) < 0.1f) x = 0;
        if (Mathf.Abs(z) < 0.1f) z = 0;

        Vector3 dir = new Vector3(x, 0, z);
        return dir.normalized; 
    }

    Vector3 GetSnappedDirection(Vector2 input)
    {
        if (input.magnitude < 0.1f)
            return Vector3.zero;

        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;

        // Snap to 8 directions (every 45 degrees)
        float snappedAngle = Mathf.Round(angle / 45f) * 45f;
        float rad = snappedAngle * Mathf.Deg2Rad;

        Vector3 dir = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
        return dir.normalized;
}

    Vector3 SnapToGrid(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x / gridSize) * gridSize,
            pos.y,
            Mathf.Round(pos.z / gridSize) * gridSize
        );
    }

    void Jump()
    {
        if (!isGrounded) return;

        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void GroundCheck()
    {
        isGrounded = _controller.isGrounded;

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
    }

    void ApplyGravity()
    {
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 gravityMove = Vector3.up * verticalVelocity;
        _controller.Move(gravityMove * Time.deltaTime);
    }

    public Vector3 Velocity
    {
        get
        {
            if (!_isMoving) return Vector3.zero;
            float speed = _isSprinting ? sprintSpeed : moveSpeed;
            return _moveDirection * speed;
        }
    }
}