using UnityEngine;
using UnityEngine.InputSystem;

public class PopupToggle : MonoBehaviour
{
    public GameObject popupPanel;
    public Key toggleKey = Key.Tab;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame)
        {
            popupPanel.SetActive(!popupPanel.activeSelf);
        }
    }
}
