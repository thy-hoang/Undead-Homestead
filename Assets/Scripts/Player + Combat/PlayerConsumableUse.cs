using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConsumableUse : MonoBehaviour
{
    [SerializeField] private HotbarManager hotbarManager;
    [SerializeField] private PlayerHealth playerHealth;

    public void OnUseItem(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (hotbarManager == null || playerHealth == null)
        {
            Debug.LogError("Hotbar Manager or Player Health is not assigned.");
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