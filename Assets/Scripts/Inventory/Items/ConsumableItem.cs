using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Consumable Item",
                 menuName = "Inventory/Consumable Item")]
public class ConsumableItem : HotbarItem
{
    [Header("Consumable Properties")]
    public List<RecipeIngredient> recipe = new List<RecipeIngredient>();
    public float healthRestored;
}