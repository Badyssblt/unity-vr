using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Damage Settings")]
    [SerializeField] private bool isInvulnerable = false;
    [SerializeField] private float invulnerabilityDuration = 0f;
    private float invulnerabilityTimer = 0f;

    [Header("Team Settings")]
    [SerializeField] private string teamTag = "Player"; // "Player" ou "Enemy"

    [Header("Events")]
    public UnityEvent<float> onHealthChanged; // Passe la health actuelle
    public UnityEvent<float, float> onDamageTaken; // Passe les dégâts et la health restante
    public UnityEvent onDeath;
    public UnityEvent<bool> onDeathWithType; // Passe true si headshot, false sinon
    public UnityEvent onRevive;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    // Track last hit type for death animation
    private bool lastHitWasHeadshot = false;
    private bool isDead = false;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0;
    public bool IsInvulnerable => isInvulnerable || invulnerabilityTimer > 0;
    public string TeamTag => teamTag;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Gérer l'invulnérabilité temporaire
        if (invulnerabilityTimer > 0)
        {
            invulnerabilityTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Infliger des dégâts à cette entité
    /// </summary>
    public void TakeDamage(float damage, string attackerTeam = "", bool isHeadshot = false)
    {
        // Ne pas prendre de dégâts si mort
        if (!IsAlive)
        {
            if (showDebugLogs) Debug.LogWarning($"⚠️ {gameObject.name} est déjà mort!");
            return;
        }

        // Ne pas prendre de dégâts si invulnérable
        if (IsInvulnerable)
        {
            if (showDebugLogs) Debug.Log($"🛡️ {gameObject.name} est invulnérable!");
            return;
        }

        // Système de team désactivé - tout le monde peut se tirer dessus

        // Tracker le type de hit pour l'animation de mort
        lastHitWasHeadshot = isHeadshot;

        // Appliquer les dégâts
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // Ne pas descendre en dessous de 0

        if (showDebugLogs)
            Debug.Log($"💥 {gameObject.name} prend {damage} dégâts | PV: {currentHealth}/{maxHealth}");

        // Déclencher les événements
        onHealthChanged?.Invoke(currentHealth);
        onDamageTaken?.Invoke(damage, currentHealth);

        // Vérifier la mort
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Soigner cette entité
    /// </summary>
    public void Heal(float amount)
    {
        if (!IsAlive) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Ne pas dépasser maxHealth

        if (showDebugLogs)
            Debug.Log($"💚 {gameObject.name} se soigne de {amount} | PV: {currentHealth}/{maxHealth}");

        onHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// Activer l'invulnérabilité temporaire
    /// </summary>
    public void SetInvulnerable(float duration)
    {
        invulnerabilityTimer = duration;
        if (showDebugLogs)
            Debug.Log($"🛡️ {gameObject.name} invulnérable pendant {duration}s");
    }

    /// <summary>
    /// Mort de l'entité
    /// </summary>
    void Die()
    {
        if (isDead) return; // Empêche Die() de se déclencher plusieurs fois
        isDead = true;

        if (showDebugLogs)
        {
            string deathType = lastHitWasHeadshot ? "HEADSHOT \U0001f3af" : "bodyshot";
            Debug.Log($"💀 {gameObject.name} est mort! ({deathType})");
        }

        onDeath?.Invoke();
        onDeathWithType?.Invoke(lastHitWasHeadshot);

        // Ne pas détruire le component, juste invoquer les events
        // Le GameObject parent (Enemy AI) sera détruit par AIController après l'animation
    }

    /// <summary>
    /// Réanimer l'entité
    /// </summary>
    public void Revive(float healthAmount = -1)
    {
        if (healthAmount < 0)
            currentHealth = maxHealth; // Réanimer à pleine santé
        else
            currentHealth = Mathf.Min(healthAmount, maxHealth);

        if (showDebugLogs)
            Debug.Log($"✨ {gameObject.name} réanimé avec {currentHealth} PV");

        onRevive?.Invoke();
        onHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// Tuer instantanément (ignore l'invulnérabilité)
    /// </summary>
    public void InstantKill(bool isHeadshot = true)
    {
        lastHitWasHeadshot = isHeadshot;
        currentHealth = 0;
        Die();
    }

    /// <summary>
    /// Reset la santé à son maximum
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth);
    }

    // Gizmo pour visualiser la santé dans la Scene view
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // Barre de vie au-dessus de l'entité
        Vector3 position = transform.position + Vector3.up * 2f;
        float healthPercent = currentHealth / maxHealth;

        // Barre rouge (fond)
        Gizmos.color = Color.red;
        Gizmos.DrawCube(position, new Vector3(1f, 0.1f, 0.01f));

        // Barre verte (santé actuelle)
        Gizmos.color = Color.green;
        Vector3 healthBarSize = new Vector3(healthPercent, 0.1f, 0.01f);
        Vector3 healthBarPos = position + Vector3.left * (1f - healthPercent) * 0.5f;
        Gizmos.DrawCube(healthBarPos, healthBarSize);
    }
}
