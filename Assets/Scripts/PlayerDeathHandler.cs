using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Gère la mort et le respawn du joueur en VR
/// </summary>
[RequireComponent(typeof(HealthSystem))]
public class PlayerDeathHandler : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay = 3f;
    [SerializeField] private bool autoRespawn = true;
    [SerializeField] private bool resetScene = false; // Recharger la scène au lieu de respawn

    [Header("Death Effects")]
    [SerializeField] private GameObject deathScreenUI; // UI de mort (optionnel)
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private bool fadeToBlack = true;
    [SerializeField] private float fadeDuration = 2f;

    [Header("XR Settings")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private bool disableMovementOnDeath = true;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private HealthSystem healthSystem;
    private AudioSource audioSource;
    private bool isDead = false;

    void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        audioSource = GetComponent<AudioSource>();

        // Auto-trouver le CharacterController si non assigné
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        // S'abonner à l'événement de mort
        if (healthSystem != null)
        {
            healthSystem.onDeath.AddListener(OnPlayerDeath);
        }
    }

    void Start()
    {
        // Sauvegarder le point de respawn initial si non défini
        if (respawnPoint == null)
        {
            GameObject respawnObj = new GameObject("PlayerRespawnPoint");
            respawnPoint = respawnObj.transform;
            respawnPoint.position = transform.position;
            respawnPoint.rotation = transform.rotation;
        }

        // Cacher l'UI de mort au démarrage
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(false);
        }
    }

    void OnPlayerDeath()
    {
        if (isDead) return; // Éviter les appels multiples

        isDead = true;

        if (showDebugLogs)
            Debug.Log("💀 Le joueur est mort!");

        // Jouer le son de mort
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Afficher l'écran de mort
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(true);
        }

        // Désactiver le mouvement
        if (disableMovementOnDeath && characterController != null)
        {
            characterController.enabled = false;
        }

        // Fade to black (si activé)
        if (fadeToBlack)
        {
            StartCoroutine(FadeToBlackCoroutine());
        }

        // Auto-respawn ou reset
        if (autoRespawn)
        {
            if (resetScene)
            {
                StartCoroutine(ResetSceneCoroutine());
            }
            else
            {
                StartCoroutine(RespawnCoroutine());
            }
        }
    }

    IEnumerator FadeToBlackCoroutine()
    {
        // TODO: Implémenter le fade to black avec un Canvas noir
        // Pour l'instant, juste un délai
        yield return new WaitForSeconds(fadeDuration);
    }

    IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        Respawn();
    }

    IEnumerator ResetSceneCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (showDebugLogs)
            Debug.Log("🔄 Rechargement de la scène...");

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Respawn()
    {
        if (showDebugLogs)
            Debug.Log("✨ Le joueur respawn!");

        // Réinitialiser la santé
        if (healthSystem != null)
        {
            healthSystem.Revive();
        }

        // Téléporter au point de respawn
        if (respawnPoint != null)
        {
            // Désactiver le CharacterController temporairement pour téléporter
            if (characterController != null)
            {
                characterController.enabled = false;
            }

            transform.position = respawnPoint.position;
            transform.rotation = respawnPoint.rotation;

            // Réactiver le CharacterController
            if (characterController != null)
            {
                characterController.enabled = true;
            }
        }

        // Cacher l'UI de mort
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(false);
        }

        isDead = false;
    }

    /// <summary>
    /// Définir un nouveau point de respawn
    /// </summary>
    public void SetRespawnPoint(Transform newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;

        if (showDebugLogs)
            Debug.Log($"✅ Nouveau point de respawn: {respawnPoint.position}");
    }

    /// <summary>
    /// Définir un nouveau point de respawn à la position actuelle
    /// </summary>
    public void SetRespawnPointHere()
    {
        if (respawnPoint == null)
        {
            GameObject respawnObj = new GameObject("PlayerRespawnPoint");
            respawnPoint = respawnObj.transform;
        }

        respawnPoint.position = transform.position;
        respawnPoint.rotation = transform.rotation;

        if (showDebugLogs)
            Debug.Log($"✅ Point de respawn mis à jour: {respawnPoint.position}");
    }
}
