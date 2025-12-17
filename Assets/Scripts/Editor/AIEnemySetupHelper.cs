using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

#if UNITY_EDITOR
/// <summary>
/// Script d'aide pour configurer rapidement une IA ennemie
/// Menu: GameObject > AI Setup > Create Enemy AI
/// </summary>
public class AIEnemySetupHelper : EditorWindow
{
    private GameObject enemyModel;
    private GameObject weaponPrefab;
    private float health = 100f;
    private float detectionRange = 15f;
    private float attackRange = 10f;
    private string enemyName = "Enemy AI";

    [MenuItem("GameObject/AI Setup/Create Enemy AI", false, 0)]
    static void ShowWindow()
    {
        AIEnemySetupHelper window = GetWindow<AIEnemySetupHelper>("AI Enemy Setup");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Configuration de l'IA Ennemie", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        enemyName = EditorGUILayout.TextField("Nom de l'ennemi", enemyName);
        enemyModel = (GameObject)EditorGUILayout.ObjectField("Modèle 3D (optionnel)", enemyModel, typeof(GameObject), false);
        weaponPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab d'arme", weaponPrefab, typeof(GameObject), false);

        EditorGUILayout.Space();
        GUILayout.Label("Paramètres de combat", EditorStyles.boldLabel);
        health = EditorGUILayout.FloatField("Points de vie", health);
        detectionRange = EditorGUILayout.FloatField("Portée de détection", detectionRange);
        attackRange = EditorGUILayout.FloatField("Portée d'attaque", attackRange);

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Ce script va créer un ennemi IA avec:\n" +
            "• NavMeshAgent pour la navigation\n" +
            "• AIController pour le comportement\n" +
            "• HealthSystem pour les PV\n" +
            "• AIWeaponHandler pour tirer\n" +
            "• DamageableHitbox sur les colliders\n" +
            "• WeaponController configuré pour l'équipe Enemy",
            MessageType.Info
        );

        EditorGUILayout.Space();

        if (GUILayout.Button("Créer l'ennemi IA", GUILayout.Height(40)))
        {
            CreateEnemyAI();
        }
    }

    void CreateEnemyAI()
    {
        // Créer le GameObject principal
        GameObject enemy = new GameObject(enemyName);
        enemy.tag = "Enemy";

        // Ajouter un visuel par défaut ou utiliser le modèle fourni
        GameObject visual;
        if (enemyModel != null)
        {
            visual = PrefabUtility.InstantiatePrefab(enemyModel) as GameObject;
            visual.transform.SetParent(enemy.transform);
            visual.transform.localPosition = Vector3.zero;
        }
        else
        {
            // Créer un capsule par défaut
            visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "Visual";
            visual.transform.SetParent(enemy.transform);
            visual.transform.localPosition = Vector3.zero;

            // Retirer le collider du visuel (on va en ajouter un séparé)
            DestroyImmediate(visual.GetComponent<Collider>());
        }

        // Ajouter un CharacterController pour le mouvement
        CapsuleCollider capsuleCollider = enemy.AddComponent<CapsuleCollider>();
        capsuleCollider.height = 2f;
        capsuleCollider.radius = 0.5f;
        capsuleCollider.center = new Vector3(0, 1f, 0);

        // Ajouter Rigidbody (kinematic pour NavMesh)
        Rigidbody rb = enemy.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // Ajouter NavMeshAgent
        NavMeshAgent agent = enemy.AddComponent<NavMeshAgent>();
        agent.height = 2f;
        agent.radius = 0.5f;
        agent.speed = 3.5f;
        agent.angularSpeed = 120f;
        agent.acceleration = 8f;

        // Ajouter HealthSystem
        HealthSystem healthSystem = enemy.AddComponent<HealthSystem>();
        SerializedObject healthSystemSO = new SerializedObject(healthSystem);
        healthSystemSO.FindProperty("maxHealth").floatValue = health;
        healthSystemSO.FindProperty("teamTag").stringValue = "Enemy";
        healthSystemSO.ApplyModifiedProperties();

        // Ajouter AIController
        AIController aiController = enemy.AddComponent<AIController>();
        SerializedObject aiControllerSO = new SerializedObject(aiController);
        aiControllerSO.FindProperty("detectionRange").floatValue = detectionRange;
        aiControllerSO.FindProperty("attackRange").floatValue = attackRange;
        aiControllerSO.ApplyModifiedProperties();

        // Ajouter AIWeaponHandler
        AIWeaponHandler weaponHandler = enemy.AddComponent<AIWeaponHandler>();

        // Si une arme est fournie, l'attacher
        if (weaponPrefab != null)
        {
            GameObject weapon = PrefabUtility.InstantiatePrefab(weaponPrefab) as GameObject;
            weapon.transform.SetParent(enemy.transform);
            weapon.transform.localPosition = new Vector3(0.5f, 1.5f, 0.5f); // Position approximative de la main
            weapon.transform.localRotation = Quaternion.Euler(0, 90, 0);

            // Configurer le WeaponController
            WeaponController weaponController = weapon.GetComponent<WeaponController>();
            if (weaponController != null)
            {
                SerializedObject weaponSO = new SerializedObject(weaponController);
                weaponSO.FindProperty("ownerTeam").stringValue = "Enemy";
                weaponSO.ApplyModifiedProperties();

                // Lier l'arme au AIWeaponHandler
                SerializedObject handlerSO = new SerializedObject(weaponHandler);
                handlerSO.FindProperty("weaponController").objectReferenceValue = weaponController;
                handlerSO.ApplyModifiedProperties();
            }

            // Désactiver XRGrabInteractable sur l'arme de l'IA
            var grabInteractable = weapon.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
            if (grabInteractable != null)
            {
                grabInteractable.enabled = false;
            }
        }

        // Créer des hitboxes
        CreateHitbox(enemy.transform, "Head Hitbox", new Vector3(0, 1.7f, 0), new Vector3(0.3f, 0.3f, 0.3f), DamageableHitbox.HitboxType.Head, healthSystem);
        CreateHitbox(enemy.transform, "Body Hitbox", new Vector3(0, 1.0f, 0), new Vector3(0.5f, 0.8f, 0.3f), DamageableHitbox.HitboxType.Body, healthSystem);
        CreateHitbox(enemy.transform, "Legs Hitbox", new Vector3(0, 0.5f, 0), new Vector3(0.4f, 0.5f, 0.3f), DamageableHitbox.HitboxType.Limb, healthSystem);

        // Positionner l'ennemi dans la scène
        if (SceneView.lastActiveSceneView != null)
        {
            Vector3 spawnPos = SceneView.lastActiveSceneView.camera.transform.position + SceneView.lastActiveSceneView.camera.transform.forward * 5f;
            enemy.transform.position = spawnPos;
        }

        // Sélectionner l'objet créé
        Selection.activeGameObject = enemy;

        Debug.Log($"✅ Ennemi IA créé: {enemyName}");
        Debug.Log($"⚠️ N'oubliez pas de:\n" +
                  "1. Bake le NavMesh (Window > AI > Navigation)\n" +
                  "2. Assigner le joueur dans AIController (ou activer autoFindPlayer)\n" +
                  "3. Configurer les layers pour les collisions");
    }

    void CreateHitbox(Transform parent, string name, Vector3 position, Vector3 size, DamageableHitbox.HitboxType type, HealthSystem healthSystem)
    {
        GameObject hitbox = new GameObject(name);
        hitbox.transform.SetParent(parent);
        hitbox.transform.localPosition = position;
        hitbox.layer = parent.gameObject.layer;

        BoxCollider col = hitbox.AddComponent<BoxCollider>();
        col.size = size;
        col.isTrigger = false; // Les hitboxes doivent être des colliders normaux

        DamageableHitbox damageableHitbox = hitbox.AddComponent<DamageableHitbox>();
        SerializedObject hitboxSO = new SerializedObject(damageableHitbox);
        hitboxSO.FindProperty("healthSystem").objectReferenceValue = healthSystem;
        hitboxSO.FindProperty("hitboxType").enumValueIndex = (int)type;
        hitboxSO.ApplyModifiedProperties();
    }

    [MenuItem("GameObject/AI Setup/Quick Enemy (No Weapon)", false, 1)]
    static void CreateQuickEnemy()
    {
        AIEnemySetupHelper helper = CreateInstance<AIEnemySetupHelper>();
        helper.enemyName = "Enemy " + Random.Range(1000, 9999);
        helper.health = 100f;
        helper.detectionRange = 15f;
        helper.attackRange = 10f;
        helper.CreateEnemyAI();
    }
}
#endif
