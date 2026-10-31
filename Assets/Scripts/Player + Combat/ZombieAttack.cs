using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1.5f;

    private float nextAttackTime;

    private void Awake()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void Update()
    {
        if (target == null || Time.time < nextAttackTime)
            return;

        float distanceToPlayer = Vector2.Distance(
            transform.position,
            target.position
        );

        if (distanceToPlayer <= attackRange)
        {
            PlayerHealth playerHealth =
                target.GetComponent<PlayerHealth>();

            if (playerHealth != null && !playerHealth.IsDead)
            {
                playerHealth.TakeDamage(attackDamage);
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }
}