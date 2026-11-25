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

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError(
                "Scene name is empty."
            );
            return;
        }

        pendingSpawnID =
            spawnID == null
                ? ""
                : spawnID.Trim();

        isTransitioning = true;

        Debug.Log(
            "Loading scene '" +
            sceneName +
            "' with spawn ID '" +
            pendingSpawnID +
            "'."
        );

        StartCoroutine(
            Transition(sceneName)
        );
    }

    private IEnumerator Transition(
        string sceneName
    )
    {
        if (fadeGroup != null)
        {
            yield return StartCoroutine(
                Fade(1f)
            );
        }

        if (audioSource != null &&
            transitionSound != null)
        {
            audioSource.PlayOneShot(
                transitionSound
            );
        }

        AsyncOperation loadOperation =
            SceneManager.LoadSceneAsync(
                sceneName
            );

        if (loadOperation == null)
        {
            Debug.LogError(
                "Could not load scene: " +
                sceneName
            );

            isTransitioning = false;
            yield break;
        }

        yield return loadOperation;

        // Allow the new scene's objects to initialize.
        yield return null;

        yield return StartCoroutine(
            PlacePlayerAfterSceneLoad()
        );

        if (fadeGroup != null)
        {
            yield return StartCoroutine(
                Fade(0f)
            );
        }

        pendingSpawnID = "";
        isTransitioning = false;
    }

    private IEnumerator PlacePlayerAfterSceneLoad()
    {
        if (string.IsNullOrWhiteSpace(pendingSpawnID))
        {
            Debug.LogWarning(
                "No destination spawn ID was provided."
            );

            yield break;
        }

        for (int attempt = 0; attempt < 30; attempt++)
        {
            SceneSpawnPoint[] spawnPoints =
                FindObjectsByType<SceneSpawnPoint>(
                    FindObjectsSortMode.None
                );

            SceneSpawnPoint matchingSpawn =
                null;

            foreach (SceneSpawnPoint point in spawnPoints)
            {
                if (point == null)
                    continue;

                string pointID =
                    point.SpawnID == null
                        ? ""
                        : point.SpawnID.Trim();

                if (pointID == pendingSpawnID)
                {
                    matchingSpawn = point;
                    break;
                }
            }

            GameObject[] players =
                GameObject.FindGameObjectsWithTag(
                    "Player"
                );

            if (players.Length > 1)
            {
                Debug.LogError(
                    "Multiple Player objects were found. " +
                    "Only one active Player should exist."
                );

                yield break;
            }

            if (matchingSpawn != null &&
                players.Length == 1)
            {
                GameObject player =
                    players[0];

                Vector3 spawnPosition =
                    matchingSpawn.transform.position;

                Rigidbody2D playerRb =
                    player.GetComponent<Rigidbody2D>();

                if (playerRb != null)
                {
                    playerRb.position =
                        spawnPosition;

                    playerRb.linearVelocity =
                        Vector2.zero;

                    playerRb.angularVelocity =
                        0f;
                }

                player.transform.position =
                    spawnPosition;

                Debug.Log(
                    "Player placed at spawn point '" +
                    pendingSpawnID +
                    "' at position " +
                    spawnPosition
                );

                yield break;
            }

            yield return null;
        }

        Debug.LogError(
            "Player could not be placed at spawn point '" +
            pendingSpawnID +
            "'."
        );
    }

    private IEnumerator Fade(
        float targetAlpha
    )
    {
        if (fadeGroup == null)
            yield break;

        float startAlpha =
            fadeGroup.alpha;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed +=
                Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed / fadeDuration
                );

            float steppedProgress =
                Mathf.Floor(
                    progress * fadeSteps
                ) / fadeSteps;

            fadeGroup.alpha =
                Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    steppedProgress
                );

            yield return null;
        }

        fadeGroup.alpha =
            targetAlpha;
    }
}