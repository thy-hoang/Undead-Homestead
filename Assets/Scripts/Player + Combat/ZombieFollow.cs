using UnityEngine;

public class ZombieFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float stoppingDistance = 1f;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackRecoveryTime = 0.34f;

    private Animator animator;
    private Rigidbody2D rb;
    private float knockbackEndTime;
    private SpriteRenderer spriteRenderer;

    public bool IsKnockedBack
    {
        get
        {
            return Time.time < knockbackEndTime;
        }
        set
        {
            if (value)
            {
                knockbackEndTime =
                    Time.time + knockbackRecoveryTime;
            }
            else
            {
                knockbackEndTime = 0f;
            }
        }
    }

    private void Start()
    {
        FindPlayer();
    }

    private void FindPlayer()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            target = player.transform;

            Debug.Log(
                gameObject.name +
                " is following " +
                player.name
            );
        }
        else
        {
            Debug.LogError(
                "No active object tagged Player was found."
            );
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (target == null)
        {
            GameObject player =
                GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogError(
                    "ZombieFollow could not find the Player."
                );
            }
        }

        if (rb == null)
        {
            Debug.LogError(
                "ZombieFollow requires a Rigidbody2D on " +
                gameObject.name
            );
        }
    }

    private void Update()
    {
        
        if (target == null) return;
        Vector2 direction = (target.position - transform.position).normalized;

        UpdateAnimations(direction);
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        Vector2 currentPosition = rb.position;
        Vector2 targetPosition = target.position;

        float distance =
            Vector2.Distance(
                currentPosition,
                targetPosition
            );

        if (distance <= stoppingDistance)
            return;

        Vector2 newPosition = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        // Debug.Log(
        //     gameObject.name +
        //     " current: " + currentPosition +
        //     " target: " + targetPosition +
        //     " new: " + newPosition
        // );

        rb.position = newPosition;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void UpdateAnimations(Vector2 direction)
    {
        // Reset all
        animator.SetBool("isWalkingUp", false);
        animator.SetBool("isWalkingDown", false);
        animator.SetBool("isWalkingSide", false);

        if (direction == Vector2.zero) return;

        // Use whichever axis is dominant
        if (Mathf.Abs(direction.y) >= Mathf.Abs(direction.x))
        {
            if (direction.y > 0)
                animator.SetBool("isWalkingUp", true);
            else
                animator.SetBool("isWalkingDown", true);
        }
        else
        {
            animator.SetBool("isWalkingSide", true);
            spriteRenderer.flipX = direction.x < 0;
        }
    }
}