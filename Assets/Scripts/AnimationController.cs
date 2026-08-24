using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController playerController;

    // Exact Parameter Hashes matching your Animator setup
    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int IsSprintingHash = Animator.StringToHash("IsSprinting");

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (playerController == null)
        {
            playerController = GetComponentInParent<PlayerController>();
        }
    }

    private void Update()
    {
        if (animator == null || playerController == null) return;

        UpdateAnimatorStates();
    }

    private void UpdateAnimatorStates()
    {
        animator.SetBool(IsWalkingHash, playerController.IsWalking);
        animator.SetBool(IsSprintingHash, playerController.IsSprinting);
    }
}