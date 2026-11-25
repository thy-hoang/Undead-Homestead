using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string targetSpawnName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSpawner.nextSpawnPoint = targetSpawnName;
            SceneTransitionManager.Instance.LoadScene(sceneToLoad);
        }
    }
}