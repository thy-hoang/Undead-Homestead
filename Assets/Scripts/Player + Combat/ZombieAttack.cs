using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1.5f;

    private float nextAttackTime;

    private void Update()
    {
        if (target == null ||
            !target.gameObject.activeInHierarchy)
        {
            FindPlayer();

            if (target == null)
                return;
        }

        if (Time.time < nextAttackTime)
            return;

        float distanceToPlayer =
            Vector2.Distance(
                transform.position,
                target.position
            );

        if (distanceToPlayer > attackRange)
            return;

        PlayerHealth playerHealth =
            target.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            playerHealth =
                target.GetComponentInParent<PlayerHealth>();
        }

        if (playerHealth == null)
        {
            playerHealth =
                target.GetComponentInChildren<PlayerHealth>();
        }

        if (playerHealth == null)
        {
            Debug.LogError(
                "PlayerHealth was not found on " +
                target.name
            );
            return;
        }

        if (playerHealth.IsDead)
            return;

        playerHealth.TakeDamage(attackDamage);

        nextAttackTime =
            Time.time + attackCooldown;

        Debug.Log(
            name +
            " attacked the player for " +
            attackDamage +
            " damage."
        );
    }

    private void FindPlayer()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            target = player.transform;
        }
    }
}