using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;
    [SerializeField] private Sprite sideSprite;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackDistance = 0.7f;
    [SerializeField] private float moveSpeed = 4f;

    private SpriteRenderer spriteRenderer;
    private Vector2 lastFacingDirection = Vector2.down;
    private Rigidbody2D rb;
    private Vector2 movementInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    private void Update()
    {
        if (movementInput == Vector2.zero)
            return;

        if (Mathf.Abs(movementInput.y) > Mathf.Abs(movementInput.x))
        {
            if (movementInput.y > 0)
            {
                spriteRenderer.sprite = upSprite;
                lastFacingDirection = Vector2.up;
            }
            else
            {
                spriteRenderer.sprite = downSprite;
                lastFacingDirection = Vector2.down;
            }
        }
        else
        {
            spriteRenderer.sprite = sideSprite;

            if (movementInput.x > 0)
            {
                spriteRenderer.flipX = false;
                lastFacingDirection = Vector2.right;
            }
            else
            {
                spriteRenderer.flipX = true;
                lastFacingDirection = Vector2.left;
            }
        }

        attackPoint.localPosition = lastFacingDirection * attackDistance;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movementInput * moveSpeed;
    }
}