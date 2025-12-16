using UnityEngine;

/// <summary>
/// Alternative au spawning via GameManager - Spawner autonome pour les cibles
/// </summary>
public class TargetSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] targetPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxTargetsAtOnce = 5;
    [SerializeField] private bool spawnOnStart = true;

    private float nextSpawnTime;
    private int currentTargetCount = 0;

    void Start()
    {
        if (spawnOnStart)
        {
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void Update()
    {
        // Only spawn if game is active
        if (GameManager.Instance != null && !GameManager.Instance.IsGameActive())
            return;

        if (Time.time >= nextSpawnTime && currentTargetCount < maxTargetsAtOnce)
        {
            SpawnTarget();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnTarget()
    {
        if (targetPrefabs.Length == 0 || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No target prefabs or spawn points assigned!");
            return;
        }

        // Random target prefab
        GameObject targetPrefab = targetPrefabs[Random.Range(0, targetPrefabs.Length)];

        // Random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject target = Instantiate(targetPrefab, spawnPoint.position, spawnPoint.rotation);

        // Subscribe to target destruction
        Target targetScript = target.GetComponent<Target>();
        if (targetScript != null)
        {
            currentTargetCount++;
            // Decrease count when destroyed
            target.GetComponent<Target>().enabled = true;
        }
    }

    // Call this when a target is destroyed
    public void OnTargetDestroyed()
    {
        currentTargetCount--;
    }
}
