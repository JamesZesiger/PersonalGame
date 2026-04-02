using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    [SerializeField] Transform model; // visual mesh
    [SerializeField] Transform cameraTransform;
    public FarmInteraction farmInteraction;
    [SerializeField] ToolManager toolManager;
    [SerializeField] UIManager uiManager;
    [SerializeField] Inventory playerInventory;

    [Header("Movement")]
    [SerializeField] float walkSpeed = 6f;
    [SerializeField] float sprintSpeed = 9f;
    [SerializeField] float acceleration = 14f;
    [SerializeField] float rotationSpeed = 10f;

    [Header("Jumping")]
    [SerializeField] float jumpHeight = 1.6f;
    [SerializeField] float gravity = -20f;

    [Header("Footstep Particles")]
    [SerializeField] GameObject footstepParticlePrefab;
    [SerializeField] Transform footPoint;
    [SerializeField] float spawnRate = 0.15f;

    CharacterController controller;

    Vector2 moveInput;
    Vector3 velocity;
    float verticalVelocity;

    bool isSprinting;
    bool isGrounded;

    float nextSpawnTime;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        // fallback if not assigned
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
        if (farmInteraction == null)
            farmInteraction = GetComponent<FarmInteraction>();

    }

    void Update()
    {
        GroundCheck();
        HandleMovement();
        ApplyGravity();
        HandleAnimations();
        HandleFootsteps();
    }

    // ---------------- MOVEMENT ----------------

    void HandleMovement()
    {
        // Camera-relative movement
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;

        float speed = isSprinting ? sprintSpeed : walkSpeed;

        Vector3 targetVelocity = moveDir * speed;

        velocity = Vector3.Lerp(
            velocity,
            targetVelocity,
            acceleration * Time.deltaTime
        );

        controller.Move(velocity * Time.deltaTime);

        // Smooth model rotation
        if (moveDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);

            model.rotation = Quaternion.Slerp(
                model.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // ---------------- GRAVITY ----------------

    void ApplyGravity()
    {
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 gravityMove = Vector3.up * verticalVelocity;
        controller.Move(gravityMove * Time.deltaTime);
    }

    void GroundCheck()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
    }

    // ---------------- JUMP ----------------

    void Jump()
    {
        if (!isGrounded) return;

        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        animator.SetTrigger("jump");
    }

    // ---------------- ANIMATIONS ----------------

    void HandleAnimations()
    {
        float speedPercent = velocity.magnitude / sprintSpeed;

        animator.SetFloat("speed", speedPercent, 0.1f, Time.deltaTime);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", verticalVelocity);
    }

    // ---------------- FOOTSTEPS ----------------

    void HandleFootsteps()
    {
        if (isGrounded && moveInput.magnitude > 0.1f)
        {
            if (Time.time >= nextSpawnTime)
            {
                GameObject particle = Instantiate(
                    footstepParticlePrefab,
                    footPoint.position,
                    Quaternion.identity
                );

                Destroy(particle, 0.5f);
                nextSpawnTime = Time.time + spawnRate;
            }
        }
    }

    // ---------------- INPUT ----------------

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
            Jump();
    }

    void OnUseTool(InputValue value)
    {
        if (value.isPressed)
            toolManager.OnUse();
    }
    void OnNext(InputValue value)
    {
        if (value.isPressed)
            toolManager.NextTool();
    }
    void OnInventory(InputValue value)
    {
        if (!value.isPressed) return;

        uiManager.TogglePlayerInventory(playerInventory);
    }

    void OnAltUseTool(InputValue value)
    {
        if (value.isPressed)
            toolManager.AltUse();
    }
    void OnAlt(InputValue value)
    {
        if (!Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false; 
        }
        


    }
}