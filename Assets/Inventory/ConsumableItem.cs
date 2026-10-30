using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Item",
                 menuName = "Inventory/Consumable Item")]
public class ConsumableItem : Item
{
    public float healthRestored;
}