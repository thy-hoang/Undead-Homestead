using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ResourceDisplay : MonoBehaviour
{
    [SerializeField] private ResourceItemManager itemManager;
    [SerializeField] private ResourceEntryUI entryPrefab;

    void Start()
    {
        GenerateDisplay();
    }

    void GenerateDisplay()
    {
        int maxRow = itemManager.resourceItems.Max(item => item.row);

        for (int i = 0; i <= maxRow; i++)
        {
            GameObject rowObject = new GameObject(
                $"Row {i}",
                typeof(RectTransform),
                typeof(HorizontalLayoutGroup)
            );

            rowObject.transform.SetParent(transform, false);

            HorizontalLayoutGroup layout =
                rowObject.GetComponent<HorizontalLayoutGroup>();

            layout.spacing = 10;
            layout.childAlignment = TextAnchor.MiddleRight;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var rowItems = itemManager.resourceItems
                .Where(item => item.row == i);

            // Generate an entry for each item belonging to the current row
            foreach (var item in rowItems)
            {
                ResourceEntryUI entry = Instantiate(
                    entryPrefab,
                    rowObject.transform
                );

                entry.Initialize(item);
            }
        }
    }
}