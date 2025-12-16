using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Attache directement l'arme à la main en tant qu'enfant
/// Pas de grab, l'arme est VRAIMENT attachée à la main
/// </summary>
public class DirectHandAttachment : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private bool attachOnStart = true;
    [SerializeField] private bool rightHand = true;
    [SerializeField] private Vector3 localPosition = new Vector3(0, 0, 0.05f);
    [SerializeField] private Vector3 localRotation = Vector3.zero;

    [Header("Optional - Pour grab plus tard")]
    [SerializeField] private bool allowDetach = true;

    private Transform handTransform;
    private bool isAttached = false;

    void Start()
    {
        if (attachOnStart)
        {
            AttachToHand();
        }
    }

    void Update()
    {
        if (allowDetach && isAttached && Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
        {
            DetachFromHand();
        }
    }

    public void AttachToHand()
    {
        // Méthode 1: Chercher via XR Origin
        var xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();

        if (xrOrigin != null)
        {
            Debug.Log("🔍 XR Origin trouvé, recherche des contrôleurs...");

            // Chercher tous les transforms
            Transform[] allTransforms = xrOrigin.GetComponentsInChildren<Transform>(true);

            foreach (Transform t in allTransforms)
            {
                Debug.Log($"  - Objet trouvé: {t.name}");

                // Chercher le bon contrôleur
                bool isRight = t.name.ToLower().Contains("right");
                bool isLeft = t.name.ToLower().Contains("left");
                bool isController = t.name.ToLower().Contains("controller") || t.name.ToLower().Contains("hand");

                if (isController && ((rightHand && isRight) || (!rightHand && isLeft)))
                {
                    handTransform = t;
                    Debug.Log($"✅ Contrôleur trouvé: {t.name}");
                    break;
                }
            }
        }
        else
        {
            Debug.LogError("❌ XR Origin non trouvé dans la scène!");
        }

        if (handTransform == null)
        {
            Debug.LogError($"❌ Impossible de trouver le contrôleur {(rightHand ? "droit" : "gauche")}");
            Debug.LogError("Utilisez Tools > VR Shooter > Print Controller Hierarchy pour voir la structure");
            return;
        }

        // Attacher l'arme comme enfant de la main
        transform.SetParent(handTransform);
        transform.localPosition = localPosition;
        transform.localRotation = Quaternion.Euler(localRotation);
        transform.localScale = Vector3.one;

        // Désactiver la physique si présente
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        isAttached = true;
        Debug.Log($"✅ Arme attachée à: {handTransform.name}");
        Debug.Log($"📍 Position locale: {transform.localPosition}");
        Debug.Log($"📍 Position mondiale: {transform.position}");
        Debug.Log($"📍 Parent complet: {GetFullPath(handTransform)}");
    }

    string GetFullPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }

    public void DetachFromHand()
    {
        if (!isAttached) return;

        transform.SetParent(null);

        // Réactiver la physique
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        isAttached = false;
        Debug.Log("🔓 Arme détachée");
    }

    public void SetHandPosition(Vector3 pos, Vector3 rot)
    {
        localPosition = pos;
        localRotation = rot;

        if (isAttached)
        {
            transform.localPosition = localPosition;
            transform.localRotation = Quaternion.Euler(localRotation);
        }
    }
}
