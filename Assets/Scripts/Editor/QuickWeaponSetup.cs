using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Crée automatiquement une arme grabbable de test
/// Menu: Tools > VR Shooter > Create Test Weapon
/// </summary>
public class QuickWeaponSetup : Editor
{
    [MenuItem("Tools/VR Shooter/Create Test Weapon")]
    static void CreateTestWeapon()
    {
        // Vérifier XR Interaction Manager
        if (FindObjectOfType<XRInteractionManager>() == null)
        {
            GameObject manager = new GameObject("XR Interaction Manager");
            manager.AddComponent<XRInteractionManager>();
            Debug.Log("✅ XR Interaction Manager créé");
        }

        // Créer le GameObject arme
        GameObject weapon = new GameObject("TestWeapon");
        weapon.transform.position = new Vector3(0, 1.2f, 1f); // Devant le joueur

        // Ajouter un visuel simple
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.name = "Visual";
        visual.transform.SetParent(weapon.transform);
        visual.transform.localPosition = new Vector3(0, 0, 0.15f);
        visual.transform.localScale = new Vector3(0.1f, 0.05f, 0.3f);

        // Supprimer le collider du visuel (on le met sur le parent)
        DestroyImmediate(visual.GetComponent<Collider>());

        // Ajouter Rigidbody
        Rigidbody rb = weapon.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = 0.5f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Ajouter Collider
        BoxCollider collider = weapon.AddComponent<BoxCollider>();
        collider.center = new Vector3(0, 0, 0.15f);
        collider.size = new Vector3(0.1f, 0.05f, 0.3f);

        // Ajouter XR Grab Interactable
        XRGrabInteractable grabInteractable = weapon.AddComponent<XRGrabInteractable>();
        grabInteractable.movementType = XRBaseInteractable.MovementType.Instantaneous;
        grabInteractable.trackPosition = true;
        grabInteractable.trackRotation = true;
        grabInteractable.throwOnDetach = false;

        // Ajouter script de debug
        weapon.AddComponent<DebugGrab>();

        // Ajouter script pour désactiver collisions
        weapon.AddComponent<DisableColliderOnGrab>();

        // Ajouter script pour attacher automatiquement à la main
        weapon.AddComponent<WeaponAttacher>();

        // Ajouter AttachTransform (point d'attache pour la main)
        GameObject attachTransform = new GameObject("AttachTransform");
        attachTransform.transform.SetParent(weapon.transform);
        attachTransform.transform.localPosition = new Vector3(0, -0.05f, 0.1f); // Poignée
        attachTransform.transform.localRotation = Quaternion.identity;

        // Assigner l'AttachTransform au XRGrabInteractable
        grabInteractable.attachTransform = attachTransform.transform;

        // Ajouter FirePoint
        GameObject firePoint = new GameObject("FirePoint");
        firePoint.transform.SetParent(weapon.transform);
        firePoint.transform.localPosition = new Vector3(0, 0, 0.3f);

        // Sélectionner l'arme
        Selection.activeGameObject = weapon;

        Debug.Log("✅ Arme de test créée à la position (0, 1.2, 1)");
        Debug.Log("📍 L'arme devrait être visible devant vous en Play mode");
        Debug.Log("🎮 Utilisez Tab + Souris + Clic Droit (ou G) pour grab");
        Debug.Log("🔒 Le collider sera désactivé automatiquement quand grabbée");

        EditorGUIUtility.PingObject(weapon);
    }

    [MenuItem("Tools/VR Shooter/Check XR Setup")]
    static void CheckXRSetup()
    {
        Debug.Log("=== VÉRIFICATION DE LA CONFIGURATION XR ===");

        // Check XR Origin
        var xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
        if (xrOrigin != null)
            Debug.Log("✅ XR Origin trouvé: " + xrOrigin.name);
        else
            Debug.LogError("❌ XR Origin MANQUANT! Ajoutez le prefab 'XR Origin (XR Rig)'");

        // Check Interaction Manager
        var interactionManager = FindObjectOfType<XRInteractionManager>();
        if (interactionManager != null)
            Debug.Log("✅ XR Interaction Manager trouvé");
        else
            Debug.LogWarning("⚠️ XR Interaction Manager manquant");

        // Check Interactors
        var interactors = FindObjectsOfType<XRBaseInteractor>();
        if (interactors.Length > 0)
            Debug.Log($"✅ {interactors.Length} Interactor(s) trouvé(s)");
        else
            Debug.LogError("❌ Aucun Interactor trouvé! Vérifiez les contrôleurs");

        // Check Interactables
        var interactables = FindObjectsOfType<XRGrabInteractable>();
        if (interactables.Length > 0)
            Debug.Log($"✅ {interactables.Length} objet(s) grabbable(s) trouvé(s)");
        else
            Debug.LogWarning("⚠️ Aucun objet grabbable dans la scène");

        Debug.Log("=== FIN DE LA VÉRIFICATION ===");
    }

    [MenuItem("Tools/VR Shooter/Add Direct Interactors to Controllers")]
    static void AddDirectInteractors()
    {
        // Trouver les contrôleurs
        var controllers = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.ActionBasedController>();

        if (controllers.Length == 0)
        {
            Debug.LogError("❌ Aucun contrôleur trouvé! Assurez-vous que XR Origin est dans la scène.");
            return;
        }

        foreach (var controller in controllers)
        {
            // Vérifier s'il a déjà un Direct Interactor
            if (controller.GetComponentInChildren<XRDirectInteractor>() == null)
            {
                GameObject interactorObj = new GameObject("Direct Interactor");
                interactorObj.transform.SetParent(controller.transform);
                interactorObj.transform.localPosition = Vector3.zero;
                interactorObj.transform.localRotation = Quaternion.identity;

                var directInteractor = interactorObj.AddComponent<XRDirectInteractor>();

                // Ajouter un trigger collider
                SphereCollider sphereCollider = interactorObj.AddComponent<SphereCollider>();
                sphereCollider.isTrigger = true;
                sphereCollider.radius = 0.1f;

                Debug.Log($"✅ Direct Interactor ajouté à {controller.name}");
            }
            else
            {
                Debug.Log($"ℹ️ {controller.name} a déjà un Direct Interactor");
            }
        }
    }
}
