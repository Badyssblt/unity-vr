using UnityEngine;

/// <summary>
/// Hitbox avec multiplicateur de dégâts
/// Attachez ce script aux colliders (tête, corps, bras, jambes) pour gérer les dégâts différemment
/// </summary>
[RequireComponent(typeof(Collider))]
public class DamageableHitbox : MonoBehaviour
{
    [Header("Hitbox Settings")]
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private HitboxType hitboxType = HitboxType.Body;
    [SerializeField] private float damageMultiplier = 1f;
    [SerializeField] private bool headshotInstantKill = true; // One shot headshot

    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private float hitSoundVolume = 1f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    public enum HitboxType
    {
        Head,       // Headshot
        Body,       // Corps
        Limb        // Bras/Jambes
    }

    void Awake()
    {
        // Auto-trouver le HealthSystem sur le parent si non assigné
        if (healthSystem == null)
        {
            healthSystem = GetComponentInParent<HealthSystem>();
        }

        if (healthSystem == null)
        {
            Debug.LogError($"❌ {gameObject.name} : Aucun HealthSystem trouvé! Ajoutez HealthSystem au parent.");
        }

        // Configurer les multiplicateurs par défaut selon le type
        if (damageMultiplier == 1f)
        {
            switch (hitboxType)
            {
                case HitboxType.Head:
                    damageMultiplier = 2f; // Headshot = 2x dégâts
                    break;
                case HitboxType.Body:
                    damageMultiplier = 1f; // Dégâts normaux
                    break;
                case HitboxType.Limb:
                    damageMultiplier = 0.75f; // Membres = 75% dégâts
                    break;
            }
        }
    }

    /// <summary>
    /// Appeler cette méthode pour infliger des dégâts à cette hitbox
    /// </summary>
    public void TakeDamage(float baseDamage, string attackerTeam = "")
    {
        if (healthSystem == null)
        {
            Debug.LogError($"❌ {gameObject.name} : HealthSystem manquant!");
            return;
        }

        // Jouer le son de hit
        PlayHitSound();

        // Déterminer si c'est un headshot
        bool isHeadshot = (hitboxType == HitboxType.Head);

        // One shot headshot
        if (isHeadshot && headshotInstantKill)
        {
            if (showDebugLogs)
            {
                Debug.Log($"🎯 HEADSHOT INSTANT KILL! {gameObject.name}");
            }
            healthSystem.InstantKill();
            return;
        }

        // Calculer les dégâts avec multiplicateur
        float finalDamage = baseDamage * damageMultiplier;

        if (showDebugLogs)
        {
            string hitMessage = isHeadshot ? "🎯 HEADSHOT!" : "💥 Hit";
            Debug.Log($"{hitMessage} {gameObject.name} | Dégâts: {baseDamage} x {damageMultiplier} = {finalDamage}");
        }

        // Envoyer les dégâts au HealthSystem avec l'info headshot
        healthSystem.TakeDamage(finalDamage, attackerTeam, isHeadshot);
    }

    void PlayHitSound()
    {
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position, hitSoundVolume);
        }
    }

    /// <summary>
    /// Obtenir le type de hitbox
    /// </summary>
    public HitboxType GetHitboxType()
    {
        return hitboxType;
    }

    /// <summary>
    /// Obtenir le multiplicateur de dégâts
    /// </summary>
    public float GetDamageMultiplier()
    {
        return damageMultiplier;
    }

    // Visualisation dans la Scene view
    void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col == null) return;

        // Couleur selon le type de hitbox
        switch (hitboxType)
        {
            case HitboxType.Head:
                Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // Rouge (headshot)
                break;
            case HitboxType.Body:
                Gizmos.color = new Color(1f, 1f, 0f, 0.3f); // Jaune (corps)
                break;
            case HitboxType.Limb:
                Gizmos.color = new Color(0f, 1f, 0f, 0.3f); // Vert (membres)
                break;
        }

        // Dessiner le collider
        if (col is BoxCollider box)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
        }
        else if (col is SphereCollider sphere)
        {
            Gizmos.DrawSphere(transform.position + sphere.center, sphere.radius);
        }
        else if (col is CapsuleCollider capsule)
        {
            Gizmos.DrawSphere(transform.position + capsule.center, capsule.radius);
        }
    }
}
