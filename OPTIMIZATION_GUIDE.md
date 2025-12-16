# Guide d'Optimisation pour Meta Quest

Ce guide vous aide à optimiser votre jeu VR pour atteindre les performances requises sur Meta Quest.

## 🎯 Objectifs de Performance

### Meta Quest 2
- **FPS Target** : 72 FPS minimum (90 FPS recommandé)
- **Frame Time** : < 13.8ms (11.1ms pour 90 FPS)
- **CPU** : Snapdragon XR2
- **GPU** : Adreno 650
- **RAM** : 6 GB

### Meta Quest 3
- **FPS Target** : 90 FPS (120 FPS possible)
- **CPU** : Snapdragon XR2 Gen 2
- **GPU** : Adreno 740
- **RAM** : 8 GB

## 📊 Limites Recommandées

### Géométrie
```
✅ Total Triangles: < 100,000 par frame
✅ Drawcalls: < 100
✅ Batches: < 50
✅ Vertices par mesh: < 5,000
```

### Textures
```
✅ Résolution max: 2048x2048
✅ Résolution recommandée: 512x512 - 1024x1024
✅ Format: ASTC (compression mobile)
✅ Mip Maps: Toujours activés
```

### Particules
```
✅ Max particules actives: < 1,000
✅ Max émetteurs actifs: < 10
✅ Texture size: 256x256 - 512x512
```

### Lumières
```
✅ Directional Lights: 1
✅ Point/Spot Lights: 0-2
✅ Temps réel: Minimiser
✅ Baked Lighting: Privilégier
```

## ⚙️ Configuration Unity

### 1. Project Settings

#### Graphics
```
Edit > Project Settings > Graphics

✅ Scriptable Render Pipeline: URP Asset
✅ Color Space: Linear
✅ Graphics API: OpenGL ES 3.0 + Vulkan
```

#### Quality Settings
```
Edit > Project Settings > Quality

Pour Android:
✅ Pixel Light Count: 1
✅ Texture Quality: Medium
✅ Anisotropic Textures: Per Texture
✅ Anti Aliasing: Disabled ou 2x MSAA
✅ Soft Particles: Disabled
✅ Shadows: Hard Shadows Only ou Disabled
✅ Shadow Resolution: Low-Medium
✅ Shadow Distance: 20-50
✅ Shadow Cascades: No Cascades
✅ Realtime Reflection Probes: Disabled
```

#### Player Settings
```
Edit > Project Settings > Player > Android

✅ Auto Graphics API: OFF
✅ Graphics APIs: Vulkan, OpenGL ES3
✅ Color Space: Linear
✅ Multithreaded Rendering: ON
✅ GPU Skinning: ON
✅ Target API Level: Automatic
✅ Minimum API Level: 24 (Android 7.0)
```

### 2. URP Asset Configuration

Localisez votre URP Asset dans `Settings/` :

```
Rendering:
✅ Rendering Path: Forward
✅ Depth Texture: Disabled
✅ Opaque Texture: Disabled

Quality:
✅ HDR: Disabled
✅ MSAA: Disabled ou 2x
✅ Render Scale: 1.0

Lighting:
✅ Main Light: Per Pixel
✅ Additional Lights: Disabled
✅ Cast Shadows: OFF ou Main Light Only
✅ Shadow Resolution: 512 ou 1024

Shadows:
✅ Max Distance: 30
✅ Cascade Count: 1
✅ Depth Bias: 1
✅ Normal Bias: 1

Post-processing:
✅ Activé UNIQUEMENT si nécessaire
✅ Bloom: Low quality
✅ Éviter: Color Grading, DOF, Motion Blur
```

### 3. XR Settings

```
Edit > Project Settings > XR Plug-in Management > Oculus

✅ Stereo Rendering Mode: Multiview
✅ Low Overhead Mode: Enabled
✅ Phase Sync: Disabled
✅ Optimize Buffer Discards: Enabled
✅ Subsampled Layout: Disabled
```

## 🎨 Optimisation des Assets

### Modèles 3D

#### LOD (Level of Detail)
```csharp
Utilisez des LOD groups pour les objets complexes:
- LOD0 (Close): 100% détails
- LOD1 (Medium): 50% triangles
- LOD2 (Far): 25% triangles
```

#### Mesh Optimization
```
✅ Read/Write Enabled: OFF
✅ Optimize Mesh: ON
✅ Generate Colliders: Seulement si nécessaire
✅ Normals: Calculate (si pas dans modèle)
✅ Tangents: None (si pas de normal maps)
```

### Textures

#### Import Settings
```
Texture Type: Default

✅ Max Size: 1024 (2048 max)
✅ Resize Algorithm: Bilinear
✅ Format: ASTC (Android)
✅ Compression Quality: Normal
✅ Generate Mip Maps: ON
✅ Filter Mode: Bilinear
✅ Aniso Level: 0-1
```

#### Atlas de Textures
Combinez plusieurs petites textures en une seule grande texture atlas.

### Matériaux

#### Shader Recommandés (ordre de préférence)
```
1. URP/Unlit - Le plus performant
2. URP/Simple Lit - Bon compromis
3. URP/Lit - Utiliser seulement si nécessaire
```

#### Propriétés à éviter
```
❌ Normal Maps (si possible)
❌ Height Maps
❌ Multiple texture maps
❌ Transparency (utiliser Alpha Clipping à la place)
```

### Particules

#### Optimisation des Particle Systems
```
✅ Max Particles: < 50 par système
✅ Simulation Space: World
✅ Prewarm: OFF
✅ Culling Mode: Automatic
✅ Scaling Mode: Hierarchy
✅ Play On Awake: OFF (contrôler par script)

Éviter:
❌ Collision Module
❌ Sub Emitters
❌ Lights Module
❌ Trails Module (coûteux)
```

#### Textures de Particules
```
✅ Resolution: 256x256 max
✅ Format: ASTC 4x4
✅ Shader: URP/Particles/Unlit
```

## 🔧 Optimisations Code

### 1. Object Pooling

Pour les objets fréquemment créés/détruits (projectiles, cibles) :

```csharp
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 20;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return Instantiate(prefab);
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
```

### 2. Update Optimizations

```csharp
// ❌ Éviter dans Update()
void Update()
{
    GameObject enemy = GameObject.Find("Enemy"); // Coûteux !
    Camera.main; // Cache dans Start() !
    GetComponent<Rigidbody>(); // Cache dans Start() !
}

// ✅ Bon
private Camera mainCamera;
private Rigidbody rb;

void Start()
{
    mainCamera = Camera.main;
    rb = GetComponent<Rigidbody>();
}

void Update()
{
    // Utiliser les références cachées
}
```

### 3. Physique

```csharp
✅ Utiliser Layers pour raycast
✅ Minimiser les colliders complexes (MeshCollider)
✅ Privilégier Box/Sphere/Capsule Colliders
✅ Fixed Timestep: 0.02 (50 Hz)
✅ Désactiver Rigidbody sur objets statiques
```

### 4. GC (Garbage Collection)

```csharp
// ❌ Éviter allocations dans Update
void Update()
{
    string text = "Score: " + score; // Allocation !
    Vector3 pos = new Vector3(); // Allocation !
}

// ✅ Bon
private Vector3 cachedPosition;
private StringBuilder scoreBuilder = new StringBuilder();

void Update()
{
    scoreBuilder.Clear();
    scoreBuilder.Append("Score: ");
    scoreBuilder.Append(score);
}
```

## 🏗️ Optimisations Scène

### Occlusion Culling

Active le culling des objets non visibles :

```
1. Window > Rendering > Occlusion Culling
2. Marquer objets statiques comme "Occluder Static"
3. Marquer petits objets comme "Occludee Static"
4. Bake Occlusion Data
```

### Baked Lighting

```
1. Window > Rendering > Lighting
2. Mixed Lighting: Baked Indirect
3. Lightmap Settings:
   - Lightmapper: Progressive GPU
   - Direct Samples: 32
   - Indirect Samples: 128
   - Lightmap Resolution: 10-20
   - Lightmap Size: 512-1024
4. Generate Lighting
```

### Static Batching

Marquez les objets immobiles comme "Static" :
```
✅ Static meshes
✅ Environnement
✅ Props qui ne bougent jamais
```

### Disable Unnecessary Components

```csharp
// Désactiver quand hors vue
void OnBecameInvisible()
{
    GetComponent<Animator>().enabled = false;
}

void OnBecameVisible()
{
    GetComponent<Animator>().enabled = true;
}
```

## 📱 Build Settings

### Build Configuration

```
File > Build Settings

✅ Platform: Android
✅ Texture Compression: ASTC
✅ Development Build: OFF (pour build final)
✅ Compression Method: LZ4HC
✅ Split Application Binary: ON (si > 100MB)
```

### Stripping

```
Player Settings > Android > Other Settings

✅ Strip Engine Code: ON
✅ Managed Stripping Level: High
✅ Vertex Compression: Everything
✅ Optimize Mesh Data: ON
```

## 🔍 Profiling & Debugging

### Unity Profiler

```
Window > Analysis > Profiler

Surveiller:
- CPU: < 11ms total
- Rendering: < 6ms
- Scripts: < 2ms
- Physics: < 1ms
- GC.Alloc: Minimiser
```

### Oculus Developer Hub

1. Installer Oculus Developer Hub
2. Connecter Quest via USB
3. Performance HUD: Level 3
4. Surveiller:
   - FPS
   - CPU/GPU timing
   - Thermal state

### RenderDoc

Pour analyse détaillée du rendu :
```
1. Capture frame sur Quest
2. Analyser drawcalls
3. Identifier bottlenecks GPU
```

## ✅ Checklist d'Optimisation

### Avant Build
- [ ] URP Asset configuré pour mobile
- [ ] Quality Settings: Medium ou Low
- [ ] Textures: ASTC compression
- [ ] Models: Optimized, < 5k verts
- [ ] Shaders: URP Unlit/Simple Lit
- [ ] Lighting: Baked
- [ ] Occlusion Culling: Baked
- [ ] Static Batching: Activé
- [ ] Physics: Layers configurés
- [ ] Audio: Compressed
- [ ] Post-processing: Minimal

### Tests
- [ ] FPS constant > 72
- [ ] Pas de frame drops
- [ ] Pas de stuttering
- [ ] Tracking stable
- [ ] Pas de heat throttling après 30min
- [ ] Profiler: Tous sous limites

### Quest Spécifique
- [ ] Stereo Rendering: Multiview
- [ ] Resolution: Auto (Quest handle)
- [ ] FFR: Fixed Foveated Rendering activé
- [ ] Refresh Rate: 72Hz stable

## 🚀 Quick Wins (Optimisations rapides)

### Top 10 Optimisations Immédiates
1. URP Asset sur "Medium" quality
2. Shadows: Disabled ou Main Light Only
3. MSAA: Disabled
4. Textures: Max 1024x1024, ASTC
5. Shaders: URP/Unlit pour tout ce qui est possible
6. Baked Lighting au lieu de Realtime
7. Static Batching pour environnement
8. Object Pooling pour projectiles
9. Cache Component references
10. Disable Post-Processing

## 📚 Resources

### Documentation
- [Unity XR Performance](https://docs.unity3d.com/Manual/xr_performance.html)
- [Oculus Best Practices](https://developer.oculus.com/documentation/unity/unity-best-practices/)
- [URP Optimization](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@14.0/manual/optimization.html)

### Tools
- Unity Profiler
- Oculus Developer Hub
- RenderDoc
- Frame Debugger (Window > Analysis > Frame Debugger)

---

**Note** : L'optimisation est un processus itératif. Testez régulièrement sur le casque réel ! 🎮
