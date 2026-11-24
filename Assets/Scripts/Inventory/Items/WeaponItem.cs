using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(
    fileName = "Weapon",
    menuName = "Inventory/Weapon Item"
)]
public class WeaponItem : CraftableItem
{
    [Header("Weapon Stats")]
    public float damage;
    public float attackSpeed;
    public float range;

    [Header("Tool Properties")]
    public bool canChopTrees;

    [Header("Tool Properties")]
    public bool canMineRocks;
}
