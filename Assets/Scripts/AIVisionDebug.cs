using UnityEngine;

/// <summary>
/// Visualise la vision de l'IA avec des lignes et des gizmos
/// Utile pour débugger et voir ce que l'IA voit
/// </summary>
public class AIVisionDebug : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AIController aiController;

    [Header("Visualization Settings")]
    [SerializeField] private bool drawVisionCone = true;
    [SerializeField] private bool drawLineToPlayer = true;
    [SerializeField] private Color canSeeColor = Color.red;
    [SerializeField] private Color cannotSeeColor = Color.yellow;
    [SerializeField] private int visionConeSegments = 20;

    void Awake()
    {
        if (aiController == null)
        {
            aiController = GetComponent<AIController>();
        }
    }

    void OnDrawGizmos()
    {
        if (aiController == null || aiController.Player == null) return;

        Vector3 position = transform.position + Vector3.up * 1.5f; // Position des yeux
        Vector3 playerPosition = aiController.Player.position;

        // Couleur en fonction de si l'IA voit le joueur
        Color visionColor = aiController.CanSeePlayer ? canSeeColor : cannotSeeColor;

        // Dessiner une ligne vers le joueur
        if (drawLineToPlayer)
        {
            Gizmos.color = visionColor;
            Gizmos.DrawLine(position, playerPosition);

            // Dessiner une sphère au bout de la ligne
            Gizmos.DrawSphere(playerPosition, 0.2f);
        }

        // Dessiner le cône de vision
        if (drawVisionCone)
        {
            DrawVisionCone(position, visionColor);
        }
    }

    void DrawVisionCone(Vector3 position, Color color)
    {
        // Récupérer les paramètres de l'AIController via reflection (pas idéal mais pratique pour debug)
        // Pour une meilleure solution, ajoutez des getters publics dans AIController

        float detectionRange = 15f; // Par défaut
        float fieldOfView = 90f;

        // Tenter de lire les valeurs via reflection
        var detectionField = typeof(AIController).GetField("detectionRange",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var fovField = typeof(AIController).GetField("fieldOfView",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (detectionField != null)
            detectionRange = (float)detectionField.GetValue(aiController);
        if (fovField != null)
            fieldOfView = (float)fovField.GetValue(aiController);

        Gizmos.color = new Color(color.r, color.g, color.b, 0.2f);

        // Dessiner les bords du cône
        float halfFOV = fieldOfView / 2f;
        Vector3 leftBoundary = Quaternion.Euler(0, -halfFOV, 0) * transform.forward * detectionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, halfFOV, 0) * transform.forward * detectionRange;

        Gizmos.DrawLine(position, position + leftBoundary);
        Gizmos.DrawLine(position, position + rightBoundary);

        // Dessiner l'arc du cône
        Vector3 previousPoint = position + leftBoundary;
        for (int i = 1; i <= visionConeSegments; i++)
        {
            float angle = -halfFOV + (fieldOfView * i / visionConeSegments);
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward * detectionRange;
            Vector3 point = position + direction;

            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }
    }
}
