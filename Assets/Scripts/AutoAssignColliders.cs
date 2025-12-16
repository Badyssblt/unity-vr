using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Assigne automatiquement tous les colliders enfants au XRGrabInteractable
/// Résout le problème des colliders non assignés dans le prefab
/// </summary>
[RequireComponent(typeof(XRGrabInteractable))]
public class AutoAssignColliders : MonoBehaviour
{
    void Awake()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();

        // Récupérer tous les colliders de cet objet et ses enfants
        Collider[] colliders = GetComponentsInChildren<Collider>();

        if (colliders.Length == 0)
        {
            Debug.LogWarning($"⚠️ Aucun collider trouvé sur {gameObject.name}");
            return;
        }

        // Assigner les colliders au XRGrabInteractable
        grabInteractable.colliders.Clear();
        foreach (Collider col in colliders)
        {
            grabInteractable.colliders.Add(col);
            Debug.Log($"✅ Collider ajouté : {col.gameObject.name}");
        }

        Debug.Log($"✅ {colliders.Length} collider(s) assigné(s) à {gameObject.name}");

        // Vérifier l'AttachTransform
        if (grabInteractable.attachTransform != null)
        {
            Debug.Log($"✅ AttachTransform configuré : {grabInteractable.attachTransform.name}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Pas d'AttachTransform sur {gameObject.name} - L'objet s'attachera au centre");
        }
    }
}
