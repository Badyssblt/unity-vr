using UnityEngine;
using UnityEditor;

/// <summary>
/// Crée une arme SIMPLE directement attachée à la main
/// Pas de XRGrabInteractable, juste un enfant du contrôleur
/// Menu: Tools > VR Shooter > Create Attached Weapon (Simple)
/// </summary>
public class SimpleAttachedWeapon : Editor
{
    [MenuItem("Tools/VR Shooter/Create Attached Weapon (Simple)")]
    static void CreateAttachedWeapon()
    {
        // Créer le GameObject arme
        GameObject weapon = new GameObject("AttachedWeapon");

        // Ajouter un visuel simple
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.name = "Visual";
        visual.transform.SetParent(weapon.transform);
        visual.transform.localPosition = new Vector3(0, 0, 0.15f);
        visual.transform.localScale = new Vector3(0.1f, 0.05f, 0.3f);

        // Pas besoin de Rigidbody ni Collider pour une arme attachée
        // Mais on les garde au cas où on veut détacher
        Rigidbody rb = weapon.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        BoxCollider collider = weapon.AddComponent<BoxCollider>();
        collider.center = new Vector3(0, 0, 0.15f);
        collider.size = new Vector3(0.1f, 0.05f, 0.3f);
        collider.enabled = false; // Désactivé car attaché

        // Script d'attachement direct
        DirectHandAttachment attachment = weapon.AddComponent<DirectHandAttachment>();

        // Configurer la position par défaut (vous pouvez ajuster)
        SerializedObject serializedAttachment = new SerializedObject(attachment);
        serializedAttachment.FindProperty("localPosition").vector3Value = new Vector3(0, 0, 0.05f);
        serializedAttachment.FindProperty("localRotation").vector3Value = new Vector3(-10, 0, 0);
        serializedAttachment.ApplyModifiedProperties();

        // Ajouter FirePoint
        GameObject firePoint = new GameObject("FirePoint");
        firePoint.transform.SetParent(weapon.transform);
        firePoint.transform.localPosition = new Vector3(0, 0, 0.3f);

        // Ajouter le script de tir simple
        weapon.AddComponent<SimpleGunBehaviour>();

        // Sélectionner l'arme
        Selection.activeGameObject = weapon;

        Debug.Log("✅ Arme attachée créée!");
        Debug.Log("📍 L'arme sera DIRECTEMENT dans votre main au Play");
        Debug.Log("🎮 Appuyez sur Play pour voir - Pas besoin de grab!");
        Debug.Log("🔫 Utilisez Clic Gauche ou Espace pour tirer");
        Debug.Log("💡 Ajustez la position dans Direct Hand Attachment si besoin");

        EditorGUIUtility.PingObject(weapon);
    }

    [MenuItem("Tools/VR Shooter/Print Controller Hierarchy")]
    static void PrintControllerHierarchy()
    {
        Debug.Log("=== RECHERCHE DES CONTRÔLEURS ===");

        // Chercher XR Origin
        var xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
        if (xrOrigin == null)
        {
            Debug.LogError("❌ XR Origin non trouvé!");
            return;
        }

        Debug.Log("✅ XR Origin trouvé: " + xrOrigin.name);

        // Lister toute la hiérarchie
        PrintHierarchy(xrOrigin.transform, 0);

        Debug.Log("=== FIN ===");
    }

    static void PrintHierarchy(Transform parent, int indent)
    {
        string indentString = new string(' ', indent * 2);
        Debug.Log($"{indentString}└─ {parent.name}");

        foreach (Transform child in parent)
        {
            PrintHierarchy(child, indent + 1);
        }
    }
}
