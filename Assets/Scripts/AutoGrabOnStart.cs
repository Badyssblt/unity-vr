using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// LA BONNE MÉTHODE : Force le grab automatiquement au démarrage
/// Utilise le système XR Interaction Toolkit correctement
/// </summary>
[RequireComponent(typeof(XRGrabInteractable))]
public class AutoGrabOnStart : MonoBehaviour
{
    [SerializeField] private bool grabOnStart = true;
    [SerializeField] private bool rightHand = true;
    [SerializeField] private float delayBeforeGrab = 0.5f;

    private XRGrabInteractable grabInteractable;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabOnStart)
        {
            Invoke(nameof(ForceGrab), delayBeforeGrab);
        }
    }

    void ForceGrab()
    {
        Debug.Log("🔍 Recherche des interactors...");

        // Trouver tous les interactors (mains)
        XRBaseInteractor[] interactors = FindObjectsOfType<XRBaseInteractor>();
        Debug.Log($"   Nombre d'interactors trouvés: {interactors.Length}");

        foreach (var interactor in interactors)
        {
            Debug.Log($"   - {interactor.name} (Type: {interactor.GetType().Name})");
        }

        XRBaseInteractor targetInteractor = null;

        // Chercher le bon interactor (main droite ou gauche)
        foreach (XRBaseInteractor interactor in interactors)
        {
            bool isRight = interactor.name.ToLower().Contains("right");
            bool isLeft = interactor.name.ToLower().Contains("left");

            if ((rightHand && isRight) || (!rightHand && isLeft))
            {
                targetInteractor = interactor;
                Debug.Log($"✅ Interactor cible trouvé: {interactor.name}");
                break;
            }
        }

        if (targetInteractor != null && grabInteractable != null)
        {
            Debug.Log($"📍 Position arme AVANT grab: {transform.position}");
            Debug.Log($"📍 Position interactor: {targetInteractor.transform.position}");

            // LA BONNE MÉTHODE : Utiliser le système XR Interaction
            var manager = grabInteractable.interactionManager;
            if (manager != null)
            {
                Debug.Log($"🎯 Tentative de grab...");
                manager.SelectEnter(targetInteractor, grabInteractable);

                // Vérifier après
                Invoke(nameof(CheckGrabSuccess), 0.2f);
            }
            else
            {
                Debug.LogError("❌ Interaction Manager est null!");
            }
        }
        else
        {
            if (targetInteractor == null)
                Debug.LogError($"❌ Interactor {(rightHand ? "droit" : "gauche")} non trouvé!");
            if (grabInteractable == null)
                Debug.LogError("❌ XRGrabInteractable non trouvé!");
        }
    }

    void CheckGrabSuccess()
    {
        Debug.Log($"📍 Position arme APRÈS grab: {transform.position}");
        Debug.Log($"🤝 Est grabbed: {grabInteractable.isSelected}");

        if (grabInteractable.isSelected)
        {
            Debug.Log($"✅ SUCCESS! Arme grabbée dans la {(rightHand ? "main droite" : "main gauche")}");
            Debug.Log($"👤 Parent actuel: {(transform.parent != null ? transform.parent.name : "null")}");
        }
        else
        {
            Debug.LogWarning("⚠️ Le grab a échoué - l'arme n'est pas selected");
        }
    }
}
