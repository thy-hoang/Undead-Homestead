using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private float knockbackDistance = 0.5f;

    [SerializeField] private SimpleFlash flashEffect;

    [SerializeField] private ParticleSystem hitParticleEffect;

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

        onHitEffects();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void onHitEffects()
    {
        flashEffect.Flash();
        if (hitParticleEffect != null)
        {
            hitParticleEffect.Play();
        }
    }

    private void Die()
    {
        ZombieDropTable dropTable =
            GetComponentInParent<ZombieDropTable>();

        if (dropTable != null)
        {
            dropTable.DropLoot(transform.position);
        }
        else
        {
            Debug.LogError(
                "ZombieDropTable was not found on " +
                gameObject.name
            );
        }

        Destroy(gameObject);
    }
}