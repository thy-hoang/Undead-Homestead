using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.7f;
    [SerializeField] private int attackDamage = 25;
    [SerializeField] private float attackCooldown = 0.4f;
    [SerializeField] private LayerMask enemyLayer;

    private float nextAttackTime;

    public void OnAttack(InputValue value)
    {
        if (value.isPressed && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void Attack()
        {
    Debug.Log("Player attacked!");

    Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(
        attackPoint.position,
        attackRadius,
        enemyLayer
    );

    foreach (Collider2D enemy in enemiesHit)
    {
        Debug.Log("Hit: " + enemy.name);

        ZombieHealth health = enemy.GetComponent<ZombieHealth>();

        if (health != null)
        {
            health.TakeDamage(attackDamage);
        }
    }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}