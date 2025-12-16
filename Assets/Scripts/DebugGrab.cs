using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Script de debug pour tester le grab
/// Attachez ce script à votre arme pour voir les événements
/// </summary>
public class DebugGrab : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable == null)
        {
            Debug.LogError("❌ PAS DE XRGrabInteractable sur " + gameObject.name);
            return;
        }

        Debug.Log("✅ XRGrabInteractable trouvé sur " + gameObject.name);

        // Subscribe aux événements
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
        grabInteractable.hoverEntered.AddListener(OnHoverEnter);
        grabInteractable.hoverExited.AddListener(OnHoverExit);
    }

    void OnHoverEnter(HoverEnterEventArgs args)
    {
        Debug.Log("🟢 HOVER ENTER - Le contrôleur est proche de l'arme");
    }

    void OnHoverExit(HoverExitEventArgs args)
    {
        Debug.Log("🔴 HOVER EXIT - Le contrôleur s'éloigne");
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log("✅ GRABBED! - L'arme est saisie par: " + args.interactorObject);
    }

    void OnRelease(SelectExitEventArgs args)
    {
        Debug.Log("⚠️ RELEASED - L'arme est lâchée");
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
            grabInteractable.hoverEntered.RemoveListener(OnHoverEnter);
            grabInteractable.hoverExited.RemoveListener(OnHoverExit);
        }
    }
}
