using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Item",
                 menuName = "Inventory/Resource Item")]
public class ResourceItem : Item
{
    public int quantity;

    public int row;
}