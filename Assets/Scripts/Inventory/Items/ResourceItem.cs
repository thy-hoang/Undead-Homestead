using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Item",
                 menuName = "Inventory/Resource Item")]
public class ResourceItem : Item
{
    [Header("Resource Properties")]
    public int quantity;
    
    [Header("Location Properties")]
    public int row;

    public int column;
}