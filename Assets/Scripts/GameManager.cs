using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 60f; // Timer in seconds
    [SerializeField] private int targetScore = 100;

    [Header("Game State")]
    private int currentScore = 0;
    private float timeRemaining;
    private bool isGameActive = false;
    private GameState gameState = GameState.Menu;

    [Header("UI References")]
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Header("Spawning")]
    [SerializeField] private GameObject[] targetPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 2f;
    private float nextSpawnTime;

    public enum GameState
    {
        Menu,
        Playing,
        GameOver
    }

    void Awake()
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

    void Start()
    {
        SetGameState(GameState.Menu);
        timeRemaining = gameDuration;
    }

    void Update()
    {
        if (gameState == GameState.Playing)
        {
            UpdateTimer();
            UpdateSpawning();
            UpdateUI();
        }
    }

    void UpdateTimer()
    {
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            EndGame();
        }
    }

    void UpdateSpawning()
    {
        if (Time.time >= nextSpawnTime && targetPrefabs.Length > 0 && spawnPoints.Length > 0)
        {
            SpawnTarget();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnTarget()
    {
        // Random target prefab
        GameObject targetPrefab = targetPrefabs[Random.Range(0, targetPrefabs.Length)];

        // Random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Instantiate(targetPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + currentScore;

        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }

    public void AddScore(int points)
    {
        if (gameState == GameState.Playing)
        {
            currentScore += points;
        }
    }

    public void StartGame()
    {
        currentScore = 0;
        timeRemaining = gameDuration;
        SetGameState(GameState.Playing);

        // Play gameplay music
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayGameplayMusic();
    }

    public void EndGame()
    {
        SetGameState(GameState.GameOver);

        if (finalScoreText != null)
            finalScoreText.text = "Final Score: " + currentScore;

        // Play game over music/sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameOverMusic();
            AudioManager.Instance.PlayGameOver();
        }

        // Clean up all remaining targets
        Target[] targets = FindObjectsOfType<Target>();
        foreach (Target target in targets)
        {
            Destroy(target.gameObject);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    void SetGameState(GameState newState)
    {
        gameState = newState;

        // Update UI visibility
        if (menuUI != null)
            menuUI.SetActive(gameState == GameState.Menu);

        if (gameUI != null)
            gameUI.SetActive(gameState == GameState.Playing);

        if (gameOverUI != null)
            gameOverUI.SetActive(gameState == GameState.GameOver);
    }

    // Public getters
    public int GetScore() => currentScore;
    public float GetTimeRemaining() => timeRemaining;
    public bool IsGameActive() => gameState == GameState.Playing;
}
