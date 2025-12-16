using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Provides haptic feedback (controller vibrations) for VR interactions
/// Attach to objects that should provide haptic feedback
/// </summary>
public class HapticFeedback : MonoBehaviour
{
    [Header("Haptic Settings")]
    [SerializeField] private float intensity = 0.5f;
    [SerializeField] private float duration = 0.1f;

    [Header("Events")]
    [SerializeField] private bool onGrab = true;
    [SerializeField] private bool onRelease = true;
    [SerializeField] private bool onActivate = true;

    private XRBaseInteractable interactable;
    private XRBaseControllerInteractor currentInteractor;

    void Start()
    {
        interactable = GetComponent<XRBaseInteractable>();

        if (interactable != null)
        {
            if (onGrab)
                interactable.selectEntered.AddListener(OnGrabbed);

            if (onRelease)
                interactable.selectExited.AddListener(OnReleased);

            if (onActivate)
                interactable.activated.AddListener(OnActivated);
        }
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject as XRBaseControllerInteractor;
        TriggerHaptic(intensity, duration);
    }

    void OnReleased(SelectExitEventArgs args)
    {
        TriggerHaptic(intensity * 0.5f, duration * 0.5f);
        currentInteractor = null;
    }

    void OnActivated(ActivateEventArgs args)
    {
        currentInteractor = args.interactorObject as XRBaseControllerInteractor;
        TriggerHaptic(intensity, duration);
    }

    public void TriggerHaptic()
    {
        TriggerHaptic(intensity, duration);
    }

    public void TriggerHaptic(float customIntensity, float customDuration)
    {
        if (currentInteractor != null && currentInteractor.xrController != null)
        {
            currentInteractor.xrController.SendHapticImpulse(customIntensity, customDuration);
        }
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnGrabbed);
            interactable.selectExited.RemoveListener(OnReleased);
            interactable.activated.RemoveListener(OnActivated);
        }
    }
}
