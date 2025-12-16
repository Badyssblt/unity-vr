using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Méthode alternative : Spawn l'arme directement sur le contrôleur
/// au lieu d'essayer de la grab depuis le monde
/// </summary>
public class SpawnWeaponInHand : MonoBehaviour
{
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private bool rightHand = true;
    [SerializeField] private float spawnDelay = 0.5f;

    void Start()
    {
        Invoke(nameof(SpawnWeapon), spawnDelay);
    }

    void SpawnWeapon()
    {
        if (weaponPrefab == null)
        {
            Debug.LogError("❌ Weapon Prefab non assigné!");
            return;
        }

        // Trouver le contrôleur
        XRBaseInteractor[] interactors = FindObjectsOfType<XRBaseInteractor>();
        XRBaseInteractor targetInteractor = null;

        foreach (XRBaseInteractor interactor in interactors)
        {
            bool isRight = interactor.name.ToLower().Contains("right");
            if ((rightHand && isRight) || (!rightHand && !isRight))
            {
                targetInteractor = interactor;
                break;
            }
        }

        if (targetInteractor == null)
        {
            Debug.LogError("❌ Contrôleur non trouvé!");
            return;
        }

        // Spawner l'arme À LA POSITION du contrôleur
        GameObject weapon = Instantiate(weaponPrefab, targetInteractor.transform.position, targetInteractor.transform.rotation);

        Debug.Log($"✅ Arme spawnée à: {weapon.transform.position}");

        // Attendre un frame puis grab
        StartCoroutine(GrabAfterFrame(weapon, targetInteractor));
    }

    System.Collections.IEnumerator GrabAfterFrame(GameObject weapon, XRBaseInteractor interactor)
    {
        yield return new WaitForEndOfFrame();

        XRGrabInteractable grabInteractable = weapon.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            var manager = grabInteractable.interactionManager;
            if (manager != null)
            {
                manager.SelectEnter(interactor, grabInteractable);
                Debug.Log($"✅ Arme grabbée!");
            }
        }
    }
}
