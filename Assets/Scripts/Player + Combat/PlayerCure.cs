using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCure : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HotbarManager hotbarManager;

    [Header("Cure Settings")]
    [SerializeField] private float cureRadius = 1.5f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Scene Settings")]
    [SerializeField] private string homeBaseSceneName = "HomeBase";

    public void OnCure(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (hotbarManager == null)
        {
            Debug.LogError("Hotbar Manager is not assigned.");
            return;
        }

        HotbarItem selectedItem =
            hotbarManager.getSelectedItem();

        AntidoteItem antidote =
            selectedItem as AntidoteItem;

        if (antidote == null)
        {
            Debug.Log("Select an antidote before curing a zombie.");
            return;
        }

        Collider2D[] objectsInRange =
            Physics2D.OverlapCircleAll(
                transform.position,
                cureRadius,
                enemyLayer
            );

        foreach (Collider2D objectInRange in objectsInRange)
        {
            ZombieCureable zombie =
                objectInRange.GetComponent<ZombieCureable>();

            if (zombie == null)
                continue;

            bool wasCured =
                zombie.CureAndReturn(homeBaseSceneName);

            if (wasCured)
            {
                hotbarManager.consumeSelectedItem();

                Debug.Log(
                    "Antidote consumed. " +
                    "The zombie is returning to the home base."
                );

                return;
            }
        }

        Debug.Log("No curable zombie is nearby.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            transform.position,
            cureRadius
        );
    }
}