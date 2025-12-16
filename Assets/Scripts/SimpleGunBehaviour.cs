using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Version simplifiée du comportement d'arme
/// Peut être appelé soit:
/// 1. Par l'événement "Activated" du XR Grab Interactable
/// 2. Automatiquement avec le clavier (si enableKeyboardInput = true)
/// </summary>
public class SimpleGunBehaviour : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private bool enableKeyboardInput = true;

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 20f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;

    void Update()
    {
        // Tir au clavier pour test (XR Device Simulator)
        if (enableKeyboardInput && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    /// <summary>
    /// Cette méthode peut être appelée:
    /// - Par l'événement "Activated" du XR Grab Interactable
    /// - Automatiquement par Update() si enableKeyboardInput = true
    /// - Manuellement par un autre script
    /// </summary>
    public void Shoot()
    {
        Debug.Log("💥 BANG!");

        // Auto-trouver FirePoint si pas assigné
        if (firePoint == null)
        {
            Transform fp = transform.Find("FirePoint");
            if (fp != null)
                firePoint = fp;
            else
                firePoint = transform; // Utiliser le transform de l'arme
        }

        // Spawn bullet
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = firePoint.forward * bulletSpeed;
            }
            Destroy(bullet, 5f);
        }
        else
        {
            Debug.LogWarning("⚠️ Pas de bulletPrefab ou firePoint assigné - tir simulé");
        }

        // Muzzle flash
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // Sound
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
}
