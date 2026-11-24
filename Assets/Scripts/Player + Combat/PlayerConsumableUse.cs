using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConsumableUse : MonoBehaviour
{
    [SerializeField] private HotbarManager hotbarManager;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private FarmPlotInteraction farmPlotInteraction;

    private void Awake()
    {
        if (farmPlotInteraction == null)
        {
            farmPlotInteraction =
                GetComponent<FarmPlotInteraction>();
        }
    }

    public void OnUseItem(InputValue value)
    {
        if (!value.isPressed)
            return;

        // Farm plots always own the interact location.
        // This prevents food consumption on empty,
        // planted, or mature farm plots.
        if (farmPlotInteraction != null &&
            farmPlotInteraction.IsStandingOnFarmPlot())
        {
            return;
        }

        if (hotbarManager == null ||
            playerHealth == null)
        {
            Debug.LogError(
                "Hotbar Manager or Player Health " +
                "is not assigned."
            );

            return;
        }

        HotbarItem selectedItem =
            hotbarManager.getSelectedItem();

        if (selectedItem is not ConsumableItem food)
        {
            Debug.Log("Select a food item first.");
            return;
        }

        playerHealth.Heal(food.healthRestored);

        hotbarManager.consumeSelectedItem();

        Debug.Log(
            $"{food.itemName} consumed. " +
            $"Restored {food.healthRestored} health."
        );
    }
}