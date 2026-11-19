using UnityEngine;
using System.Linq;

public class ResourceDisplay : MonoBehaviour
{
    [SerializeField] private ResourceItemManager itemManager;
    [SerializeField] private ResourceEntryUI entryPrefab;

    private const float ENTRY_WIDTH = 128f;
    private const float ENTRY_HEIGHT = 64f;

    private const float ROW_SPACING = 10f;

    void Start()
    {
        Debug.Log("ResourceDisplay Start() ran.");

        GenerateDisplay();
    }

    void GenerateDisplay()
    {
        Debug.Log("GenerateDisplay() ran.");

        if (itemManager == null)
        {
            Debug.LogError(
                "ResourceDisplay has no Item Manager assigned."
            );
            return;
        }

        if (entryPrefab == null)
        {
            Debug.LogError(
                "ResourceDisplay has no Entry Prefab assigned."
            );
            return;
        }

        var items = itemManager.resourceItems;

        Debug.Log(
            "Generating " +
            itemManager.resourceItems.Count +
            " resource entries."
        );

        if (items.Count == 0)
            return;

        int maxColumn = items.Max(item => item.column);
        int maxRow = items.Max(item => item.row);
        int maxItemsInRow = items
            .GroupBy(item => item.row)
            .Max(group => group.Count());

        // Size the display to fit its contents
        RectTransform displayRect = GetComponent<RectTransform>();

        displayRect.sizeDelta = new Vector2(
            (maxColumn + 1) * ENTRY_WIDTH,
            (maxRow + 1) * ENTRY_HEIGHT + maxRow * ROW_SPACING
        );

        // Generate entries
        foreach (var item in items)
        {
            ResourceEntryUI entry = Instantiate(entryPrefab, transform);

            Debug.Log(
                "Created resource entry for: " +
                item.itemName
            );

            RectTransform entryRect = entry.GetComponent<RectTransform>();

            entryRect.anchorMin = new Vector2(1, 1);
            entryRect.anchorMax = new Vector2(1, 1);
            entryRect.pivot = new Vector2(1, 1);

            // count how many items precede this one in its row
            var rowItems = items
                .Where(x => x.row == item.row)
                .ToList();

            int index = rowItems.IndexOf(item);

            entryRect.anchoredPosition = new Vector2(
                -item.column * ENTRY_WIDTH,
                -item.row * (ENTRY_HEIGHT + ROW_SPACING)
            );

            entry.Initialize(item);
        }
    }

    public void refreshDisplay()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        GenerateDisplay();
    }

    void Update()
    {
        if (itemManager.doUpdateDisplay)
        {
            refreshDisplay();
            itemManager.doUpdateDisplay = false;
        }
    }
}