using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Crée une arme avec la BONNE méthode XR Interaction Toolkit
/// Menu: Tools > VR Shooter > Create Weapon (XR Method - CORRECT)
/// </summary>
public class ProperXRWeapon : Editor
{
    [MenuItem("Tools/VR Shooter/Create Weapon (XR Method - CORRECT)")]
    static void CreateProperXRWeapon()
    {
        // Créer l'arme
        GameObject weapon = new GameObject("Weapon_XR");
        weapon.transform.position = new Vector3(0, 1.2f, 1f);

        // Visuel
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.name = "Visual";
        visual.transform.SetParent(weapon.transform);
        visual.transform.localPosition = new Vector3(0, 0, 0.15f);
        visual.transform.localScale = new Vector3(0.1f, 0.05f, 0.3f);
        DestroyImmediate(visual.GetComponent<Collider>());

        // Rigidbody
        Rigidbody rb = weapon.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = 0.5f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Collider
        BoxCollider collider = weapon.AddComponent<BoxCollider>();
        collider.center = new Vector3(0, 0, 0.15f);
        collider.size = new Vector3(0.1f, 0.05f, 0.3f);

        // XR Grab Interactable
        XRGrabInteractable grabInteractable = weapon.AddComponent<XRGrabInteractable>();
        grabInteractable.movementType = XRBaseInteractable.MovementType.Instantaneous;
        grabInteractable.trackPosition = true;
        grabInteractable.trackRotation = true;
        grabInteractable.throwOnDetach = false;

        // AttachTransform (LA CLÉ!)
        GameObject attachTransform = new GameObject("AttachTransform");
        attachTransform.transform.SetParent(weapon.transform);
        attachTransform.transform.localPosition = new Vector3(0, -0.02f, 0.08f);
        attachTransform.transform.localRotation = Quaternion.Euler(-15, 0, 0);

        // Assigner l'AttachTransform
        grabInteractable.attachTransform = attachTransform.transform;

        // FirePoint
        GameObject firePoint = new GameObject("FirePoint");
        firePoint.transform.SetParent(weapon.transform);
        firePoint.transform.localPosition = new Vector3(0, 0, 0.3f);

        // Scripts
        weapon.AddComponent<SimpleGunBehaviour>();
        weapon.AddComponent<DisableColliderOnGrab>();
        weapon.AddComponent<AutoGrabOnStart>(); // Auto-grab au démarrage

        Selection.activeGameObject = weapon;

        Debug.Log("✅ Arme créée avec la BONNE méthode XR Interaction Toolkit!");
        Debug.Log("📍 Composants:");
        Debug.Log("   - XR Grab Interactable (système standard)");
        Debug.Log("   - AttachTransform (définit comment tenir l'arme)");
        Debug.Log("   - Auto Grab On Start (grab automatique)");
        Debug.Log("🎮 L'arme sera grabbée automatiquement au démarrage");
        Debug.Log("💡 Ajustez la position en modifiant l'AttachTransform dans la hiérarchie");

        EditorGUIUtility.PingObject(weapon);
    }
}
