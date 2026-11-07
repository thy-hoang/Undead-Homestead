using UnityEngine;
using System.Collections.Generic;

public class ResourceItemManager : MonoBehaviour
{
    public List<ResourceItem> resourceItems = new List<ResourceItem>();

    void Awake()
    {
        ResourceItem[] loadedItems = Resources.LoadAll<ResourceItem>("");

        resourceItems.AddRange(loadedItems);
    }

    public ResourceItem GetResourceItemByName(string itemName)
    {
        return resourceItems.Find(item => item.itemName == itemName);
    }
}