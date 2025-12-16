using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private int maxAmmo = 10;
    [SerializeField] private int currentAmmo;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float reloadTime = 2f;

    [Header("Raycast Settings")]
    [SerializeField] private bool useRaycast = true;
    [SerializeField] private float raycastMaxDistance = 100f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Transform firePoint;

    [Header("Direction Settings")]
    [SerializeField] private ShootDirection shootDirection = ShootDirection.Forward;
    [SerializeField] private bool showDebugRaycast = true; // Toggle pour afficher/masquer les gizmos

    public enum ShootDirection
    {
        Forward,        // firePoint.forward
        Backward,       // -firePoint.forward
        Right,          // firePoint.right
        Left,           // -firePoint.right
        Up,             // firePoint.up
        Down            // -firePoint.up
    }

    [Header("Projectile Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 50f;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private LineRenderer laserSight;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioClip emptySound;

    [Header("XR Interaction")]
    [SerializeField] private XRGrabInteractable grabInteractable;
    [SerializeField] private InputActionProperty shootAction;

    [Header("Two-Handed Weapon")]
    [SerializeField] private bool requireTwoHands = true; // Nécessite les deux mains pour tirer
    [SerializeField] private bool requireBothTriggers = true; // Nécessite les deux triggers pressés

    [Header("Haptic Feedback")]
    [SerializeField] private bool useHaptics = true;
    [SerializeField] private float shootHapticIntensity = 0.3f;
    [SerializeField] private float shootHapticDuration = 0.1f;

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI currentAmmoText;
    [SerializeField] private TextMeshProUGUI maxAmmoText;
    [SerializeField] private GameObject ammoParent;

    [Header("VR Reload System")]
    [SerializeField] private bool useVRReload = true;
    [SerializeField] private MagazineSocket magazineSocket; // Socket pour insérer le chargeur
    [SerializeField] private KeyCode ejectMagazineKey = KeyCode.E;

    private float nextFireTime;
    private bool isReloading;
    private XRBaseControllerInteractor primaryInteractor;   // Main principale (grip)
    private XRBaseControllerInteractor secondaryInteractor; // Main secondaire (foregrip)
    private List<XRBaseControllerInteractor> interactors = new List<XRBaseControllerInteractor>();

    void Start()
    {
        currentAmmo = maxAmmo;

        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Auto-trouver le firePoint si non assigné
        if (firePoint == null)
        {
            Transform bulletSpawn = transform.Find("FirePoint");
            if (bulletSpawn != null)
            {
                firePoint = bulletSpawn;
                Debug.Log("✅ FirePoint trouvé: FirePoint");
            }
            else
            {
                firePoint = transform;
                Debug.LogWarning("⚠️ FirePoint non assigné! Utilise transform de l'arme");
            }
        }

        // IMPORTANT: Activer l'action Input
        if (shootAction.action != null)
        {
            shootAction.action.Enable();
            Debug.Log($"✅ Shoot Action activée: {shootAction.action.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ Shoot Action est NULL!");
        }

        // Subscribe to grab events for haptics
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }

        // Subscribe to magazine socket events for VR reload
        if (useVRReload && magazineSocket != null)
        {
            magazineSocket.onMagazineInserted.AddListener(OnMagazineInserted);
            magazineSocket.onMagazineEjected.AddListener(OnMagazineEjected);
            Debug.Log("✅ MagazineSocket connecté au WeaponController");
        }
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        XRBaseControllerInteractor interactor = args.interactorObject as XRBaseControllerInteractor;

        if (interactor != null)
        {
            // Ajouter l'interactor à la liste
            if (!interactors.Contains(interactor))
            {
                interactors.Add(interactor);
            }

            // Déterminer quelle est la main principale et secondaire
            if (primaryInteractor == null)
            {
                primaryInteractor = interactor;
                Debug.Log("✋ Main PRINCIPALE grabbée");
            }
            else if (secondaryInteractor == null)
            {
                secondaryInteractor = interactor;
                Debug.Log("✋ Main SECONDAIRE grabbée - ARME À DEUX MAINS ACTIVE");
            }
        }

        // Afficher l'UI seulement si au moins une main tient l'arme
        if (ammoParent != null)
        {
            ammoParent.SetActive(true);
            currentAmmoText.text = currentAmmo.ToString();
            maxAmmoText.text = maxAmmo.ToString();
        }
    }

    public void UpdateAmmoUI()
    {
         currentAmmoText.text = currentAmmo.ToString();
         maxAmmoText.text = maxAmmo.ToString();
        
    }

    void OnReleased(SelectExitEventArgs args)
    {
        XRBaseControllerInteractor interactor = args.interactorObject as XRBaseControllerInteractor;

        if (interactor != null)
        {
            // Retirer l'interactor de la liste
            interactors.Remove(interactor);

            // Mettre à jour les références
            if (interactor == primaryInteractor)
            {
                primaryInteractor = secondaryInteractor; // La secondaire devient principale
                secondaryInteractor = null;
                Debug.Log("✋ Main principale relâchée");
            }
            else if (interactor == secondaryInteractor)
            {
                secondaryInteractor = null;
                Debug.Log("✋ Main secondaire relâchée");
            }
        }

        // Cacher l'UI si aucune main ne tient l'arme
        if (interactors.Count == 0 && ammoParent != null)
        {
            ammoParent.SetActive(false);
        }
    }

    void OnMagazineInserted(GameObject magazine)
    {
        Debug.Log($"✅ Chargeur inséré: {magazine.name} - Rechargement automatique !");

        // Recharger l'arme instantanément
        currentAmmo = maxAmmo;
        isReloading = false;

        // Jouer le son de rechargement
        PlaySound(reloadSound);

        // Mettre à jour l'UI
        UpdateAmmoUI();

        // Feedback haptique
        TriggerHaptic(0.2f, 0.1f);
    }

    void OnMagazineEjected()
    {
        Debug.Log("❌ Chargeur éjecté !");

        // Optionnel : vider l'arme quand le chargeur est retiré
        // currentAmmo = 0;
        // UpdateAmmoUI();
    }

    void Update()
    {
        // Only allow shooting when weapon is grabbed
        if (grabInteractable != null && grabInteractable.isSelected)
        {
            // Vérifier les conditions de tir
            if (CanShoot() && Time.time >= nextFireTime && !isReloading)
            {
                Shoot();
            }
        }

        // Update laser sight
        UpdateLaserSight();
    }

    bool CanShoot()
    {
        // Vérifier si les deux mains sont requises
        if (requireTwoHands)
        {
            if (primaryInteractor == null || secondaryInteractor == null)
            {
                Debug.LogWarning("⚠️ Les DEUX mains sont nécessaires pour tirer!");
                return false;
            }
        }
        else
        {
            // Au moins une main doit tenir l'arme
            if (primaryInteractor == null)
            {
                return false;
            }
        }

        // Vérifier si les deux triggers sont requis
        if (requireBothTriggers && requireTwoHands)
        {
            bool primaryTrigger = IsTriggerPressed(primaryInteractor);
            bool secondaryTrigger = IsTriggerPressed(secondaryInteractor);

            if (!primaryTrigger || !secondaryTrigger)
            {
                if (!primaryTrigger && !secondaryTrigger)
                {
                    // Aucun trigger pressé
                }
                else if (!primaryTrigger)
                {
                    Debug.LogWarning("⚠️ Appuyez sur le trigger de la main PRINCIPALE!");
                }
                else if (!secondaryTrigger)
                {
                    Debug.LogWarning("⚠️ Appuyez sur le trigger de la main SECONDAIRE!");
                }
                return false;
            }

            Debug.Log("✅ Les DEUX triggers sont pressés! TIR AUTORISÉ!");
            return true;
        }
        else
        {
            // Mode normal : un seul trigger suffit
            return IsTriggerPressed(primaryInteractor);
        }
    }

    bool IsTriggerPressed(XRBaseControllerInteractor interactor)
    {
        if (interactor == null) return false;

        // Utiliser ActionBasedController pour lire le trigger
        var actionController = interactor.GetComponent<ActionBasedController>();
        if (actionController != null)
        {
            var activateAction = actionController.activateAction;
            if (activateAction != null && activateAction.action != null)
            {
                float value = activateAction.action.ReadValue<float>();
                return value > 0.5f; // Trigger pressé à plus de 50%
            }
        }

        // Fallback: essayer avec le XRController directement
        var xrController = interactor.GetComponent<XRController>();
        if (xrController != null)
        {
            return xrController.activateInteractionState.activatedThisFrame ||
                   xrController.activateInteractionState.active;
        }

        return false;
    }

    public void Shoot()
    {
        if (currentAmmo <= 0)
        {
            PlaySound(emptySound);
            TriggerHaptic(0.1f, 0.05f); // Light haptic for empty
            return;
        }

        currentAmmo--;
        nextFireTime = Time.time + fireRate;

        currentAmmoText.text = currentAmmo.ToString();

        // Play muzzle flash
        if (muzzleFlash != null)
            muzzleFlash.Play();

        // Play shoot sound
        PlaySound(shootSound);

        // Trigger haptic feedback
        TriggerHaptic(shootHapticIntensity, shootHapticDuration);

        if (useRaycast)
        {
            ShootRaycast();
        }
        else
        {
            ShootProjectile();
        }

        // Auto reload when empty
        if (currentAmmo <= 0)
        {
            StartReload();
        }
    }

    void ShootRaycast()
    {
        if (firePoint == null)
        {
            Debug.LogError("❌ FirePoint est NULL!");
            return;
        }

        // Position de départ
        Vector3 rayOrigin = firePoint.position;

        // Obtenir la direction en fonction du setting
        Vector3 rayDirection = GetShootDirection();

        Debug.Log($"🔫 Tir | Direction: {shootDirection}");

        Ray ray = new Ray(rayOrigin, rayDirection);

        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit, raycastMaxDistance, targetLayer);

        // UN SEUL DrawRay au moment du tir (0.5 secondes)
        if (hasHit)
        {
            // Ligne verte jusqu'au point d'impact
            Debug.DrawLine(rayOrigin, hit.point, Color.green, 0.5f);
            Debug.Log($"🎯 TOUCHE! {hit.collider.name} à {hit.distance:F2}m");

            Target target = hit.collider.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage();
            }

            if (impactEffect != null)
            {
                GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }

            if (laserSight != null)
            {
                StartCoroutine(ShowShootEffect(hit.point));
            }
        }
        else
        {

            if (laserSight != null)
            {
                StartCoroutine(ShowShootEffect(rayOrigin + rayDirection * raycastMaxDistance));
            }
        }
    }

    // Obtenir la direction de tir en fonction du setting
    Vector3 GetShootDirection()
    {
        switch (shootDirection)
        {
            case ShootDirection.Forward:
                return firePoint.forward;
            case ShootDirection.Backward:
                return -firePoint.forward;
            case ShootDirection.Right:
                return firePoint.right;
            case ShootDirection.Left:
                return -firePoint.right;
            case ShootDirection.Up:
                return firePoint.up;
            case ShootDirection.Down:
                return -firePoint.up;
            default:
                return firePoint.forward;
        }
    }

    // Affiche un effet visuel du tir
    System.Collections.IEnumerator ShowShootEffect(Vector3 endPoint)
    {
        if (laserSight != null)
        {
            laserSight.enabled = true;
            laserSight.SetPosition(0, firePoint.position);
            laserSight.SetPosition(1, endPoint);

            yield return new WaitForSeconds(0.1f);

            laserSight.enabled = false;
        }
    }


    void ShootProjectile()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = firePoint.forward * bulletSpeed;
            }
        }
    }

    public void StartReload()
    {
        if (!isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    System.Collections.IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        PlaySound(reloadSound);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        
    }



    void UpdateLaserSight()
    {
        if (laserSight == null) return;

        // Show laser sight only when weapon is grabbed
        bool shouldShow = (grabInteractable != null && grabInteractable.isSelected);

        // Ne pas montrer pendant le tir pour éviter conflit
        if (shouldShow && firePoint != null)
        {
            laserSight.enabled = true;

            Vector3 rayOrigin = firePoint.position;
            Vector3 rayDirection = GetShootDirection(); // Utilise la même direction que le tir

            Ray ray = new Ray(rayOrigin, rayDirection);
            RaycastHit hit;

            Vector3 endPoint;
            if (Physics.Raycast(ray, out hit, raycastMaxDistance))
            {
                endPoint = hit.point;
            }
            else
            {
                endPoint = rayOrigin + rayDirection * raycastMaxDistance;
            }

            // S'assurer que le LineRenderer part bien du FirePoint
            laserSight.SetPosition(0, rayOrigin);
            laserSight.SetPosition(1, endPoint);
        }
        else
        {
            laserSight.enabled = false;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void TriggerHaptic(float intensity, float duration)
    {
        if (!useHaptics) return;

        // Envoyer le feedback haptique à la main principale
        if (primaryInteractor != null && primaryInteractor.xrController != null)
        {
            primaryInteractor.xrController.SendHapticImpulse(intensity, duration);
        }

        // Si deux mains sont utilisées, envoyer aussi à la main secondaire
        if (secondaryInteractor != null && secondaryInteractor.xrController != null)
        {
            secondaryInteractor.xrController.SendHapticImpulse(intensity * 0.5f, duration); // Moins fort sur la main secondaire
        }
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }

        // Se désabonner du magazine socket
        if (magazineSocket != null)
        {
            magazineSocket.onMagazineInserted.RemoveListener(OnMagazineInserted);
            magazineSocket.onMagazineEjected.RemoveListener(OnMagazineEjected);
        }
    }

    // Public getters for UI
    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;
    public bool IsReloading() => isReloading;

    // Visualisation du raycast dans la Scene view
    #if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (firePoint == null || !showDebugRaycast) return;

        Vector3 rayOrigin = firePoint.position;
        Vector3 rayDirection = GetShootDirection(); // Utilise la direction configurée

        // GROSSE sphère ROUGE au point de départ (FirePoint)
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(rayOrigin, 0.03f);

        // GROSSE flèche CYAN pour montrer la direction de tir configurée
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(rayOrigin, rayDirection * 0.5f); // Flèche de 50cm

        // Dessiner le raycast complet en MAGENTA (pour bien le voir)
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(rayOrigin, rayOrigin + rayDirection * raycastMaxDistance);

        // Dessiner TOUTES les directions possibles en gris transparent
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
        Gizmos.DrawRay(rayOrigin, firePoint.forward * 0.3f);
        Gizmos.DrawRay(rayOrigin, -firePoint.forward * 0.3f);
        Gizmos.DrawRay(rayOrigin, firePoint.right * 0.3f);
        Gizmos.DrawRay(rayOrigin, -firePoint.right * 0.3f);
        Gizmos.DrawRay(rayOrigin, firePoint.up * 0.3f);
        Gizmos.DrawRay(rayOrigin, -firePoint.up * 0.3f);

        // Faire un vrai raycast pour dessiner le point d'impact
        Ray ray = new Ray(rayOrigin, rayDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastMaxDistance, targetLayer))
        {
            // Point d'impact en vert
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hit.point, 0.05f);

            // Ligne jusqu'au point d'impact
            Gizmos.color = Color.green;
            Gizmos.DrawLine(rayOrigin, hit.point);
        }
    }
    #endif
}
