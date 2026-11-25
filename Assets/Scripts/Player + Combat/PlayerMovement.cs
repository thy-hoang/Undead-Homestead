using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackDistance = 0.7f;
    [SerializeField] private float moveSpeed = 4f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastFacingDirection = Vector2.down;
    private Rigidbody2D rb;
    private Vector2 movementInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    private void Update()
    {
        UpdateAnimations();
        if (movementInput == Vector2.zero)
            return;

        attackPoint.localPosition = lastFacingDirection * attackDistance;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movementInput * moveSpeed;
    }

    void UpdateAnimations()
    {
        animator.SetBool("isWalkingUp", false);
        animator.SetBool("isWalkingDown", false);
        animator.SetBool("isWalkingSide", false);

        if (movementInput == Vector2.zero) return;

        if (movementInput.y > 0)
        {
            animator.SetBool("isWalkingUp", true);
        }
        else if (movementInput.y < 0)
        {
            animator.SetBool("isWalkingDown", true);
        }
        else if (movementInput.x != 0)
        {
            animator.SetBool("isWalkingSide", true);
            spriteRenderer.flipX = movementInput.x < 0;
        }
    }
}