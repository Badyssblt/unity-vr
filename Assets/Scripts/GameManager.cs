using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    [SerializeField] private int pointsPerKill = 10; // Points par ennemi tué
    [SerializeField] private int headshotBonus = 5; // Bonus pour headshot

    [Header("Wave System")]
    [SerializeField] private WaveSpawner waveSpawner;

    [Header("Game State")]
    private int currentScore = 0;
    private int enemiesKilled = 0;
    private GameState gameState = GameState.Menu;

    [Header("UI References")]
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject winUI;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI enemiesText;
    [SerializeField] private TextMeshProUGUI waveText;
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
        if (scoreText == null)
            Debug.LogWarning("GameManager: scoreText n'est pas assigné dans l'Inspector!");
        if (enemiesText == null)
            Debug.LogWarning("GameManager: enemiesText n'est pas assigné dans l'Inspector!");

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
            enemiesText.text = "Kills: " + enemiesKilled;

        if (waveText != null && waveSpawner != null)
            waveText.text = string.Format("Vague {0}/{1}", waveSpawner.CurrentWave, waveSpawner.MaxWaves);
    }

    public void AddScore(int points)
    {
        if (gameState == GameState.Playing)
        {
            currentScore += points;
            UpdateUI();
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

        Debug.Log($"Enemy killed! Total: {enemiesKilled} | Score: {currentScore} | Headshot: {isHeadshot}");

        UpdateUI();
    }

    public void StartGame()
    {
        currentScore = 0;
        enemiesKilled = 0;
        SetGameState(GameState.Playing);

        // Démarrer les vagues
        if (waveSpawner != null)
            waveSpawner.StartWaves();

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
            finalKillsText.text = "Enemies Killed: " + enemiesKilled;

        // Play game over music/sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameOverMusic();
            AudioManager.Instance.PlayGameOver();
        }
    }

    public void OnAllWavesCompleted()
    {
        WinGame();
    }

    public void WinGame()
    {
        Debug.Log($"=== WIN GAME CALLED === enemiesKilled: {enemiesKilled} | Score: {currentScore}");

        if (winUI == null)
            Debug.LogWarning("GameManager: winUI n'est pas assigné dans l'Inspector!");
        if (winScoreText == null)
            Debug.LogWarning("GameManager: winScoreText n'est pas assigné dans l'Inspector!");
        if (winKillsText == null)
            Debug.LogWarning("GameManager: winKillsText n'est pas assigné dans l'Inspector!");

        // Mettre à jour les textes AVANT de changer l'état (pour qu'ils soient prêts)
        if (winScoreText != null)
            winScoreText.text = "Final Score: " + currentScore;

        if (winKillsText != null)
            winKillsText.text = "Enemies Killed: " + enemiesKilled;

        SetGameState(GameState.Win);

        Debug.Log($"Victory! Final Score: {currentScore} | Enemies Killed: {enemiesKilled}");

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
    public WaveSpawner GetWaveSpawner() => waveSpawner;
    public bool IsGameActive() => gameState == GameState.Playing;
    public bool HasWon() => gameState == GameState.Win;
}
