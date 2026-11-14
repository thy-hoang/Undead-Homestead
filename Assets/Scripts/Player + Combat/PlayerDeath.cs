using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HotbarManager hotbarManager;

    [Header("Scene Settings")]
    [SerializeField] private string labSceneName = "LabScene";

    private bool isDying;

    public void Die()
    {
        if (isDying)
            return;

        isDying = true;

        Debug.Log("Player died.");

        if (hotbarManager != null)
            hotbarManager.clearHotbar();

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnLabSceneLoaded;
        SceneManager.LoadScene(labSceneName);
    }

    private void OnLabSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        if (scene.name != labSceneName)
            return;

        SceneManager.sceneLoaded -= OnLabSceneLoaded;

        GameObject bedPoint =
            GameObject.FindGameObjectWithTag("BedPoint");

        if (bedPoint == null)
        {
            Debug.LogError(
                "No object tagged BedPoint was found in LabScene."
            );

            return;
        }

        transform.position =
            bedPoint.transform.position;

        PlayerHealth playerHealth =
            GetComponent<PlayerHealth>();

        if (playerHealth != null)
            playerHealth.ResetHealth();

        DeathScreenUI deathScreen =
            FindFirstObjectByType<DeathScreenUI>();

        if (deathScreen != null)
            deathScreen.ShowDeathScreen();

        // Reconnect to the LabScene hotbar if one exists.
        HotbarManager newHotbar =
            FindFirstObjectByType<HotbarManager>();

        if (newHotbar != null)
            hotbarManager = newHotbar;

        Debug.Log("Player respawned at the lab bed.");

        isDying = false;
    }
}