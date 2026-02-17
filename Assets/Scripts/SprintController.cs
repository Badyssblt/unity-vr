using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Permet au joueur de sprinter en appuyant sur une touche ou un bouton VR
/// </summary>
public class SprintController : MonoBehaviour
{
    [Header("Sprint Settings")]
    [SerializeField] private float normalSpeed = 2f;
    [SerializeField] private float sprintSpeed = 5f;

    [Header("Input")]
    [Tooltip("Touche clavier pour sprinter (pour test sans VR)")]
    [SerializeField] private Key keyboardSprintKey = Key.LeftShift;

    [Header("References")]
    [SerializeField] private ActionBasedContinuousMoveProvider moveProvider;

    private bool isSprinting;
    private InputAction rightControllerPrimaryButton;

    void Start()
    {
        // Auto-trouver le MoveProvider si non assigné
        if (moveProvider == null)
        {
            moveProvider = FindObjectOfType<ActionBasedContinuousMoveProvider>();
        }

        if (moveProvider == null)
        {
            Debug.LogError("SprintController: Aucun ActionBasedContinuousMoveProvider trouvé!");
            enabled = false;
            return;
        }

        // Sauvegarder la vitesse normale
        normalSpeed = moveProvider.moveSpeed;

        // Créer l'action pour le bouton A du contrôleur droit (Meta Quest 2)
        rightControllerPrimaryButton = new InputAction("RightPrimaryButton", InputActionType.Button);
        rightControllerPrimaryButton.AddBinding("<XRController>{RightHand}/primaryButton");
        rightControllerPrimaryButton.Enable();
    }

    void Update()
    {
        if (moveProvider == null) return;

        // Vérifier l'input VR (bouton A du contrôleur droit)
        bool vrSprint = rightControllerPrimaryButton != null && rightControllerPrimaryButton.IsPressed();

        // Vérifier l'input clavier
        bool keyboardSprint = Keyboard.current != null &&
                              Keyboard.current[keyboardSprintKey].isPressed;

        // Sprint si l'un des deux inputs est actif
        isSprinting = vrSprint || keyboardSprint;

        // Appliquer la vitesse
        moveProvider.moveSpeed = isSprinting ? sprintSpeed : normalSpeed;
    }

    void OnDestroy()
    {
        // Libérer l'action
        if (rightControllerPrimaryButton != null)
        {
            rightControllerPrimaryButton.Disable();
            rightControllerPrimaryButton.Dispose();
        }
    }

    void OnDisable()
    {
        // Remettre la vitesse normale quand le script est désactivé
        if (moveProvider != null)
        {
            moveProvider.moveSpeed = normalSpeed;
        }
    }
}
