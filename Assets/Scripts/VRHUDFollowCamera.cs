using UnityEngine;

public class VRHUDFollowCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform vrCamera;

    [Header("Position Settings")]
    [SerializeField] private Vector3 offsetFromCamera = new Vector3(0.15f, -0.1f, 0.5f);
    [SerializeField] private bool followRotation = true;

    [Header("Smooth Settings")]
    [SerializeField] private bool useSmoothFollow = false;
    [SerializeField] private float smoothSpeed = 5f;

    void Start()
    {
        // Auto-trouver la caméra VR si non assignée
        if (vrCamera == null)
        {
            vrCamera = Camera.main.transform;
            Debug.Log($"✅ Caméra VR trouvée: {vrCamera.name}");
        }
    }

    void LateUpdate()
    {
        if (vrCamera == null) return;

        // Calculer la position cible
        Vector3 targetPosition = vrCamera.position + vrCamera.TransformDirection(offsetFromCamera);

        // Appliquer la position
        if (useSmoothFollow)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
        }
        else
        {
            transform.position = targetPosition;
        }

        // Faire face à la caméra (optionnel)
        if (followRotation)
        {
            transform.rotation = vrCamera.rotation;
        }
    }
}
