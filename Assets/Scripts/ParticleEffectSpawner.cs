using UnityEngine;

/// <summary>
/// Spawne des effets de particules simples à utiliser
/// </summary>
public class ParticleEffectSpawner : MonoBehaviour
{
    public static ParticleEffectSpawner Instance;

    [Header("Effect Prefabs")]
    [SerializeField] private GameObject impactEffectPrefab;
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private GameObject sparkEffectPrefab;
    [SerializeField] private GameObject smokeEffectPrefab;

    [Header("Settings")]
    [SerializeField] private float defaultLifetime = 2f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnImpactEffect(Vector3 position, Vector3 normal)
    {
        SpawnEffect(impactEffectPrefab, position, Quaternion.LookRotation(normal));
    }

    public void SpawnExplosion(Vector3 position)
    {
        SpawnEffect(explosionEffectPrefab, position, Quaternion.identity);
    }

    public void SpawnSparks(Vector3 position, Vector3 normal)
    {
        SpawnEffect(sparkEffectPrefab, position, Quaternion.LookRotation(normal));
    }

    public void SpawnSmoke(Vector3 position)
    {
        SpawnEffect(smokeEffectPrefab, position, Quaternion.identity);
    }

    public void SpawnEffect(GameObject effectPrefab, Vector3 position, Quaternion rotation)
    {
        if (effectPrefab == null) return;

        GameObject effect = Instantiate(effectPrefab, position, rotation);

        // Auto destroy after lifetime
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        float lifetime = ps != null ? ps.main.duration + ps.main.startLifetime.constantMax : defaultLifetime;

        Destroy(effect, lifetime);
    }

    // Helper pour créer un effet de particules simple
    public static GameObject CreateSimpleParticleEffect(string effectName, Color color, int count = 20)
    {
        GameObject effectObj = new GameObject(effectName);

        ParticleSystem ps = effectObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = color;
        main.startLifetime = 1f;
        main.startSize = 0.1f;
        main.startSpeed = 2f;
        main.maxParticles = count;
        main.loop = false;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, count)
        });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        var renderer = effectObj.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));

        return effectObj;
    }
}
