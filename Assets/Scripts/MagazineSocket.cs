using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

/// <summary>
/// Gère le socket pour insérer/éjecter un chargeur dans une arme VR
/// </summary>
public class MagazineSocket : MonoBehaviour
{
    [Header("Socket Settings")]
    [SerializeField] private XRSocketInteractor socketInteractor;
    [SerializeField] private bool requireMagazineTag = true; // Vérifier le tag "Magazine"
    [SerializeField] private bool ejectOnGrab = false; // Éjecter automatiquement quand l'arme est grabbée

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip insertSound;
    [SerializeField] private AudioClip ejectSound;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject socketVisual; // Optionnel : objet à afficher/masquer
    [SerializeField] private bool hideVisualWhenFilled = true;

    [Header("Events")]
    public UnityEvent<GameObject> onMagazineInserted; // Événement quand un chargeur est inséré
    public UnityEvent onMagazineEjected; // Événement quand un chargeur est éjecté

    // État actuel
    private GameObject currentMagazine;
    private bool hasMagazine = false;

    void Start()
    {
        // Auto-trouver le XRSocketInteractor si non assigné
        if (socketInteractor == null)
            socketInteractor = GetComponent<XRSocketInteractor>();

        if (socketInteractor == null)
        {
            Debug.LogError("❌ MagazineSocket: XRSocketInteractor manquant!");
            enabled = false;
            return;
        }

        // Auto-trouver l'AudioSource si non assigné
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // S'abonner aux événements du socket
        socketInteractor.selectEntered.AddListener(OnMagazineInserted);
        socketInteractor.selectExited.AddListener(OnMagazineEjected);

        // Vérifier si un chargeur est déjà présent au démarrage
        if (socketInteractor.hasSelection)
        {
            currentMagazine = socketInteractor.GetOldestInteractableSelected().transform.gameObject;
            hasMagazine = true;
            UpdateVisual();
        }

    }

    void OnMagazineInserted(SelectEnterEventArgs args)
    {
        GameObject insertedObject = args.interactableObject.transform.gameObject;

        // Vérifier si c'est bien un chargeur (optionnel)
        if (requireMagazineTag && !insertedObject.CompareTag("Magazine"))
        {
            Debug.LogWarning($"⚠️ L'objet '{insertedObject.name}' n'a pas le tag 'Magazine'!");

            // Optionnel : éjecter automatiquement si ce n'est pas un chargeur
            // EjectMagazine();
            // return;
        }

        currentMagazine = insertedObject;
        hasMagazine = true;

        // Passer les colliders du mag en trigger pour ne pas bloquer le joueur
        // Le socket garde sa sélection avec des triggers, mais le CharacterController les traverse
        foreach (Collider col in currentMagazine.GetComponentsInChildren<Collider>())
        {
            col.isTrigger = true;
        }

        // Jouer le son d'insertion
        PlaySound(insertSound);

        // Mettre à jour le visuel
        UpdateVisual();

        // Déclencher l'événement
        onMagazineInserted?.Invoke(currentMagazine);

        
    }

    void OnMagazineEjected(SelectExitEventArgs args)
    {
        Debug.Log($"❌ Chargeur éjecté: {currentMagazine?.name ?? "Inconnu"}");

        // Remettre les colliders du mag en non-trigger pour le grab et la physique
        if (currentMagazine != null)
        {
            foreach (Collider col in currentMagazine.GetComponentsInChildren<Collider>())
            {
                col.isTrigger = false;
            }
        }

        GameObject ejectedMagazine = currentMagazine;
        currentMagazine = null;
        hasMagazine = false;

        // Détacher le magazine après un frame (le XR Toolkit finit son traitement avant)
        if (ejectedMagazine != null)
            StartCoroutine(DetachMagazineNextFrame(ejectedMagazine));

        // Jouer le son d'éjection
        PlaySound(ejectSound);

        // Mettre à jour le visuel
        UpdateVisual();

        // Déclencher l'événement
        onMagazineEjected?.Invoke();
    }

    /// <summary>
    /// Désactive le socket, détache le magazine et réactive le socket après un délai.
    /// Sans ça, le socket re-grab immédiatement le magazine éjecté.
    /// </summary>
    System.Collections.IEnumerator DetachMagazineNextFrame(GameObject magazine)
    {
        // Désactiver le socket AVANT le yield pour bloquer le re-grab immédiat
        if (socketInteractor != null)
            socketInteractor.enabled = false;

        yield return null; // attendre que le XR Toolkit finisse son SelectExit

        if (magazine != null)
        {
            magazine.transform.SetParent(null);

            Rigidbody rb = magazine.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(Vector3.down * 2f, ForceMode.Impulse);
            }
        }

        // Réactiver le socket après 1s pour pouvoir insérer un nouveau chargeur
        yield return new WaitForSeconds(1f);
        if (socketInteractor != null)
            socketInteractor.enabled = true;
    }

    /// <summary>
    /// Éjecter programmatiquement le chargeur du socket
    /// </summary>
    public void EjectMagazine()
    {
        if (!socketInteractor.hasSelection)
        {
            Debug.LogWarning("⚠️ Aucun chargeur à éjecter");
            return;
        }

        var interactable = socketInteractor.GetOldestInteractableSelected();
        socketInteractor.interactionManager.SelectExit(socketInteractor, interactable);

        Debug.Log("🔓 Chargeur éjecté programmatiquement");
    }

    /// <summary>
    /// Vérifier si le socket contient un chargeur
    /// </summary>
    public bool HasMagazine()
    {
        return hasMagazine && currentMagazine != null;
    }

    /// <summary>
    /// Obtenir le chargeur actuellement dans le socket
    /// </summary>
    public GameObject GetMagazine()
    {
        return currentMagazine;
    }

    /// <summary>
    /// Obtenir le XRSocketInteractor
    /// </summary>
    public XRSocketInteractor GetSocketInteractor()
    {
        return socketInteractor;
    }

    /// <summary>
    /// Activer/désactiver le socket
    /// </summary>
    public void SetSocketActive(bool active)
    {
        if (socketInteractor != null)
        {
            socketInteractor.enabled = active;
            Debug.Log($"Socket {(active ? "activé" : "désactivé")}");
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void UpdateVisual()
    {
        if (socketVisual != null && hideVisualWhenFilled)
        {
            // Masquer le visuel quand un chargeur est inséré
            socketVisual.SetActive(!hasMagazine);
        }
    }

    void OnDestroy()
    {
        // Se désabonner pour éviter les memory leaks
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.RemoveListener(OnMagazineInserted);
            socketInteractor.selectExited.RemoveListener(OnMagazineEjected);
        }
    }

    // Visualisation du socket dans la Scene view
    #if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (socketInteractor == null)
            socketInteractor = GetComponent<XRSocketInteractor>();

        if (socketInteractor != null)
        {
            // Dessiner une sphère au centre du socket
            Gizmos.color = hasMagazine ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.05f);

            // Dessiner le rayon d'interaction
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            // Note: XRSocketInteractor n'a pas de socketSnappingRadius public dans cette version
            // mais on peut dessiner une zone approximative
            Gizmos.DrawWireSphere(transform.position, 0.1f);
        }
    }
    #endif
}
