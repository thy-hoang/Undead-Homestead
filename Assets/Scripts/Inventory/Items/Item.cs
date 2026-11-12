using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Item Properties")]
    public string itemName;
    public Sprite icon;
    public string description;
}