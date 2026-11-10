using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(
    fileName = "Weapon",
    menuName = "Inventory/Weapon Item"
)]
public class WeaponItem : HotbarItem
{
    [Header("Weapon Properties")]
    public List<CraftingComponent> components = new List<CraftingComponent>();

    [Header("Weapon Stats")]
    public float damage;
    public float attackSpeed;
    public float range;
}
