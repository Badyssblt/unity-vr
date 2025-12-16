using UnityEngine;
using UnityEditor;
using TMPro;

/// <summary>
/// Crée automatiquement tous les prefabs et assets nécessaires pour le jeu VR
/// </summary>
public class GameAssetCreator : EditorWindow
{
    [MenuItem("Tools/VR Shooter/Create All Game Assets")]
    public static void CreateAllAssets()
    {
        if (EditorUtility.DisplayDialog("Créer tous les assets",
            "Cela va créer tous les prefabs et assets manquants pour votre jeu VR.\n\n" +
            "Les assets seront créés dans Assets/Prefabs/",
            "Créer", "Annuler"))
        {
            CreatePrefabFolders();
            CreateParticleEffects();
            CreateFloatingTextPrefab();
            CreateUIElements();
            CreateTargetPrefabs();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Succès",
                "Tous les assets ont été créés avec succès!\n\n" +
                "Vérifiez le dossier Assets/Prefabs/",
                "OK");
        }
    }

    [MenuItem("Tools/VR Shooter/Create Particle Effects")]
    public static void CreateParticleEffects()
    {
        string prefabPath = "Assets/Prefabs/Effects";
        if (!AssetDatabase.IsValidFolder(prefabPath))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "Effects");
        }

        // Impact Effect
        CreateParticleEffect("ImpactEffect", Color.yellow, 30, prefabPath);

        // Explosion Effect
        CreateParticleEffect("ExplosionEffect", new Color(1f, 0.5f, 0f), 50, prefabPath);

        // Spark Effect
        CreateParticleEffect("SparkEffect", Color.white, 20, prefabPath);

        // Target Destruction Effect
        CreateParticleEffect("TargetDestructionEffect", Color.red, 40, prefabPath);

        Debug.Log("✅ Effets de particules créés dans " + prefabPath);
    }

    static void CreateParticleEffect(string name, Color color, int particleCount, string path)
    {
        GameObject effect = ParticleEffectSpawner.CreateSimpleParticleEffect(name, color, particleCount);

        string prefabPath = $"{path}/{name}.prefab";
        PrefabUtility.SaveAsPrefabAsset(effect, prefabPath);

        DestroyImmediate(effect);
    }

    [MenuItem("Tools/VR Shooter/Create Floating Text Prefab")]
    public static void CreateFloatingTextPrefab()
    {
        string prefabPath = "Assets/Prefabs/UI";
        if (!AssetDatabase.IsValidFolder(prefabPath))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "UI");
        }

        GameObject floatingTextObj = new GameObject("FloatingText");

        // Add TextMeshPro
        TextMeshPro textMesh = floatingTextObj.AddComponent<TextMeshPro>();
        textMesh.text = "+10";
        textMesh.fontSize = 5;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.color = Color.yellow;

        // Add FloatingText script
        FloatingText floatingText = floatingTextObj.AddComponent<FloatingText>();

        // Créer le prefab
        string fullPath = $"{prefabPath}/FloatingText.prefab";
        PrefabUtility.SaveAsPrefabAsset(floatingTextObj, fullPath);

        DestroyImmediate(floatingTextObj);

        Debug.Log("✅ Prefab FloatingText créé: " + fullPath);
    }

    [MenuItem("Tools/VR Shooter/Create Target Prefabs")]
    public static void CreateTargetPrefabs()
    {
        string prefabPath = "Assets/Prefabs/Targets";
        if (!AssetDatabase.IsValidFolder(prefabPath))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "Targets");
        }

        // Cible statique
        CreateTargetPrefab("StaticTarget", false, Color.red, prefabPath);

        // Cible mobile
        CreateTargetPrefab("MovingTarget", true, Color.blue, prefabPath);

        Debug.Log("✅ Prefabs de cibles créés dans " + prefabPath);
    }

    static void CreateTargetPrefab(string name, bool isMoving, Color color, string path)
    {
        GameObject targetObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        targetObj.name = name;

        // Matériau
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = color;
        targetObj.GetComponent<Renderer>().material = mat;

        // Layer pour le raycast
        targetObj.layer = LayerMask.NameToLayer("Default");

        // Ajouter le script Target
        Target target = targetObj.AddComponent<Target>();

        // Configurer le movement si nécessaire
        if (isMoving)
        {
            // Ces valeurs seront configurables dans l'inspector
        }

        string prefabPath = $"{path}/{name}.prefab";
        PrefabUtility.SaveAsPrefabAsset(targetObj, prefabPath);

        DestroyImmediate(targetObj);
    }

    [MenuItem("Tools/VR Shooter/Create UI Canvas")]
    public static void CreateUIElements()
    {
        // Créer le canvas principal s'il n'existe pas
        Canvas mainCanvas = FindObjectOfType<Canvas>();

        if (mainCanvas == null)
        {
            GameObject canvasObj = new GameObject("MainCanvas");
            mainCanvas = canvasObj.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.WorldSpace;

            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            // Positionner le canvas
            RectTransform rt = canvasObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(1000, 600);
            canvasObj.transform.position = new Vector3(0, 2, 3);

            Debug.Log("✅ Canvas principal créé");
        }
    }

    static void CreatePrefabFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        string[] folders = { "Effects", "UI", "Targets", "Weapons" };
        foreach (string folder in folders)
        {
            string path = $"Assets/Prefabs/{folder}";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets/Prefabs", folder);
            }
        }

        Debug.Log("✅ Dossiers de prefabs créés");
    }
}
