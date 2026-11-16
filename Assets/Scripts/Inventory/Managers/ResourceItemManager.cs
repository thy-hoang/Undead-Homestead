using UnityEngine;
using System.Collections.Generic;

public class ResourceItemManager : MonoBehaviour
{
    public List<ResourceItem> resourceItems = new List<ResourceItem>();

    public bool doUpdateDisplay = false;

    private void Awake()
    {
        ResourceItem[] loadedItems =
            Resources.LoadAll<ResourceItem>("");

        resourceItems.AddRange(loadedItems);

        Debug.Log(
            "Loaded resource items: " +
            resourceItems.Count
        );
    }

    public ResourceItem GetResourceItemByName(string itemName)
    {
        return resourceItems.Find(item => item.itemName == itemName);
    }
}
