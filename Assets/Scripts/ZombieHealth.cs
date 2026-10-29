using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
{
    currentHealth -= damage;

    Debug.Log("Zombie health: " + currentHealth);

    if (currentHealth <= 0)
    {
        Debug.Log("Zombie defeated!");
        Die();
    }
}

    private void Die()
    {
        Destroy(gameObject);
    }
}