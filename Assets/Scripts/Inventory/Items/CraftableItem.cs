using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(
    fileName = "New Craftable Item",
    menuName = "Inventory/Craftable Item"
)]
public class CraftableItem : HotbarItem
{
    [Header("Craftable Properties")]
    public List<CraftingComponent> components = new List<CraftingComponent>();
}
