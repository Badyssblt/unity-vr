using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float damage = 25f;
    [SerializeField] private string ownerTeam = "Player"; // "Player" ou "Enemy"

    [Header("Visual Effects")]
    [SerializeField] private GameObject impactEffect;

    [Header("Physics")]
    [SerializeField] private LayerMask collisionMask;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// Configurer le projectile (appelé par l'arme qui tire)
    /// </summary>
    public void Initialize(float damage, string ownerTeam)
    {
        this.damage = damage;
        this.ownerTeam = ownerTeam;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (showDebugLogs)
            Debug.Log($"🎯 Projectile a touché: {collision.gameObject.name}");

        // Spawn impact effect
        if (impactEffect != null)
        {
            ContactPoint contact = collision.contacts[0];
            GameObject effect = Instantiate(impactEffect, contact.point, Quaternion.LookRotation(contact.normal));
            Destroy(effect, 2f);
        }

        // NOUVEAU SYSTÈME DE DÉGÂTS avec Hitbox
        // Priorité 1 : Chercher DamageableHitbox (pour headshots)
        DamageableHitbox hitbox = collision.gameObject.GetComponent<DamageableHitbox>();
        if (hitbox != null)
        {
            hitbox.TakeDamage(damage, ownerTeam);
        }
        else
        {
            // Priorité 2 : Chercher HealthSystem directement
            HealthSystem healthSystem = collision.gameObject.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(damage, ownerTeam);
            }
            else
            {
                // Priorité 3 : Ancien système Target (pour compatibilité)
                Target target = collision.gameObject.GetComponent<Target>();
                if (target != null)
                {
                    target.TakeDamage();
                }
            }
        }

        // Destroy bullet
        Destroy(gameObject);
    }
}
