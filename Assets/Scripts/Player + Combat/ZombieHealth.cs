using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private float knockbackDistance = 0.5f;

    private Rigidbody2D rb;

    private void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        currentHealth -= damage;

        Vector2 knockbackDirection =
            ((Vector2)transform.position - attackerPosition).normalized;

        if (rb != null)
        {
            rb.MovePosition(
                rb.position + knockbackDirection * knockbackDistance
            );
        }
        else
        {
            transform.position +=
                (Vector3)(knockbackDirection * knockbackDistance);
        }

        Debug.Log("Zombie health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}