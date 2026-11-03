using UnityEngine;
using System.Collections.Generic;

public class HotbarManager : MonoBehaviour
{
    public List<GameObject> childList = new List<GameObject>();

    public List<HotbarItem> hotbarItems = new List<HotbarItem>();

    void Start()
    {
        foreach (Transform child in transform)
        {
            childList.Add(child.gameObject);
        }

        Debug.Log($"HotbarManager: Found {childList.Count} child objects.");

        
    }
}