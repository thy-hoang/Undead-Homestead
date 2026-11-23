
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    [SerializeField] private int fadeSteps = 10;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip transitionSound;

    private bool isTransitioning;

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
        StartCoroutine(Fade(0f));
    }

    public void LoadScene(string sceneName)
    {
        if (isTransitioning) return;

        isTransitioning = true;
        StartCoroutine(Transition(sceneName));
    }

    private IEnumerator Transition(string sceneName)
    {
        

        // Fade out
        yield return StartCoroutine(Fade(1f));

        // Play transition sound
        audioSource.PlayOneShot(transitionSound);

        // Load scene
        SceneManager.LoadScene(sceneName);

        // Fade in
        yield return StartCoroutine(Fade(0f));

        isTransitioning = false;
    }


    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / fadeDuration);

            // Quantize the progress into discrete steps
            float steppedProgress = Mathf.Floor(progress * fadeSteps) / fadeSteps;

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
