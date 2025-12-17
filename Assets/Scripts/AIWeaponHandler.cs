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

        // Appliquer l'imprécision (spread) pour l'IA
        ApplyAISpread();

        // Tirer
        weaponController.Shoot();

        if (showDebugLogs)
            Debug.Log($"🔫 {gameObject.name} tire! Munitions restantes: {weaponController.GetCurrentAmmo()}");
    }

    /// <summary>
    /// Applique un spread (imprécision) à l'arme de l'IA
    /// Plus l'accuracy est faible, plus le spread est grand
    /// </summary>
    void ApplyAISpread()
    {
        if (accuracy >= 1f) return; // Précision parfaite, pas de spread

        // Calculer l'angle de spread basé sur l'accuracy
        float spreadMultiplier = 1f - accuracy;
        float currentSpread = maxSpreadAngle * spreadMultiplier;

        // Appliquer une rotation aléatoire au firePoint (si accessible)
        // Note: Cela nécessiterait d'accéder au firePoint du WeaponController
        // Pour l'instant, on laisse le tir normal
        // Vous pouvez améliorer cela en ajoutant une méthode publique dans WeaponController
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
}
