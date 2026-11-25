using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    // Remembers the spawn point name across different scenes
    public static string nextSpawnPoint;

    void Start()
    {
        if (!string.IsNullOrEmpty(nextSpawnPoint))
        {
            // Find the empty GameObject by name
            GameObject spawnPoint = GameObject.Find(nextSpawnPoint);
            
            if (spawnPoint != null)
            {
                // Move the player to that position
                transform.position = spawnPoint.transform.position;
            }

            // Clear it so normal game starts don't use it by mistake
            nextSpawnPoint = "";
        }
    }
}
