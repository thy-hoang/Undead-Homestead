using UnityEngine;

public class ZombieFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float stoppingDistance = 1.0f;

    public bool IsKnockedBack { get; set; }

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void FixedUpdate()
    {
        if (target == null || IsKnockedBack)
            return;

        Vector2 direction = ((Vector2)target.position - rb.position).normalized;

        float distance = Vector2.Distance(rb.position, target.position);

        if (distance > stoppingDistance)
        {
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}