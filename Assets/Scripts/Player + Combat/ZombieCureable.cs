using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieCureable : MonoBehaviour
{
    public bool IsCured { get; private set; }

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color curedColor = Color.green;

    private string homeSceneName;
    private GameObject zombieRoot;

    private void Awake()
    {
        zombieRoot = transform.root.gameObject;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public bool CureAndReturn(string sceneName)
    {
        if (IsCured)
            return false;

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Home base scene name is empty.");
            return false;
        }

        IsCured = true;
        homeSceneName = sceneName;

        ZombieFollow follow =
            zombieRoot.GetComponentInChildren<ZombieFollow>();

        if (follow != null)
            follow.enabled = false;

        ZombieAttack attack =
            zombieRoot.GetComponentInChildren<ZombieAttack>();

        if (attack != null)
            attack.enabled = false;

        if (spriteRenderer != null)
            spriteRenderer.color = curedColor;

        DontDestroyOnLoad(zombieRoot);

        SceneManager.sceneLoaded += OnHomeSceneLoaded;

        Debug.Log(
            "Cured zombie preserved. Loading scene: " +
            homeSceneName
        );

        SceneManager.LoadScene(homeSceneName);

        return true;
    }

    private void OnHomeSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        Debug.Log("Loaded scene: " + scene.name);

        if (scene.name != homeSceneName)
            return;

        SceneManager.sceneLoaded -= OnHomeSceneLoaded;

        GameObject homeBasePoint =
            GameObject.FindGameObjectWithTag(
                "HomeBasePoint"
            );

        if (homeBasePoint == null)
        {
            Debug.LogError(
                "HomeBasePoint was not found in the home-base scene."
            );

            return;
        }

        zombieRoot.transform.position =
            homeBasePoint.transform.position;

        zombieRoot.SetActive(true);

        Debug.Log(
            "Cured zombie placed at home base position: " +
            homeBasePoint.transform.position
        );
    }
}