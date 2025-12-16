using UnityEngine;
using UnityEditor;

/// <summary>
/// Helper script to quickly setup the project
/// Menu: Tools > VR Shooter > Setup Project
/// </summary>
public class ProjectSetupHelper : Editor
{
    [MenuItem("Tools/VR Shooter/Setup Project Layers")]
    static void SetupLayers()
    {
        // Create Target layer
        CreateLayer("Target");
        Debug.Log("VR Shooter: Layers setup complete!");
    }

    static void CreateLayer(string layerName)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");

        // Check if layer already exists
        for (int i = 8; i < layers.arraySize; i++)
        {
            SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
            if (layerSP.stringValue == layerName)
            {
                Debug.Log($"Layer '{layerName}' already exists");
                return;
            }
        }

        // Find empty layer slot
        for (int i = 8; i < layers.arraySize; i++)
        {
            SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(layerSP.stringValue))
            {
                layerSP.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
                Debug.Log($"Layer '{layerName}' created successfully");
                return;
            }
        }

        Debug.LogWarning($"No empty layer slot found for '{layerName}'");
    }

    [MenuItem("Tools/VR Shooter/Setup Input Actions")]
    static void SetupInputActions()
    {
        Debug.Log("Make sure to configure Input Actions manually:");
        Debug.Log("1. Open XR Interaction Toolkit settings");
        Debug.Log("2. Ensure 'XRI Default Input Actions' is enabled");
        Debug.Log("3. In WeaponController, set Shoot Action to: XRI RightHand Interaction/Activate");
    }

    [MenuItem("Tools/VR Shooter/Quick Scene Setup")]
    static void QuickSceneSetup()
    {
        // Check if XR Origin exists
        GameObject xrOrigin = GameObject.Find("XR Origin (XR Rig)");
        if (xrOrigin == null)
        {
            Debug.LogWarning("XR Origin not found! Please add 'XR Origin (XR Rig)' prefab to your scene.");
        }
        else
        {
            Debug.Log("XR Origin found!");
        }

        // Check if GameManager exists
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm == null)
        {
            GameObject gameManager = new GameObject("GameManager");
            gameManager.AddComponent<GameManager>();
            Debug.Log("GameManager created!");
        }
        else
        {
            Debug.Log("GameManager already exists!");
        }

        Debug.Log("Quick setup complete! Check the console for warnings.");
    }
}
