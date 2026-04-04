using UnityEngine;

public class PlayerAnimatorManager : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public GridPlayerController playerController;
    void Update()
    {
        HandleAnimations();
    }
    void HandleAnimations()
    {
        float speedPercent = playerController.Velocity.magnitude / playerController.sprintSpeed;

        animator.SetFloat("speed", speedPercent, 0.1f, Time.deltaTime);
        animator.SetBool("isGrounded", playerController.isGrounded);
        animator.SetFloat("yVelocity", playerController.verticalVelocity);
    }
}
