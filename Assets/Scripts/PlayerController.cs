using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    

    [Header("Movement")]
    [SerializeField] float walkSpeed = 6f;
    [SerializeField] float sprintSpeed = 9f;
    [SerializeField] float acceleration = 14f;

    [Header("Jumping")]
    [SerializeField] float jumpHeight = 1.6f;
    [SerializeField] float gravity = -20f;

    [Header("Grounding")]
    [SerializeField] LayerMask groundMask;

    [Header("Footstep Particles")]
    [SerializeField] private GameObject footstepParticlePrefab;
    [SerializeField] private Transform footPoint;
    [SerializeField] private float spawnRate = 0.15f;

    private float nextSpawnTime;
    CharacterController controller;

    Vector2 moveInput;


    Vector3 velocity;
    float verticalVelocity;

    bool isSprinting;
    bool isGrounded;
    private bool isFiring;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        GroundCheck();
        HandleMovement();
        ApplyGravity();

        if (isGrounded && moveInput.magnitude > 0.1f)
        {
            if (Time.time >= nextSpawnTime)
            {
                GameObject particle = Instantiate(
                    footstepParticlePrefab,
                    footPoint.position,
                    Quaternion.identity
                );

                Destroy(particle, 0.5f); // cleanup after 2 seconds

                nextSpawnTime = Time.time + spawnRate;
            }
        }
    }

    void GroundCheck()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;
    }

    void HandleMovement()
    {
        Vector3 move =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        float speed = isSprinting ? sprintSpeed : walkSpeed;

        Vector3 targetVelocity = move * speed;

        velocity = Vector3.Lerp(
            velocity,
            targetVelocity,
            acceleration * Time.deltaTime
        );

        controller.Move(velocity * Time.deltaTime);
    }

    void ApplyGravity()
    {
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 gravityMove = Vector3.up * verticalVelocity;
        controller.Move(gravityMove * Time.deltaTime);
    }

    

    void Jump()
    {
        Debug.Log(isGrounded);
        if (!isGrounded) return;
        Debug.Log("jump");
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

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
    
}