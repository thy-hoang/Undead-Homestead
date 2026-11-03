using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceEntryUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text quantityText;

    public void Initialize(ResourceItem item)
    {
        icon.sprite = item.icon;
        quantityText.text = item.quantity.ToString();
    }
}