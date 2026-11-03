using UnityEngine;
using System.Collections.Generic;

public class ResourceItemManager : MonoBehaviour
{
    public List<ResourceItem> resourceItems = new List<ResourceItem>();

    void Awake()
    {
        ResourceItem[] loadedItems = Resources.LoadAll<ResourceItem>("");

        resourceItems.AddRange(loadedItems);

        Debug.Log($"Loaded {resourceItems.Count} resource items.");
        Debug.Log("Resource Items:");
        foreach (var item in resourceItems)
        {
            Debug.Log($"- {item.itemName} (Quantity: {item.quantity}, Row: {item.row})");
        }
    }

    ResourceItem GetResourceItemByName(string itemName)
    {
        return resourceItems.Find(item => item.itemName == itemName);
    }
}