# Guide des Effets Visuels (Juice)

Ce guide explique comment créer tous les effets visuels pour votre jeu VR.

## 1. Muzzle Flash (Éclair de tir)

### Création du Particle System

1. Sélectionnez votre arme dans la hiérarchie
2. `Clic droit > Effects > Particle System`
3. Nommez-le "MuzzleFlash"
4. Positionnez-le au bout du canon (même position que FirePoint)

### Configuration

```
Main Module:
- Duration: 0.1
- Looping: OFF
- Start Lifetime: 0.1
- Start Speed: 0
- Start Size: 0.3
- Start Color: Orange/Yellow
- Play On Awake: OFF

Emission:
- Rate over Time: 0
- Bursts: 1 burst at time 0, count: 10-20

Shape:
- Shape: Cone
- Angle: 25
- Radius: 0.1

Color over Lifetime:
- Gradient: Yellow -> Orange -> Red -> Transparent

Size over Lifetime:
- Curve: Start small, grow quickly, then shrink

Renderer:
- Render Mode: Billboard
- Material: Default-Particle
```

4. Assignez ce Particle System dans le champ "Muzzle Flash" du WeaponController

## 2. Impact Effect (Effet d'impact)

### Création

1. `GameObject > Effects > Particle System`
2. Nommez-le "ImpactEffect"

### Configuration

```
Main Module:
- Duration: 0.5
- Looping: OFF
- Start Lifetime: 0.3-0.5
- Start Speed: 2-4
- Start Size: 0.05-0.15
- Start Color: Gray/Brown (pour impact mur) ou Rouge (pour sang)
- Play On Awake: ON

Emission:
- Rate over Time: 0
- Bursts: 1 burst at time 0, count: 20-30

Shape:
- Shape: Hemisphere
- Radius: 0.1

Color over Lifetime:
- Gradient vers transparent

Size over Lifetime:
- Shrink vers 0

Renderer:
- Material: Default-Particle
```

3. Créez un Prefab dans `Assets/Prefabs/`
4. Assignez dans WeaponController

## 3. Destruction Effect (Explosion de cible)

### Création

1. `GameObject > Effects > Particle System`
2. Nommez-le "DestructionEffect"

### Configuration

```
Main Module:
- Duration: 1
- Looping: OFF
- Start Lifetime: 0.5-1
- Start Speed: 3-8
- Start Size: 0.1-0.3
- Start Color: Couleur de la cible + variations
- Play On Awake: ON

Emission:
- Rate over Time: 0
- Bursts: 1 burst at time 0, count: 30-50

Shape:
- Shape: Sphere
- Radius: 0.3

Color over Lifetime:
- Gradient: Couleur vive -> Transparent

Size over Lifetime:
- Grow then shrink

Velocity over Lifetime:
- Linear: Y = -2 (gravity effect)

Renderer:
- Material: Default-Particle
```

### Ajouter des morceaux

Pour plus de "juice", ajoutez un deuxième Particle System comme enfant pour des morceaux :

```
- Start Speed: 5-10
- Start Size: 0.05-0.1
- Mesh: Cube
- Gravity Modifier: 1
- Collision Module: ON (pour que les morceaux rebondissent)
```

3. Créez un Prefab
4. Assignez dans Target script

## 4. Laser Sight (Viseur laser)

### Configuration du Line Renderer

1. Sur votre arme, ajoutez un composant `Line Renderer`

```
Line Renderer:
- Positions: 2 (Start et End)
  - Position 0: (0, 0, 0)
  - Position 1: (0, 0, 1)
- Width: 0.005
- Color Gradient: Rouge vif -> Rouge transparent
- Corner Vertices: 5
- End Cap Vertices: 5
```

2. Créez un matériau pour le laser :
   - `Clic droit > Create > Material`
   - Shader: `Universal Render Pipeline > Unlit`
   - Color: Rouge vif (255, 0, 0)
   - Surface Type: Transparent
   - Emission: Activé, rouge vif

3. Assignez ce matériau au Line Renderer

## 5. Floating Text (Points flottants)

### Création du Prefab

1. `GameObject > 3D Object > TextMeshPro - Text`
2. Nommez-le "FloatingText"

### Configuration

```
TextMeshPro:
- Text: "+10"
- Font Size: 1
- Alignment: Center
- Color: Yellow (#FFFF00)
- Vertex Color: Yellow avec gradient vers transparent
- Extra Settings > Sorting Layer: Overlay
```

3. Ajoutez le script `FloatingText.cs`
4. Créez un Prefab

## 6. Matériaux Émissifs (Glowing)

### Pour rendre des objets lumineux

1. Créez un nouveau matériau
2. Shader: `Universal Render Pipeline > Lit`
3. Dans la section "Emission" :
   - Cochez "Emission"
   - Color: Couleur vive (ex: cyan, magenta, jaune)
   - Intensity: 2-5 pour un effet marqué

### Bloom Post-Processing

Pour intensifier l'effet des matériaux émissifs :

1. Sur la caméra (dans XR Origin), ajoutez un `Volume` component
2. Créez un nouveau Profile
3. Add Override > Bloom
   - Intensity: 0.5-1
   - Threshold: 0.9
   - Scatter: 0.7

## 7. Trail Effect (Traînée de projectile)

Si vous utilisez des projectiles :

1. Sur votre prefab de balle, ajoutez un `Trail Renderer`

```
Trail Renderer:
- Time: 0.3
- Width: 0.05 -> 0.01
- Color: Gradient (blanc vif -> transparent)
- Material: Particules avec Additive blending
```

## 8. Matériaux pour Cibles

### Matériau de base

```
Material:
- Shader: URP/Lit
- Base Color: Couleur vive
- Metallic: 0
- Smoothness: 0.3
```

### Animation de dissolution (Optionnel - Avancé)

Pour faire disparaître les cibles progressivement au lieu de les détruire instantanément :

1. Utilisez un shader avec dissolution (Unity Asset Store)
2. Ou créez un Shader Graph simple avec un effet de "dissolve"

## 9. Optimisations pour Quest

### Limites à respecter

```
- Max particles actives simultanément: ~1000
- Max particle systems actifs: ~10
- Texture size: 512x512 max pour particules
- Éviter les matériaux complexes
- Utiliser "Unlit" quand possible
```

### Settings recommandés

```
Quality Settings pour Android:
- Pixel Light Count: 1
- Texture Quality: Medium
- Anisotropic Textures: Disabled
- Anti Aliasing: Disabled ou 2x
- Soft Particles: Disabled
- Realtime Reflection Probes: Disabled
```

## 10. Exemples de Palettes de Couleurs

### Sci-Fi
- Primaire: Cyan (#00FFFF)
- Secondaire: Magenta (#FF00FF)
- Accent: Orange (#FF8800)

### Fantasy
- Primaire: Violet (#8800FF)
- Secondaire: Or (#FFD700)
- Accent: Vert émeraude (#00FF88)

### Cyberpunk
- Primaire: Rose néon (#FF0088)
- Secondaire: Bleu électrique (#0088FF)
- Accent: Jaune (#FFFF00)

### Western
- Primaire: Marron (#8B4513)
- Secondaire: Rouge (#CC0000)
- Accent: Or (#FFD700)

## 11. Sons pour compléter les effets

Les effets visuels sont 2x plus efficaces avec des sons !

### Sons recommandés à chercher sur Freesound.org

```
Tir d'arme:
- "gun shot"
- "laser shot"
- "bow release"

Impact:
- "bullet impact"
- "hit"
- "thud"

Explosion:
- "explosion"
- "pop"
- "burst"

UI:
- "button click"
- "beep"
- "whoosh"

Ambiance:
- "wind"
- "crowd"
- Musique d'action/épique
```

## 12. Checklist Effets Visuels

- [ ] Muzzle flash configuré et assigné
- [ ] Impact effects créés (mur, cible)
- [ ] Destruction effects avec particules
- [ ] Laser sight fonctionnel
- [ ] Floating text pour les points
- [ ] Matériaux émissifs sur éléments importants
- [ ] Trail sur projectiles (si applicable)
- [ ] Palette de couleurs cohérente
- [ ] Tous les prefabs créés et assignés
- [ ] Testé dans le casque (performance OK)

## Conseil Final

**Principe du Juice** : Chaque action du joueur doit avoir un feedback visuel/sonore immédiat.

- Tirer → Flash + Son + Recul visuel
- Toucher cible → Impact particles + Son + Points flottants
- Détruire cible → Explosion + Morceaux + Son fort
- Recharger → Animation + Son métallique
- Gagner points → Texte + Effet + Son satisfaisant

Plus il y a de feedback, plus le jeu est satisfaisant ! 🎮✨
