using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Script de diagnostic et correction automatique pour les boutons VR
/// </summary>
public class VRButtonDiagnostic : MonoBehaviour
{
    [Header("Diagnostic")]
    [SerializeField] private bool runDiagnosticOnStart = true;
    [SerializeField] private bool autoFix = true;

    [Header("Configuration")]
    [SerializeField] private bool ensureCollider = true;
    [SerializeField] private bool ensureInteractionLayers = true;
    [SerializeField] private Vector3 colliderSize = new Vector3(0.1f, 0.05f, 0.1f);

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    void Start()
    {
        if (runDiagnosticOnStart)
        {
            RunDiagnostic();
        }
    }

    [ContextMenu("Run Diagnostic")]
    public void RunDiagnostic()
    {
        if (showDebugLogs)
            Debug.Log($"=== Diagnostic VR Button: {gameObject.name} ===");

        bool hasIssues = false;

        // 1. Vérifier XRSimpleInteractable
        XRSimpleInteractable interactable = GetComponent<XRSimpleInteractable>();
        if (interactable == null)
        {
            LogError("❌ Pas de XRSimpleInteractable trouvé!");
            hasIssues = true;

            if (autoFix)
            {
                interactable = gameObject.AddComponent<XRSimpleInteractable>();
                LogSuccess("✅ XRSimpleInteractable ajouté automatiquement");
            }
        }
        else
        {
            LogSuccess("✅ XRSimpleInteractable présent");
        }

        // 2. Vérifier Collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            LogError("❌ Pas de Collider trouvé!");
            hasIssues = true;

            if (autoFix && ensureCollider)
            {
                BoxCollider boxCol = gameObject.AddComponent<BoxCollider>();
                boxCol.size = colliderSize;
                boxCol.isTrigger = false;
                LogSuccess($"✅ BoxCollider ajouté automatiquement (size: {colliderSize})");
            }
        }
        else
        {
            LogSuccess($"✅ Collider présent: {col.GetType().Name}");

            // Vérifier si le collider est en trigger
            if (col.isTrigger)
            {
                LogWarning("⚠️ Le Collider est en mode Trigger! (devrait être False pour RayInteractor)");
                hasIssues = true;

                if (autoFix)
                {
                    col.isTrigger = false;
                    LogSuccess("✅ Collider.isTrigger corrigé à False");
                }
            }
            else
            {
                LogSuccess("✅ Collider.isTrigger = False (correct)");
            }
        }

        // 3. Vérifier VRButton
        VRButton vrButton = GetComponent<VRButton>();
        if (vrButton == null)
        {
            LogError("❌ Pas de VRButton trouvé!");
            hasIssues = true;
        }
        else
        {
            LogSuccess("✅ VRButton présent");
        }

        // 4. Vérifier Interaction Layers
        if (interactable != null && ensureInteractionLayers)
        {
            // Vérifier si au moins un layer est activé
            if (interactable.interactionLayers.value == 0)
            {
                LogWarning("⚠️ Aucun Interaction Layer activé!");
                hasIssues = true;

                if (autoFix)
                {
                    // Activer le layer 0 (Default)
                    interactable.interactionLayers = 1;
                    LogSuccess("✅ Interaction Layer 0 (Default) activé");
                }
            }
            else
            {
                LogSuccess($"✅ Interaction Layers configurés: {interactable.interactionLayers.value}");
            }
        }

        // 5. Vérifier XRInteractionManager
        if (interactable != null)
        {
            if (interactable.interactionManager == null)
            {
                LogWarning("⚠️ Pas d'XRInteractionManager assigné (il sera trouvé automatiquement au runtime)");

                // Tenter de trouver le manager dans la scène
                XRInteractionManager manager = FindObjectOfType<XRInteractionManager>();
                if (manager != null && autoFix)
                {
                    interactable.interactionManager = manager;
                    LogSuccess("✅ XRInteractionManager assigné automatiquement");
                }
            }
            else
            {
                LogSuccess("✅ XRInteractionManager assigné");
            }
        }

        // 6. Vérifier MeshRenderer (optionnel mais recommandé)
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            LogWarning("⚠️ Pas de MeshRenderer trouvé (recommandé pour le feedback visuel)");
        }
        else
        {
            LogSuccess("✅ MeshRenderer présent");
        }

        // Résumé
        if (showDebugLogs)
        {
            if (!hasIssues)
            {
                Debug.Log($"<color=green>✅ Tous les tests passés pour {gameObject.name}!</color>");
            }
            else if (autoFix)
            {
                Debug.Log($"<color=yellow>⚠️ Des problèmes ont été détectés et corrigés pour {gameObject.name}</color>");
            }
            else
            {
                Debug.Log($"<color=red>❌ Des problèmes ont été détectés pour {gameObject.name}. Activez 'autoFix' pour les corriger automatiquement.</color>");
            }
        }
    }

    [ContextMenu("Find All VR Buttons in Scene")]
    public void FindAllVRButtonsInScene()
    {
        VRButton[] buttons = FindObjectsOfType<VRButton>();
        Debug.Log($"=== Trouvé {buttons.Length} boutons VR dans la scène ===");

        foreach (VRButton button in buttons)
        {
            Debug.Log($"- {button.gameObject.name} ({button.transform.GetPath()})");
        }
    }

    [ContextMenu("Fix All VR Buttons in Scene")]
    public void FixAllVRButtonsInScene()
    {
        VRButton[] buttons = FindObjectsOfType<VRButton>();
        Debug.Log($"=== Correction de {buttons.Length} boutons VR ===");

        foreach (VRButton button in buttons)
        {
            VRButtonDiagnostic diagnostic = button.GetComponent<VRButtonDiagnostic>();
            if (diagnostic == null)
            {
                diagnostic = button.gameObject.AddComponent<VRButtonDiagnostic>();
                diagnostic.autoFix = true;
            }

            diagnostic.RunDiagnostic();
        }
    }

    void LogSuccess(string message)
    {
        if (showDebugLogs)
            Debug.Log($"<color=green>{message}</color>");
    }

    void LogWarning(string message)
    {
        if (showDebugLogs)
            Debug.LogWarning(message);
    }

    void LogError(string message)
    {
        if (showDebugLogs)
            Debug.LogError(message);
    }
}

// Extension pour obtenir le chemin complet d'un Transform
public static class TransformExtensions
{
    public static string GetPath(this Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(VRButtonDiagnostic))]
public class VRButtonDiagnosticEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VRButtonDiagnostic diagnostic = (VRButtonDiagnostic)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

        if (GUILayout.Button("🔍 Run Diagnostic", GUILayout.Height(30)))
        {
            diagnostic.RunDiagnostic();
        }

        if (GUILayout.Button("🔎 Find All VR Buttons in Scene", GUILayout.Height(25)))
        {
            diagnostic.FindAllVRButtonsInScene();
        }

        if (GUILayout.Button("🔧 Fix All VR Buttons in Scene", GUILayout.Height(25)))
        {
            diagnostic.FixAllVRButtonsInScene();
        }
    }
}
#endif
