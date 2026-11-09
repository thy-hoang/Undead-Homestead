using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlaceableItemQuantity
{
    public PlaceableItem item;
    public int quantity;
}

public class PlaceableInventoryManager : MonoBehaviour
{
    [SerializeField]
    private List<PlaceableItemQuantity> startingItems =
        new List<PlaceableItemQuantity>();

    public event Action InventoryChanged;

    public int GetQuantity(PlaceableItem item)
    {
        PlaceableItemQuantity entry = FindEntry(item);

        if (entry == null)
            return 0;

        return entry.quantity;
    }

    public bool TryConsume(PlaceableItem item)
    {
        PlaceableItemQuantity entry = FindEntry(item);

        if (entry == null || entry.quantity <= 0)
            return false;

        entry.quantity--;

        InventoryChanged?.Invoke();
        return true;
    }

    public void AddItem(PlaceableItem item)
    {
        if (item == null)
            return;

        PlaceableItemQuantity entry = FindEntry(item);

        if (entry == null)
        {
            entry = new PlaceableItemQuantity
            {
                item = item,
                quantity = 0
            };

            startingItems.Add(entry);
        }

        entry.quantity++;

        InventoryChanged?.Invoke();
    }

    private PlaceableItemQuantity FindEntry(
        PlaceableItem item)
    {
        foreach (PlaceableItemQuantity entry in startingItems)
        {
            if (entry.item == item)
                return entry;
        }

        return null;
    }
}