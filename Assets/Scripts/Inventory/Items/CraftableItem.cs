using UnityEngine;
using System.Collections.Generic;

public class CraftableItem : HotbarItem
{
    [Header("Craftable Properties")]
    public List<CraftingComponent> components = new List<CraftingComponent>();
}
