using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Attache automatiquement l'arme à la main au démarrage
/// L'arme est directement dans votre main, pas besoin de la grab
/// </summary>
public class WeaponAttacher : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool attachOnStart = true;
    [SerializeField] private bool rightHand = true; // true = main droite, false = main gauche

    private XRGrabInteractable grabInteractable;
    private XRDirectInteractor targetInteractor;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (attachOnStart)
        {
            // Attendre un frame pour que tout soit initialisé
            Invoke(nameof(AttachToHand), 0.1f);
        }
    }

    void AttachToHand()
    {
        // Trouver le bon interactor (main droite ou gauche)
        XRDirectInteractor[] interactors = FindObjectsOfType<XRDirectInteractor>();

        foreach (XRDirectInteractor interactor in interactors)
        {
            // Vérifier si c'est la bonne main
            bool isRightHand = interactor.name.Contains("Right");

            if ((rightHand && isRightHand) || (!rightHand && !isRightHand))
            {
                targetInteractor = interactor;
                break;
            }
        }

        if (targetInteractor != null && grabInteractable != null)
        {
            // Forcer l'attachement
            var interactionManager = grabInteractable.interactionManager;
            if (interactionManager != null)
            {
                interactionManager.SelectEnter(targetInteractor, grabInteractable);
                Debug.Log($"✅ Arme attachée à la {(rightHand ? "main droite" : "main gauche")}");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Impossible d'attacher l'arme - Interactor non trouvé");
        }
    }

    // Appeler cette méthode pour attacher manuellement
    public void AttachToRightHand()
    {
        rightHand = true;
        AttachToHand();
    }

    public void AttachToLeftHand()
    {
        rightHand = false;
        AttachToHand();
    }
}
