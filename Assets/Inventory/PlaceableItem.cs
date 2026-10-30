using UnityEngine;

[CreateAssetMenu(fileName = "New Placeable Item",
                 menuName = "Inventory/Placeable Item")]
public class PlaceableItem : Item
{
    public GameObject prefab;
    public float placementRange;
}