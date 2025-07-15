using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    public static SpawnPointManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Transform[] spawnPoint;

    public Transform GetSpawnPoint(int index)
    {
        if (index < 0 || index >= spawnPoint.Length)
        {
            Debug.LogWarning($"Invalid spawn point index {index}. Using default position.");
            return null; // or return a default spawn point
        }
        return spawnPoint[index];
    }
}
