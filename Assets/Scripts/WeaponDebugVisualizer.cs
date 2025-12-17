using UnityEngine;

/// <summary>
/// Visualise les tirs du WeaponController en temps réel
/// Aide à débugger pourquoi les balles ne touchent pas
/// </summary>
public class WeaponDebugVisualizer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool showRaycastLines = true;
    [SerializeField] private float lineDuration = 2f;
    [SerializeField] private Color hitColor = Color.green;
    [SerializeField] private Color missColor = Color.red;

    [Header("Info")]
    [SerializeField] private int totalShots = 0;
    [SerializeField] private int totalHits = 0;
    [SerializeField] private int totalMisses = 0;

    private WeaponController weaponController;

    void Awake()
    {
        weaponController = GetComponent<WeaponController>();
    }

    void Update()
    {
        // Afficher des infos dans l'Inspector
        if (weaponController != null)
        {
            // Vous pouvez ajouter des stats ici
        }
    }

    public void OnShootRaycast(Vector3 origin, Vector3 direction, float distance, bool hit, Vector3 hitPoint)
    {
        totalShots++;

        if (showRaycastLines)
        {
            if (hit)
            {
                totalHits++;
                Debug.DrawLine(origin, hitPoint, hitColor, lineDuration);
                Debug.Log($"✅ Tir #{totalShots} TOUCHÉ à {Vector3.Distance(origin, hitPoint):F2}m");
            }
            else
            {
                totalMisses++;
                Debug.DrawLine(origin, origin + direction * distance, missColor, lineDuration);
                Debug.Log($"❌ Tir #{totalShots} RATÉ");
            }
        }
    }
}
