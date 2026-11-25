using UnityEngine;

public class SceneSpawnPoint : MonoBehaviour
{
    [SerializeField] private string spawnID;

    public string SpawnID
    {
        get { return spawnID; }
    }
}