using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    [SerializeField] private int totalEnemies = 20; // Nombre d'ennemis à tuer pour gagner
    [SerializeField] private int pointsPerKill = 10; // Points par ennemi tué
    [SerializeField] private int headshotBonus = 5; // Bonus pour headshot

    [Header("Game State")]
    private int currentScore = 0;
    private int enemiesKilled = 0;
    private bool isGameActive = false;
    private GameState gameState = GameState.Menu;

    [Header("UI References")]
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject winUI;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI enemiesText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalKillsText;
    [SerializeField] private TextMeshProUGUI winScoreText;
    [SerializeField] private TextMeshProUGUI winKillsText;
    [SerializeField] private GameObject crosshairUI;

    public enum GameState
    {
        Menu,
        Playing,
        GameOver,
        Win
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
    }

    void Update()
    {
        if (gameState == GameState.Playing)
        {
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + currentScore;

        if (enemiesText != null)
        {
            int remaining = totalEnemies - enemiesKilled;
            enemiesText.text = string.Format("Enemies: {0}/{1}", enemiesKilled, totalEnemies);
        }
    }

    public void AddScore(int points)
    {
        if (gameState == GameState.Playing)
        {
            currentScore += points;
        }
    }

    public void OnEnemyKilled(bool isHeadshot = false)
    {
        if (gameState != GameState.Playing) return;

        enemiesKilled++;

        // Ajouter les points
        int points = pointsPerKill;
        if (isHeadshot)
            points += headshotBonus;

        AddScore(points);

        Debug.Log($"Enemy killed! Total: {enemiesKilled}/{totalEnemies} | Score: {currentScore} | Headshot: {isHeadshot}");

        // Vérifier si tous les ennemis sont tués
        if (enemiesKilled >= totalEnemies)
        {
            WinGame();
        }
    }

    public void StartGame()
    {
        currentScore = 0;
        enemiesKilled = 0;
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

        if (finalKillsText != null)
            finalKillsText.text = string.Format("Enemies Killed: {0}/{1}", enemiesKilled, totalEnemies);

        // Play game over music/sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameOverMusic();
            AudioManager.Instance.PlayGameOver();
        }
    }

    public void WinGame()
    {
        SetGameState(GameState.Win);

        if (winScoreText != null)
            winScoreText.text = "Final Score: " + currentScore;

        if (winKillsText != null)
            winKillsText.text = string.Format("Enemies Killed: {0}/{1}", enemiesKilled, totalEnemies);

        Debug.Log($"Victory! Final Score: {currentScore} | Enemies Killed: {enemiesKilled}/{totalEnemies}");

        // Play victory music/sound (you can add a PlayVictoryMusic method in AudioManager)
        if (AudioManager.Instance != null)
        {
            // AudioManager.Instance.PlayVictoryMusic();
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

        if (winUI != null)
            winUI.SetActive(gameState == GameState.Win);

        if (crosshairUI != null)
            crosshairUI.SetActive(gameState == GameState.Playing);
    }

    // Public getters
    public int GetScore() => currentScore;
    public int GetEnemiesKilled() => enemiesKilled;
    public int GetTotalEnemies() => totalEnemies;
    public bool IsGameActive() => gameState == GameState.Playing;
    public bool HasWon() => gameState == GameState.Win;
}
