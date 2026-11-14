using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    public bool IsDead => currentHealth <= 0;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log(
            "Player took damage. Health: " +
            currentHealth + "/" + maxHealth
        );

        if (currentHealth <= 0)
        {
            PlayerDeath playerDeath =
                GetComponent<PlayerDeath>();

            if (playerDeath != null)
                playerDeath.Die();
        }
    }

    public void Heal(float amount)
    {
        int oldHealth = currentHealth;

        currentHealth += Mathf.RoundToInt(amount);
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        int restoredAmount = currentHealth - oldHealth;

        Debug.Log(
            "Player health restored by " +
            restoredAmount + ". Current health: " +
            currentHealth + "/" + maxHealth
        );
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;

        Debug.Log(
            "Player respawned with full health: " +
            currentHealth + "/" + maxHealth
        );
    }
}