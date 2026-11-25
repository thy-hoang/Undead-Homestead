using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [SerializeField] private SimpleFlash flashEffect;

    [SerializeField] private ParticleSystem hitParticleEffect;

    [SerializeField] private float knockbackForce = 20f;
    [SerializeField] private float knockbackDuration = 0.34f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip onHitSound;
    [SerializeField] private AudioClip onDeathSound;

    [SerializeField] private GameObject damageIndicatorPrefab;

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

        if (currentHealth <= 0)
        {
            Die();
        }

        onHitEffects(damage);

        
    }

    public void onHitEffects(int damage)
    {
        flashEffect.Flash();
        if (hitParticleEffect != null)
        {
            hitParticleEffect.Play();
        }

        // Sound effect
        if (audioSource != null && onHitSound != null && currentHealth > 0)
        {
            audioSource.PlayOneShot(onHitSound);
        }

        // Spawn damage indicator
        Vector3 spawnPosition = transform.position + Vector3.up;

        GameObject indicator = Instantiate(
            damageIndicatorPrefab,
            spawnPosition,
            Quaternion.identity
        );

        indicator.GetComponent<DamageIndicator>().SetDamage(damage);
    }

    private void Die()
    {
        Debug.Log(
            gameObject.name +
            " died. Destroying zombie."
        );

        ZombieDropTable dropTable =
            GetComponentInParent<ZombieDropTable>();

        if (dropTable != null)
        {
            dropTable.DropLoot(transform.position);
        }

        StatisticsManager.Instance.Increment("ZombiesKilled", 1);

        // Sound effect
        if (audioSource != null && onDeathSound != null)
        {
            AudioSource.PlayClipAtPoint(
                onDeathSound,
                transform.position
            );
        }

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