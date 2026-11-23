using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [SerializeField] private SimpleFlash flashEffect;

    [SerializeField] private ParticleSystem hitParticleEffect;

    [SerializeField] private float knockbackForce = 7f;
    [SerializeField] private float knockbackDuration = 0.34f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip onHitSound;

    private ZombieFollow zombieFollow;

    private Rigidbody2D rb;

    private void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        zombieFollow = GetComponent<ZombieFollow>();
    }

    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        currentHealth -= damage;

        Vector2 knockbackDirection =
            ((Vector2)transform.position - attackerPosition).normalized;

        if (rb != null)
        {
            StartCoroutine(ApplyKnockback(knockbackDirection));
        }
        else
        {
            Debug.LogError(
                "Rigidbody2D component not found on " +
                gameObject.name
            );
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

        // Sound effect
        if (audioSource != null && onHitSound != null)
        {
            audioSource.PlayOneShot(onHitSound);
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

        StatisticsManager.Instance.Increment("ZombiesKilled", 1);

        Destroy(gameObject);
    }

    private System.Collections.IEnumerator ApplyKnockback(
        Vector2 direction
    )
    {
        if (zombieFollow != null)
            zombieFollow.IsKnockedBack = true;

        float elapsedTime = 0f;
        Vector2 initialVelocity = direction * knockbackForce;

        while (elapsedTime < knockbackDuration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / knockbackDuration);

            Vector2 currentVelocity = Vector2.Lerp(
                initialVelocity,
                Vector2.zero,
                t
            );

            rb.MovePosition(
                rb.position + currentVelocity * Time.fixedDeltaTime
            );

            elapsedTime += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        if (zombieFollow != null)
            zombieFollow.IsKnockedBack = false;
    }
}