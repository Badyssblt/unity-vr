using UnityEngine;

/// <summary>
/// Gère l'utilisation du WeaponController par l'IA
/// Permet à l'IA de tirer sans avoir besoin des contrôles VR
/// </summary>
public class AIWeaponHandler : MonoBehaviour
{
    [Header("Weapon Reference")]
    [SerializeField] private WeaponController weaponController;

    [Header("AI Shooting Settings")]
    [SerializeField] private float accuracy = 0.8f; // 0 = très imprécis, 1 = parfait
    [SerializeField] private float maxSpreadAngle = 10f; // Angle max de déviation en degrés
    [SerializeField] private float aimHeightOffset = 1.5f; // Hauteur de visée (1.5m = poitrine, 1.7m = tête)
    [SerializeField] private bool autoReload = true;
    [SerializeField] private bool unlimitedAmmo = false;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private AIController aiController;

    void Awake()
    {
        aiController = GetComponent<AIController>();

        // Auto-trouver le WeaponController si non assigné
        if (weaponController == null)
        {
            weaponController = GetComponentInChildren<WeaponController>();
        }

        if (weaponController == null)
        {
            Debug.LogError($"❌ {gameObject.name} : Aucun WeaponController trouvé! Ajoutez une arme à l'IA.");
        }
    }

    void Update()
    {
        // Ne pas recharger tant que le jeu n'a pas commencé
        if (GameManager.Instance == null || !GameManager.Instance.IsGameActive())
            return;

        // Auto-reload si nécessaire
        if (autoReload && weaponController != null)
        {
            if (weaponController.GetCurrentAmmo() <= 0 && !weaponController.IsReloading())
            {
                if (showDebugLogs)
                    Debug.Log($"🔄 {gameObject.name} recharge son arme");

                weaponController.StartReload();
            }
        }

        // Cheat: Munitions infinies
        if (unlimitedAmmo && weaponController != null)
        {
            // Vous pouvez ajouter une méthode publique dans WeaponController pour remplir les munitions
            // Pour l'instant, on laisse l'auto-reload gérer
        }
    }

    /// <summary>
    /// Déclencher un tir depuis l'IA
    /// </summary>
    public void Shoot()
    {
        // Ne pas tirer tant que le jeu n'a pas commencé
        if (GameManager.Instance == null || !GameManager.Instance.IsGameActive())
            return;

        if (weaponController == null)
        {
            Debug.LogWarning($"⚠️ {gameObject.name} : Pas d'arme pour tirer!");
            return;
        }

        // Vérifier si on peut tirer
        if (weaponController.GetCurrentAmmo() <= 0)
        {
            if (showDebugLogs)
                Debug.Log($"⚠️ {gameObject.name} : Plus de munitions!");
            return;
        }

        if (weaponController.IsReloading())
        {
            if (showDebugLogs)
                Debug.Log($"⚠️ {gameObject.name} : En train de recharger!");
            return;
        }

        // Faire pivoter l'arme vers le joueur avant de tirer
        AimWeaponAtPlayer();

        // Appliquer l'imprécision (spread) pour l'IA
        ApplyAISpread();

        // Tirer
        weaponController.Shoot();

        if (showDebugLogs)
            Debug.Log($"🔫 {gameObject.name} tire! Munitions restantes: {weaponController.GetCurrentAmmo()}");
    }

    /// <summary>
    /// Fait pivoter le FirePoint vers le joueur pour une visée précise
    /// </summary>
    void AimWeaponAtPlayer()
    {
        // Obtenir la cible depuis l'AIController
        if (aiController == null || aiController.Player == null)
        {
            if (showDebugLogs)
                Debug.LogWarning($"⚠️ {gameObject.name} : Pas de cible pour viser!");
            return;
        }

        Transform player = aiController.Player;
        Transform firePoint = weaponController.GetFirePoint();

        if (firePoint == null)
        {
            if (showDebugLogs)
                Debug.LogWarning($"⚠️ {gameObject.name} : Pas de FirePoint trouvé!");
            return;
        }

        // Calculer la position de visée (ajout d'un offset vertical pour viser la poitrine/tête)
        Vector3 targetPosition = player.position + Vector3.up * aimHeightOffset;

        // Calculer la direction vers la cible depuis le FirePoint
        Vector3 directionToPlayer = (targetPosition - firePoint.position).normalized;

        // Faire pivoter le FirePoint vers le joueur en espace monde
        firePoint.rotation = Quaternion.LookRotation(directionToPlayer);

        if (showDebugLogs)
            Debug.Log($"🎯 {gameObject.name} vise le joueur à {targetPosition} (offset: {aimHeightOffset}m) depuis {firePoint.position}");
    }

    /// <summary>
    /// Applique un spread (imprécision) au FirePoint de l'IA
    /// Plus l'accuracy est faible, plus le spread est grand
    /// </summary>
    void ApplyAISpread()
    {
        if (accuracy >= 1f) return; // Précision parfaite, pas de spread

        Transform firePoint = weaponController.GetFirePoint();
        if (firePoint == null) return;

        // Calculer l'angle de spread basé sur l'accuracy
        float spreadMultiplier = 1f - accuracy;
        float currentSpread = maxSpreadAngle * spreadMultiplier;

        // Appliquer une rotation aléatoire au FirePoint pour simuler l'imprécision
        float randomX = Random.Range(-currentSpread, currentSpread);
        float randomY = Random.Range(-currentSpread, currentSpread);

        firePoint.Rotate(randomX, randomY, 0f);

        if (showDebugLogs)
            Debug.Log($"🎲 {gameObject.name} applique un spread de {currentSpread:F2}° (accuracy: {accuracy:F2})");
    }

    /// <summary>
    /// Vérifier si l'arme peut tirer
    /// </summary>
    public bool CanShoot()
    {
        if (weaponController == null) return false;
        return weaponController.GetCurrentAmmo() > 0 && !weaponController.IsReloading();
    }

    /// <summary>
    /// Obtenir les munitions actuelles
    /// </summary>
    public int GetCurrentAmmo()
    {
        if (weaponController == null) return 0;
        return weaponController.GetCurrentAmmo();
    }

    /// <summary>
    /// Recharger l'arme manuellement
    /// </summary>
    public void Reload()
    {
        if (weaponController == null) return;
        weaponController.StartReload();
    }

    /// <summary>
    /// Vérifier si l'arme est en train de recharger
    /// </summary>
    public bool IsWeaponReloading()
    {
        if (weaponController == null) return false;
        return weaponController.IsReloading();
    }
}
