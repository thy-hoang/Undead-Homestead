using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Item",
                 menuName = "Inventory/Consumable Item")]
public class ConsumableItem : HotbarItem
{
    public float healthRestored;
}