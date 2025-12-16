using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Affiche un cube visible là où est le contrôleur
/// Pour voir où est votre main dans le simulateur
/// </summary>
public class ShowControllerPosition : MonoBehaviour
{
    private GameObject visualCube;

    void Start()
    {
        // Créer un cube pour visualiser la position du contrôleur
        visualCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visualCube.name = "ControllerVisual";
        visualCube.transform.localScale = Vector3.one * 0.05f;

        // Couleur vive
        var renderer = visualCube.GetComponent<Renderer>();
        renderer.material.color = Color.green;

        // Pas de collisions
        Destroy(visualCube.GetComponent<Collider>());

        // Enfant du contrôleur
        visualCube.transform.SetParent(transform);
        visualCube.transform.localPosition = Vector3.zero;

        Debug.Log($"✅ Cube visuel ajouté à {gameObject.name}");
    }
}
