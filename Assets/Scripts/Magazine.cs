using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Chargeur interactif pour les armes VR
/// </summary>
[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class Magazine : MonoBehaviour
{
    [Header("Magazine Settings")]
    [SerializeField] private int ammoCount = 30; // Nombre de munitions dans le chargeur
    [SerializeField] private bool isInserted = false; // Est inséré dans une arme

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private Transform originalParent;
    private Collider[] colliders;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        colliders = GetComponents<Collider>();
        originalParent = transform.parent;
    }

    void Start()
    {
        // S'assurer que le chargeur a la physique activée
        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }
    }

    /// <summary>
    /// Vérifie si le chargeur peut être inséré dans une arme
    /// </summary>
    public bool CanBeInserted()
    {
        return !isInserted;
    }

    /// <summary>
    /// Vérifie si le chargeur est actuellement tenu par une main
    /// </summary>
    public bool IsGrabbed()
    {
        return grabInteractable != null && grabInteractable.isSelected;
    }

    /// <summary>
    /// Appelé quand le chargeur est inséré dans une arme
    /// </summary>
    public void OnInserted(Transform attachPoint)
    {
        isInserted = true;

        // Attacher au point d'attache de l'arme
        transform.SetParent(attachPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // Désactiver la physique
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Garder grabInteractable et colliders actifs pour pouvoir retirer le chargeur
        // MagazineSocket met déjà les colliders en trigger pour éviter les collisions

        Debug.Log($"📥 Chargeur inséré avec {ammoCount} munitions");
    }

    /// <summary>
    /// Appelé quand le chargeur est éjecté de l'arme
    /// </summary>
    public void OnEjected()
    {
        isInserted = false;

        // Réactiver l'interaction
        if (grabInteractable != null)
        {
            grabInteractable.enabled = true;
        }

        // Détacher de l'arme
        transform.SetParent(null);

        // Réactiver la physique
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // Réactiver les colliders pour pouvoir grab le mag
        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

        Debug.Log("📤 Chargeur éjecté");
    }

    /// <summary>
    /// Retourne le nombre de munitions dans le chargeur
    /// </summary>
    public int GetAmmoCount()
    {
        return ammoCount;
    }

    /// <summary>
    /// Définit le nombre de munitions dans le chargeur
    /// </summary>
    public void SetAmmoCount(int count)
    {
        ammoCount = Mathf.Max(0, count);
    }

    /// <summary>
    /// Vérifie si le chargeur est vide
    /// </summary>
    public bool IsEmpty()
    {
        return ammoCount <= 0;
    }

    /// <summary>
    /// Recharge le chargeur à pleine capacité
    /// </summary>
    public void Refill(int maxCapacity = 30)
    {
        ammoCount = maxCapacity;
        Debug.Log($"♻️ Chargeur rechargé: {ammoCount} munitions");
    }

    // Afficher les infos dans l'inspector
    void OnValidate()
    {
        ammoCount = Mathf.Max(0, ammoCount);
    }
}
