using UnityEngine;

public class ZombieFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float stoppingDistance = 1.0f;

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
        if (target == null)
            return;

        float distance = Vector2.Distance(rb.position, target.position);

        if (distance > stoppingDistance)
        {
            Vector2 newPosition = Vector2.MoveTowards(
                rb.position,
                target.position,
                moveSpeed * Time.fixedDeltaTime
            );

            rb.MovePosition(newPosition);
        }
    }
}