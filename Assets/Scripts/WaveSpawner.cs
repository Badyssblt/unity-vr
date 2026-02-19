using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Wave Settings")]
    [SerializeField] private int enemiesPerWaveBase = 3;
    [SerializeField] private int enemiesIncreasePerWave = 2;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float timeBetweenSpawns = 0.5f;
    [SerializeField] private int maxWaves = 5;

    [Header("State")]
    private int currentWave = 0;
    private int enemiesAliveInWave = 0;
    private bool isSpawning = false;

    public int CurrentWave => currentWave;
    public int MaxWaves => maxWaves;
    public int EnemiesAliveInWave => enemiesAliveInWave;
    public bool AllWavesCompleted => currentWave >= maxWaves && enemiesAliveInWave <= 0;

    public void StartWaves()
    {
        currentWave = 0;
        StartNextWave();
    }

    void StartNextWave()
    {
        if (currentWave >= maxWaves) return;

        currentWave++;
        int enemyCount = enemiesPerWaveBase + (currentWave - 1) * enemiesIncreasePerWave;
        enemiesAliveInWave = enemyCount;

        Debug.Log($"=== Vague {currentWave}/{maxWaves} : {enemyCount} ennemis ===");

        StartCoroutine(SpawnWave(enemyCount));
    }

    IEnumerator SpawnWave(int count)
    {
        isSpawning = true;

        for (int i = 0; i < count; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        isSpawning = false;
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0)
        {
            Debug.LogWarning("WaveSpawner: No enemy prefabs or spawn points assigned!");
            return;
        }

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject enemy = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

        // S'abonner à la mort de l'ennemi
        HealthSystem health = enemy.GetComponent<HealthSystem>();
        if (health != null)
        {
            health.onDeath.AddListener(() => OnEnemyKilledInWave());
        }
    }

    public void OnEnemyKilledInWave()
    {
        enemiesAliveInWave--;

        Debug.Log($"Ennemi tué dans la vague {currentWave}. Restants: {enemiesAliveInWave}");

        if (enemiesAliveInWave <= 0 && !isSpawning)
        {
            // Heal le joueur à la fin de chaque vague
            HealPlayer();

            if (currentWave >= maxWaves)
            {
                // Toutes les vagues terminées
                Debug.Log("=== Toutes les vagues sont terminées! ===");
                if (GameManager.Instance != null)
                    GameManager.Instance.OnAllWavesCompleted();
            }
            else
            {
                // Lancer la prochaine vague après un délai
                StartCoroutine(StartNextWaveAfterDelay());
            }
        }
    }

    IEnumerator StartNextWaveAfterDelay()
    {
        Debug.Log($"Prochaine vague dans {timeBetweenWaves}s...");
        yield return new WaitForSeconds(timeBetweenWaves);
        StartNextWave();
    }

    void HealPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            HealthSystem playerHealth = playerObj.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.ResetHealth();
                Debug.Log("Joueur soigné à la fin de la vague!");
            }
        }
    }

    public int GetEnemiesInWave(int wave)
    {
        return enemiesPerWaveBase + (wave - 1) * enemiesIncreasePerWave;
    }
}
