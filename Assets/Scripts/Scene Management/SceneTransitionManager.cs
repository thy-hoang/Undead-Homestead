using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private int fadeSteps = 10;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip transitionSound;

    private bool isTransitioning;
    private bool sceneFinishedLoading;
    private string pendingSpawnID;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (fadeGroup != null)
        {
            StartCoroutine(Fade(0f));
        }
    }

    public void LoadScene(
        string sceneName,
        string spawnID
    )
    {
        if (isTransitioning)
            return;

        pendingSpawnID = spawnID;
        isTransitioning = true;

        StartCoroutine(Transition(sceneName));
    }

    private IEnumerator Transition(string sceneName)
    {
        if (fadeGroup != null)
        {
            yield return StartCoroutine(Fade(1f));
        }

        if (audioSource != null &&
            transitionSound != null)
        {
            audioSource.PlayOneShot(transitionSound);
        }

        sceneFinishedLoading = false;

        SceneManager.sceneLoaded += HandleSceneLoaded;

        SceneManager.LoadScene(sceneName);

        yield return new WaitUntil(
            () => sceneFinishedLoading
        );

        if (fadeGroup != null)
        {
            yield return StartCoroutine(Fade(0f));
        }

        isTransitioning = false;
    }

    private void HandleSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;

        PlacePlayerAtSpawnPoint();

        sceneFinishedLoading = true;
    }

    private void PlacePlayerAtSpawnPoint()
    {
        if (string.IsNullOrEmpty(pendingSpawnID))
        {
            Debug.LogWarning(
                "No destination spawn ID was provided."
            );

            return;
        }

        SceneSpawnPoint[] spawnPoints =
            FindObjectsByType<SceneSpawnPoint>(
                FindObjectsSortMode.None
            );

        SceneSpawnPoint matchingSpawn = null;

        foreach (SceneSpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.SpawnID == pendingSpawnID)
            {
                matchingSpawn = spawnPoint;
                break;
            }
        }

        if (matchingSpawn == null)
        {
            Debug.LogError(
                "No spawn point with ID '" +
                pendingSpawnID +
                "' was found in the loaded scene."
            );

            return;
        }

        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError(
                "Player was not found in the loaded scene."
            );

            return;
        }

        Vector3 spawnPosition =
            matchingSpawn.transform.position;

        player.transform.position = spawnPosition;

        Rigidbody2D playerRb =
            player.GetComponent<Rigidbody2D>();

        if (playerRb != null)
        {
            playerRb.position = spawnPosition;
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
        }

        Debug.Log(
            "Player placed at spawn point '" +
            pendingSpawnID +
            "' at position " +
            spawnPosition
        );

        pendingSpawnID = "";
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeGroup == null)
            yield break;

        float startAlpha = fadeGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed / fadeDuration
                );

            float steppedProgress =
                Mathf.Floor(
                    progress * fadeSteps
                ) / fadeSteps;

            fadeGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                steppedProgress
            );

            yield return null;
        }

        fadeGroup.alpha = targetAlpha;
    }
}