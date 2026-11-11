using UnityEngine;

[CreateAssetMenu(
    fileName = "New Placeable Item",
    menuName = "Inventory/Placeable Item"
)]
public class PlaceableItem : Item
{
    [Header("Placeable Properties")]
    public GameObject prefab;

    public Vector2Int gridSize = Vector2Int.one;

    public bool canStore = true;

    public float placementRange;
}