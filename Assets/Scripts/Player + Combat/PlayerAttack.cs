using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Base Player Stats")]
    [SerializeField] private int attackDamage = 25;
    [SerializeField] private float attackSpeed = 2.5f;
    [SerializeField] private float attackRadius = 1.5f;

    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private HotbarManager hotbarManager;

    private float nextAttackTime;

    public void OnAttack(InputValue value)
    {
        if (!value.isPressed || Time.time < nextAttackTime)
            return;

        Attack();

        float finalAttackSpeed = GetFinalAttackSpeed();
        nextAttackTime = Time.time + (1f / finalAttackSpeed);
    }

    private void Attack()
    {
        HotbarItem selectedItem = hotbarManager.getSelectedItem();

        if (selectedItem is not WeaponItem weapon)
        {
            Debug.Log("Select a weapon before attacking.");
            return;
        }

        int finalDamage =
            attackDamage + Mathf.RoundToInt(weapon.damage);

        float finalRadius =
            attackRadius + weapon.range;

        float finalAttackSpeed =
            attackSpeed + weapon.attackSpeed;

        float finalCooldown =
            1f / Mathf.Max(finalAttackSpeed, 0.01f);

        Debug.Log(
            $"Attacking with {weapon.itemName}. " +
            $"Damage: {finalDamage}, " +
            $"Range: {finalRadius}, " +
            $"Attack Speed: {finalAttackSpeed}"
        );

        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(
            attackPoint.position,
            finalRadius,
            enemyLayer
        );

        foreach (Collider2D enemy in enemiesHit)
        {
            ZombieHealth health = enemy.GetComponent<ZombieHealth>();

            if (health != null)
            {
                health.TakeDamage(finalDamage, transform.position);
            }
        }

        nextAttackTime = Time.time + finalCooldown;
    }

    private float GetFinalAttackSpeed()
    {
        float finalSpeed = attackSpeed;

        if (hotbarManager != null)
        {
            HotbarItem selectedItem = hotbarManager.getSelectedItem();

            if (selectedItem is WeaponItem weapon)
            {
                finalSpeed += weapon.attackSpeed;
            }
        }

        return Mathf.Max(finalSpeed, 0.01f);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius
        );
    }
}