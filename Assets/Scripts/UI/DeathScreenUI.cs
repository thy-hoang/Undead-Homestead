using UnityEngine;

public class DeathScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject deathPanel;

    private void Awake()
    {
        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    public void ShowDeathScreen()
    {
        if (deathPanel != null)
            deathPanel.SetActive(true);

        Debug.Log("Death screen displayed.");
    }
}