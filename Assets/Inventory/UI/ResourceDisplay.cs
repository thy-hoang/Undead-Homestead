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
        GenerateDisplay();
    }

    void GenerateDisplay()
    {
        var items = itemManager.resourceItems;

        if (items.Count == 0)
            return;

        int maxRow = items.Max(item => item.row);
        int maxItemsInRow = items
            .GroupBy(item => item.row)
            .Max(group => group.Count());

        // Size the display to fit its contents
        RectTransform displayRect = GetComponent<RectTransform>();

        displayRect.sizeDelta = new Vector2(
            maxItemsInRow * ENTRY_WIDTH,
            (maxRow + 1) * ENTRY_HEIGHT + maxRow * ROW_SPACING
        );

        // Generate entries
        foreach (var item in items)
        {
            ResourceEntryUI entry = Instantiate(entryPrefab, transform);

            RectTransform entryRect = entry.GetComponent<RectTransform>();

            entryRect.anchorMin = new Vector2(1, 1);
            entryRect.anchorMax = new Vector2(1, 1);
            entryRect.pivot = new Vector2(1, 1);

            // Count how many items precede this one in its row
            var rowItems = items
                .Where(x => x.row == item.row)
                .ToList();

            int index = rowItems.IndexOf(item);

            entryRect.anchoredPosition = new Vector2(
                -index * ENTRY_WIDTH,
                -item.row * (ENTRY_HEIGHT + ROW_SPACING)
            );

            entry.Initialize(item);
        }
    }
}