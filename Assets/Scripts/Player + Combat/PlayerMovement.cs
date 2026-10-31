using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private Transform attackPoint;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
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
        if (movementInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (movementInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }


        if (movementInput.x > 0)
        {
            spriteRenderer.flipX = false;
            attackPoint.localPosition = new Vector3(
                Mathf.Abs(attackPoint.localPosition.x),
                attackPoint.localPosition.y,
                attackPoint.localPosition.z
            );
        }
        else if (movementInput.x < 0)
        {
            spriteRenderer.flipX = true;
            attackPoint.localPosition = new Vector3(
                -Mathf.Abs(attackPoint.localPosition.x),
                attackPoint.localPosition.y,
                attackPoint.localPosition.z
            );
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movementInput * moveSpeed;
    }
}