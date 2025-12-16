using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Bouton VR interactif qui peut être pressé avec les contrôleurs ou le rayon
/// </summary>
[RequireComponent(typeof(XRSimpleInteractable))]
public class VRButton : MonoBehaviour
{
    [Header("Visual Feedback")]
    [SerializeField] private MeshRenderer buttonRenderer;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private Color pressedColor = Color.green;

    [Header("Animation")]
    [SerializeField] private Transform buttonVisual;
    [SerializeField] private float pressDepth = 0.01f;
    [SerializeField] private float pressSpeed = 10f;

    [Header("Audio")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;

    [Header("Events")]
    public UnityEvent onButtonPressed;

    private XRSimpleInteractable interactable;
    private Vector3 originalPosition;
    private bool isPressed = false;
    private Material buttonMaterial;

    void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        if (buttonVisual != null)
            originalPosition = buttonVisual.localPosition;

        if (buttonRenderer != null)
        {
            buttonMaterial = buttonRenderer.material;
            buttonMaterial.color = normalColor;
        }
    }

    void OnEnable()
    {
        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
        interactable.selectEntered.AddListener(OnSelectEnter);
        interactable.selectExited.AddListener(OnSelectExit);
    }

    void OnDisable()
    {
        interactable.hoverEntered.RemoveListener(OnHoverEnter);
        interactable.hoverExited.RemoveListener(OnHoverExit);
        interactable.selectEntered.RemoveListener(OnSelectEnter);
        interactable.selectExited.RemoveListener(OnSelectExit);
    }

    void Update()
    {
        if (buttonVisual != null)
        {
            Vector3 targetPosition = isPressed
                ? originalPosition - Vector3.forward * pressDepth
                : originalPosition;

            buttonVisual.localPosition = Vector3.Lerp(
                buttonVisual.localPosition,
                targetPosition,
                Time.deltaTime * pressSpeed
            );
        }
    }

    void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (buttonMaterial != null)
            buttonMaterial.color = hoverColor;

        if (hoverSound != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(hoverSound);
    }

    void OnHoverExit(HoverExitEventArgs args)
    {
        if (buttonMaterial != null && !isPressed)
            buttonMaterial.color = normalColor;
    }

    void OnSelectEnter(SelectEnterEventArgs args)
    {
        isPressed = true;

        if (buttonMaterial != null)
            buttonMaterial.color = pressedColor;

        if (clickSound != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(clickSound);

        onButtonPressed?.Invoke();
    }

    void OnSelectExit(SelectExitEventArgs args)
    {
        isPressed = false;

        if (buttonMaterial != null)
            buttonMaterial.color = interactable.isHovered ? hoverColor : normalColor;
    }
}
