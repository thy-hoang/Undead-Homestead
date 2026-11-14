using UnityEngine;
using System.Collections.Generic;

public class PlaceableItemManager : MonoBehaviour
{
    public List<PlaceableItem> placeableItems =
        new List<PlaceableItem>();

    private void Awake()
    {
        PlaceableItem[] loadedItems =
            Resources.LoadAll<PlaceableItem>("");

        placeableItems.AddRange(loadedItems);
    }
}