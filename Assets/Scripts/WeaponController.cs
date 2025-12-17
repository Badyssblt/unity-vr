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
    [SerializeField] private float damage = 25f; // Dégâts de base de l'arme
    [SerializeField] private string ownerTeam = "Player"; // "Player" ou "Enemy"

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
    private bool hasMagazine = false; // Track si un magazine est inséré (true par défaut)
    private float nextEmptySoundTime = 0f; // Temps avant de pouvoir rejouer le son empty
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

        // Marquer que le magazine est présent
        hasMagazine = true;

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

        // Marquer que le magazine n'est plus présent
        hasMagazine = false;

        // Vider l'arme quand le chargeur est retiré
        currentAmmo = 0;
        UpdateAmmoUI();

        // Feedback haptique
        TriggerHaptic(0.1f, 0.05f);
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

        // Ne pas afficher le laser en continu (seulement lors du tir)
        // UpdateLaserSight();
    }

    bool CanShoot()
    {
        // Vérifier si un magazine est inséré (si VR reload est activé)
        if (useVRReload && !hasMagazine)
        {
            Debug.LogWarning("⚠️ Aucun chargeur dans l'arme!");
            return false;
        }

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
            // Mode normal : utiliser shootAction directement
            if (shootAction.action != null)
            {
                float triggerValue = shootAction.action.ReadValue<float>();
                return triggerValue > 0.1f; // Trigger pressé à plus de 10%
            }

            // Fallback si shootAction n'est pas configuré
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
            // Jouer le son "empty" seulement si le cooldown est écoulé
            if (Time.time >= nextEmptySoundTime)
            {
                PlaySound(emptySound);
                TriggerHaptic(0.1f, 0.05f);

                // Empêcher le son de rejouer pendant sa durée (ou 0.5s par défaut)
                float soundDuration = emptySound != null ? emptySound.length : 0.5f;
                nextEmptySoundTime = Time.time + soundDuration;
            }
            return;
        }

        currentAmmo--;
        nextFireTime = Time.time + fireRate;
        UpdateLaserSight();
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

        // DEBUG: Dessiner une grosse sphère au point de départ
        Debug.DrawRay(rayOrigin, Vector3.up * 0.5f, Color.cyan, 2f);
        Debug.DrawRay(rayOrigin, rayDirection * 2f, Color.magenta, 2f);

        Ray ray = new Ray(rayOrigin, rayDirection);

        // TEST: Raycast sans filtre pour voir si on touche QUELQUE CHOSE
        RaycastHit testHit;
        bool testHasHit = Physics.Raycast(ray, out testHit, raycastMaxDistance);
        if (testHasHit)
        {
            Debug.Log($"🧪 TEST: On touche {testHit.collider.name} à {testHit.distance:F2}m (sans filtre layer)");
        }
        else
        {
            Debug.Log($"🧪 TEST: On ne touche RIEN même sans filtre layer! Direction: {rayDirection}");
        }

        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit, raycastMaxDistance, targetLayer);

        // DEBUG: Afficher info sur le raycast
        Debug.Log($"🔫 {gameObject.name} tire! Origin: {rayOrigin}, Direction: {rayDirection}, Distance max: {raycastMaxDistance}, LayerMask: {targetLayer.value}");

        // UN SEUL DrawRay au moment du tir (0.5 secondes)
        if (hasHit)
        {
            // Ligne verte jusqu'au point d'impact
            Debug.DrawLine(rayOrigin, hit.point, Color.green, 2f);
            Debug.Log($"🎯 TOUCHE! {hit.collider.name} (Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}) à {hit.distance:F2}m");

            // NOUVEAU SYSTÈME DE DÉGÂTS avec Hitbox
            // Priorité 1 : Chercher DamageableHitbox (pour headshots)
            DamageableHitbox hitbox = hit.collider.GetComponent<DamageableHitbox>();
            if (hitbox != null)
            {
                hitbox.TakeDamage(damage, ownerTeam);
            }
            else
            {
                // Priorité 2 : Chercher HealthSystem directement
                HealthSystem healthSystem = hit.collider.GetComponent<HealthSystem>();
                if (healthSystem != null)
                {
                    healthSystem.TakeDamage(damage, ownerTeam);
                }
                else
                {
                    // Priorité 3 : Ancien système Target (pour compatibilité)
                    Target target = hit.collider.GetComponent<Target>();
                    if (target != null)
                    {
                        target.TakeDamage();
                    }
                }
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
            // DEBUG: Aucun impact
            Debug.DrawLine(rayOrigin, rayOrigin + rayDirection * raycastMaxDistance, Color.red, 2f);
            Debug.Log($"❌ RATÉ! Aucun objet touché (LayerMask: {targetLayer.value})");

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

    // Affiche un effet visuel du tir (active le laser brièvement)
    System.Collections.IEnumerator ShowShootEffect(Vector3 endPoint)
    {
        if (laserSight != null)
        {
            // Activer le laser et afficher la trajectoire du tir
            laserSight.enabled = true;
            laserSight.SetPosition(0, firePoint.position);
            laserSight.SetPosition(1, endPoint);

            yield return new WaitForSeconds(0.1f);

            // Désactiver le laser après le tir
            laserSight.enabled = false;
        }
    }


    void ShootProjectile()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // Obtenir la direction de tir configurée
            Vector3 shootDir = GetShootDirection();

            // Créer le projectile
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(shootDir));

            // Initialiser le projectile avec les dégâts et l'équipe
            BulletProjectile projectile = bullet.GetComponent<BulletProjectile>();
            if (projectile != null)
            {
                projectile.Initialize(damage, ownerTeam);
            }

            // Appliquer la vélocité
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = shootDir * bulletSpeed;
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
        if (audioSource == null)
        {
            Debug.LogWarning("⚠️ AudioSource est NULL! Ajoutez un AudioSource au WeaponController.");
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("⚠️ AudioClip est NULL! Assignez un son dans l'Inspector.");
            return;
        }

        audioSource.PlayOneShot(clip);
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
