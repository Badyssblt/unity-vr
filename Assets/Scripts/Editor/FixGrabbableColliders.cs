using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Utilitaire Editor pour fixer automatiquement les XRGrabInteractable qui n'ont pas de colliders assignés
/// </summary>
public class FixGrabbableColliders : EditorWindow
{
    [MenuItem("Tools/XR/Fix Grabbable Colliders")]
    public static void FixAllGrabbables()
    {
        // Trouver tous les XRGrabInteractable dans la scène et les prefabs
        XRGrabInteractable[] allGrabbables = Resources.FindObjectsOfTypeAll<XRGrabInteractable>();

        int fixedCount = 0;

        foreach (XRGrabInteractable grabbable in allGrabbables)
        {
            // Vérifier si les colliders sont vides
            if (grabbable.colliders.Count == 0)
            {
                // Ajouter le script AutoAssignColliders s'il n'existe pas déjà
                if (grabbable.GetComponent<AutoAssignColliders>() == null)
                {
                    Undo.AddComponent<AutoAssignColliders>(grabbable.gameObject);
                    fixedCount++;
                    Debug.Log($"✅ AutoAssignColliders ajouté à : {grabbable.gameObject.name}");
                }
            }
        }

        if (fixedCount > 0)
        {
            Debug.Log($"✅ {fixedCount} objet(s) fixé(s) avec AutoAssignColliders");
            EditorUtility.DisplayDialog("Succès",
                $"{fixedCount} objet(s) ont été corrigés.\n\nLe script AutoAssignColliders va maintenant assigner automatiquement les colliders au démarrage.",
                "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Info",
                "Aucun objet à corriger trouvé.",
                "OK");
        }
    }

    [MenuItem("Tools/XR/Fix LaserGun Prefab")]
    public static void FixLaserGunPrefab()
    {
        // Charger le prefab LaserGun
        string[] guids = AssetDatabase.FindAssets("LaserGun t:GameObject");

        if (guids.Length == 0)
        {
            Debug.LogError("❌ Prefab LaserGun non trouvé");
            return;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        if (prefab == null)
        {
            Debug.LogError("❌ Impossible de charger le prefab");
            return;
        }

        // Ouvrir le prefab pour modification
        string prefabPath = AssetDatabase.GetAssetPath(prefab);
        GameObject prefabContents = PrefabUtility.LoadPrefabContents(prefabPath);

        XRGrabInteractable grabbable = prefabContents.GetComponent<XRGrabInteractable>();

        if (grabbable == null)
        {
            Debug.LogError("❌ Pas de XRGrabInteractable sur LaserGun");
            PrefabUtility.UnloadPrefabContents(prefabContents);
            return;
        }

        // Ajouter AutoAssignColliders si nécessaire
        if (prefabContents.GetComponent<AutoAssignColliders>() == null)
        {
            prefabContents.AddComponent<AutoAssignColliders>();
            Debug.Log("✅ AutoAssignColliders ajouté au prefab LaserGun");
        }

        // Assigner manuellement les colliders si possible
        Collider[] colliders = prefabContents.GetComponentsInChildren<Collider>();
        if (colliders.Length > 0)
        {
            grabbable.colliders.Clear();
            foreach (Collider col in colliders)
            {
                grabbable.colliders.Add(col);
            }
            Debug.Log($"✅ {colliders.Length} collider(s) assigné(s) au prefab LaserGun");
        }

        // Sauvegarder le prefab
        PrefabUtility.SaveAsPrefabAsset(prefabContents, prefabPath);
        PrefabUtility.UnloadPrefabContents(prefabContents);

        EditorUtility.DisplayDialog("Succès",
            "Le prefab LaserGun a été corrigé !\n\nLes colliders ont été assignés et le script AutoAssignColliders a été ajouté.",
            "OK");
    }
}
