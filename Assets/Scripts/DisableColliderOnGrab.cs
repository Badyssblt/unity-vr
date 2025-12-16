using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Désactive le collider de l'arme quand elle est grabbée
/// Cela évite les collisions indésirables avec le joueur
/// </summary>
[RequireComponent(typeof(XRGrabInteractable))]
public class DisableColliderOnGrab : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool disableColliderOnGrab = true;
    [SerializeField] private bool makeKinematicOnGrab = true;

    private XRGrabInteractable grabInteractable;
    private Collider[] colliders;
    private Rigidbody rb;
    private bool wasKinematic;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        colliders = GetComponents<Collider>();
        rb = GetComponent<Rigidbody>();

        if (rb != null)
            wasKinematic = rb.isKinematic;

        // Subscribe aux événements
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        if (disableColliderOnGrab)
        {
            // Désactiver tous les colliders
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
            Debug.Log("🔒 Colliders désactivés sur grab");
        }

        if (makeKinematicOnGrab && rb != null)
        {
            wasKinematic = rb.isKinematic;
            rb.isKinematic = true;
        }
    }

    void OnReleased(SelectExitEventArgs args)
    {
        if (disableColliderOnGrab)
        {
            // Réactiver tous les colliders
            foreach (Collider col in colliders)
            {
                col.enabled = true;
            }
            Debug.Log("🔓 Colliders réactivés après release");
        }

        if (makeKinematicOnGrab && rb != null)
        {
            rb.isKinematic = wasKinematic;
        }
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }
}
